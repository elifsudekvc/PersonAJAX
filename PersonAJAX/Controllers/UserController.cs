using Dapper;
using Newtonsoft.Json;
using PersonAJAX.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PersonAJAX.Controllers
{
    public class UserController : Controller
    {
        private readonly string _connectionString;

        public UserController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        }
        [CustomAuthorize("Admin")]
        public async Task<ActionResult> Index()
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();

                var query = "SELECT u.UserId, u.UserName, u.UserEmail, u.Password, u.RoleId, r.RoleName " +
            "FROM Users u LEFT JOIN UserRoles r ON u.RoleId = r.RoleId";

                var users = (await dbConnection.QueryAsync<User, UserRole, User>(
                    query,
                    (user, userrole) =>
                    {
                        user.Role = userrole;
                        return user;
                    },
                    splitOn: "RoleName")).ToList();

                var userroles = (await dbConnection.QueryAsync<UserRole>("SELECT * FROM UserRoles")).ToList();

                var userRoleSelectList = new SelectList(userroles, "RoleId", "RoleName");

                ViewBag.ListOfUserRole = userRoleSelectList;

                return View(users);
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddUser(User user)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(_connectionString))
                {
                    await dbConnection.OpenAsync();

                    user.HashPassword();

                    var query = "INSERT INTO Users (UserName, UserEmail, Password, RoleId) VALUES (@UserName, @UserEmail, @Password, @RoleId); SELECT CAST(SCOPE_IDENTITY() as int)";


                        int UserId = (await dbConnection.QueryAsync<int>(query, new
                        {
                            userName = user.UserName,
                            userEmail = user.UserEmail,
                            password = user.Password,
                            user.RoleId
                        })).Single();

                        return Json(new { success = true, UserId });
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex.Message });
            }
        }


        [HttpPost]
        public async Task<JsonResult> UpdateUser(User user)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(_connectionString))
                {
                    await dbConnection.OpenAsync();
                    var query = "UPDATE Users SET UserName = @UserName, UserEmail = @UserEmail, RoleId=@RoleId WHERE UserId = @UserId";

                    await dbConnection.ExecuteAsync(query, new
                    {
                        UserName = user.UserName,
                        UserEmail = user.UserEmail,
                        UserId = user.UserId,
                        RoleId = user.RoleId
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
        public async Task<JsonResult> DeleteUser(int UserId)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();
                var query = "DELETE FROM Users WHERE UserId = @UserId";
                await dbConnection.ExecuteAsync(query, new { UserId = UserId });
                return Json(new { success = true });
            }
        }
    }
}