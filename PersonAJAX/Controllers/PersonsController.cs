using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using Newtonsoft.Json;
using PersonAJAX.Models;

namespace PersonAJAX.Controllers
{
    public class PersonsController : Controller
    {
        private readonly string _connectionString;

        public PersonsController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }


        public ActionResult Index()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();

                var query = "SELECT p.PersonId, p.PersonName, p.Email, p.DepartmentId, d.DepartmentNames " +
            "FROM Person p LEFT JOIN Departments d ON p.DepartmentId = d.DepartmentId";

                var persons = dbConnection.Query<Persons, Departments, Persons>(query, (person, department) =>
                {
                    person.Departments = department;
                    return person;
                }, splitOn: "DepartmentNames").ToList();

                // Veritabanından departmanları alma
                var departments = dbConnection.Query<Departments>("SELECT * FROM Departments").ToList();
                // SelectList'i manuel olarak oluştur
                var departmentSelectList = new SelectList(departments, "DepartmentId", "DepartmentNames");
                // SelectList'i görünüme aktarın
                ViewBag.ListOfDepartment = departmentSelectList;

                return View(persons);
            }
        }


        [HttpPost]
        public JsonResult AddPerson(Persons persons)
        {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();
                    var query = "INSERT INTO Person (PersonName, Email, DepartmentId) VALUES (@PersonName, @Email, @DepartmentId); SELECT CAST(SCOPE_IDENTITY() as int)";
                    int PersonID = dbConnection.Query<int>(query, new { personName = persons.PersonName, email = persons.Email, persons.DepartmentId  }).Single();
                    return Json(new { success = true, PersonID });
                }

        }

        public JsonResult GetPersonById(int PersonId)
        {

                using (var dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();
                    var model = dbConnection.QueryFirstOrDefault<Persons>(
                        "SELECT * FROM Person WHERE PersonId = @PersonId",
                        new { PersonId });

                    if (model != null)
                    {
                        string value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        return Json(value, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Person not found", JsonRequestBehavior.AllowGet);
                    }
                }
        }

        [HttpPost]
        public JsonResult UpdatePerson(Persons persons)
        {
            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();

                    var query = "UPDATE Person SET PersonName = @PersonName, Email = @Email, DepartmentId=@DepartmentId WHERE PersonId = @PersonId";

                    dbConnection.Execute(query, new
                    {
                        PersonName = persons.PersonName,
                        Email = persons.Email,
                        PersonId = persons.PersonId,
                        DepartmentId = persons.DepartmentId
                    });

                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex.Message });
            }
        }



        [HttpPost]
        public JsonResult DeletePerson(int PersonId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var query = "DELETE FROM Person WHERE PersonId = @PersonId";
                dbConnection.Execute(query, new { PersonId = PersonId });
                return Json(new { success = true });
            }
        }
    }
}
  