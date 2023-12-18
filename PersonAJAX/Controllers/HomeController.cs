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
    public class HomeController : Controller
    {
        private readonly string _connectionString;

        public HomeController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        }
        public ActionResult Index()
        {
            return View();
        }

        [Authorize (Roles ="User, Admin")]
        public async Task<ActionResult> UserArea()
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
        [Authorize (Roles ="Admin")]
        public ActionResult AdminArea()
        {
            return View();
        }

    }
}