using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<ActionResult> Index()
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();

                var query = "SELECT p.PersonId, p.PersonName, p.Email, p.ProfileImage, p.DepartmentId, d.DepartmentNames " +
            "FROM Person p LEFT JOIN Departments d ON p.DepartmentId = d.DepartmentId";

                var persons = (await dbConnection.QueryAsync<Persons, Departments, Persons>(
                    query,
                    (person, department) =>
                    {
                        person.Departments = department;
                        return person;
                    },
                    splitOn: "DepartmentNames")).ToList();

                var departments = (await dbConnection.QueryAsync<Departments>("SELECT * FROM Departments")).ToList();

                var departmentSelectList = new SelectList(departments, "DepartmentId", "DepartmentNames");

                ViewBag.ListOfDepartment = departmentSelectList;

                return View(persons);
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddPerson(Persons persons)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(_connectionString))
                {
                    await dbConnection.OpenAsync();
                    var query = "INSERT INTO Person (PersonName, Email, DepartmentId, ProfileImage) VALUES (@PersonName, @Email, @DepartmentId, @ProfileImage); SELECT CAST(SCOPE_IDENTITY() as int)";

                    
                    using (Stream fileStream = persons.ImageUpload.InputStream)
                    {
                        
                        string relativePath = "/AppFile/Images/" + persons.ImageUpload.FileName;
                        
                        persons.ImageUpload.SaveAs(Server.MapPath("~" + relativePath));

                        persons.ProfileImage = relativePath;

                        int PersonID = (await dbConnection.QueryAsync<int>(query, new
                        {
                            personName = persons.PersonName,
                            email = persons.Email,
                            profileImage = persons.ProfileImage,
                            persons.DepartmentId
                        })).Single();

                        return Json(new { success = true, PersonID });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex.Message });
            }
        }



        public async Task<JsonResult> GetPersonById(int PersonId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();
                var model = await dbConnection.QueryFirstOrDefaultAsync<Persons>(
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
        public async Task<JsonResult> UpdatePerson(Persons persons)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(_connectionString))
                {
                    await dbConnection.OpenAsync();

                    if (persons.ImageUpload != null && persons.ImageUpload.ContentLength > 0)
                    {
                        
                        using (Stream fileStream = persons.ImageUpload.InputStream)
                        {
                            string relativePath = "/AppFile/Images/" + persons.ImageUpload.FileName;
                            persons.ImageUpload.SaveAs(Server.MapPath("~" + relativePath));
                            persons.ProfileImage = relativePath;
                        }
                    }

                    var query = "UPDATE Person SET PersonName = @PersonName, Email = @Email, DepartmentId=@DepartmentId, ProfileImage=@ProfileImage WHERE PersonId = @PersonId";

                    await dbConnection.ExecuteAsync(query, new
                    {
                        PersonName = persons.PersonName,
                        Email = persons.Email,
                        PersonId = persons.PersonId,
                        ProfileImage = persons.ProfileImage,
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
        public async Task<JsonResult> DeletePerson(int PersonId)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();
                var query = "DELETE FROM Person WHERE PersonId = @PersonId";
                await dbConnection.ExecuteAsync(query, new { PersonId = PersonId });
                return Json(new { success = true });
            }
        }
    }
}
