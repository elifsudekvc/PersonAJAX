using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using Newtonsoft.Json;
using PersonAJAX.Models;

namespace PersonAJAX.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly string _connectionString;

        public DepartmentsController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }


        public async Task<ActionResult> Index()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var departments = await dbConnection.QueryAsync<Departments>("SELECT * FROM Departments");
                return View(departments);
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddDepartments(Departments departments)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();
                var query = "INSERT INTO Departments (DepartmentNames) VALUES (@DepartmentNames); SELECT CAST(SCOPE_IDENTITY() as int)";
                int DepartmentID = (await dbConnection.QueryAsync<int>(query, new
                {
                    departmentNames = departments.DepartmentNames,
                })).Single();

                return Json(new { success = true, DepartmentID });
            }
        }

        public async Task<JsonResult> GetPersonById(int DepartmentId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();
                var model = await dbConnection.QueryFirstOrDefaultAsync<Departments>(
                    "SELECT * FROM Departments WHERE DepartmentId = @DepartmentId",
                    new { DepartmentId });

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
                    return Json("Department not found", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateDepartments(Departments departments)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(_connectionString))
                {
                    await dbConnection.OpenAsync();
                    var query = "UPDATE Departments SET DepartmentNames = @DepartmentNames WHERE DepartmentId = @DepartmentId";

                    await dbConnection.ExecuteAsync(query, new
                    {
                        DepartmentNames = departments.DepartmentNames,
                        DepartmentId = departments.DepartmentId
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
        public async Task<JsonResult> DeleteDepartments(int DepartmentId)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();
                var query = "DELETE FROM Departments WHERE DepartmentId = @DepartmentId";
                await dbConnection.ExecuteAsync(query, new { DepartmentId = DepartmentId });
                return Json(new { success = true });
            }
        }
    }
}