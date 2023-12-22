using Dapper;
using PersonAJAX.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PersonAJAX.Controllers
{
    public class LoginController : Controller
    {

        private readonly string _connectionString;

        public LoginController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        }

        // GET: Login
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(User user)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();

                user.HashPassword();

                await dbConnection.QueryAsync<User>("INSERT INTO Users (UserName, UserEmail, Password, RoleId) VALUES (@UserName, @UserEmail, @Password, @RoleId)", user);
                ModelState.Clear();
            }
            FormsAuthentication.SetAuthCookie(user.UserEmail, true);


            Session["UserId"] = user.UserId;
            Session["UserName"] = user.UserName;
            Session["UserRole"] = user.RoleId;
            return RedirectToAction("Index", "Home");
        }



        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(Login login)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                await dbConnection.OpenAsync();

                var user = await dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE UserEmail = @UserEmail", login);

                if (user != null)
                {
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(login.Password));
                        string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                        if (user.Password == hashedPassword)
                        {
                            
                            FormsAuthentication.SetAuthCookie(user.UserEmail, true);

                            Session["UserId"] = user.UserId;
                            Session["UserName"] = user.UserName;
                            Session["UserRole"] = user.RoleId;
                            Session["UserEmail"] = user.UserEmail;
                            Session["UserPassword"] = login.Password; // Şifre değerini oturuma ekle


                            if (user.RoleId == 1)
                            {
                                return RedirectToAction("UserArea", "Home");
                            }
                            else
                            {
                                return RedirectToAction("AdminArea", "Home");
                            }
                        }
                    }
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View();
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}