using PersonAJAX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PersonAJAX.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user)
        {
            using (DB db = new DB())
            {
                db.Users.Add(user);
                db.SaveChanges();
                ModelState.Clear();
            }
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            using (DB db = new DB())
            {
                //kullanıcı adı ve şifre bilgilerini kullanarak kullanıcıyı kimlik doğrulama işlemine tabi tutar.
                var user=db.Users.Where(a=>a.UserEmail==login.UserEmail&&a.Password==login.Password).FirstOrDefault();
                //Eğer geçerli bir kullanıcı bulunursa, bu kullanıcı için bir FormsAuthenticationTicket oluşturulur.
                //Bu bileti şifrelemek ve kullanıcıya bir çerez olarak sunmak için FormsAuthentication.Encrypt metodu kullanılır. 
                if (user!=null)
                {
                    var Ticket = new FormsAuthenticationTicket(login.UserEmail, true, 3000);
                    string Encrypt = FormsAuthentication.Encrypt(Ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, Encrypt);
                    cookie.Expires= DateTime.Now.AddHours(3000);
                    cookie.HttpOnly= true;
                    Response.Cookies.Add(cookie);
                    //Kullanıcının rolüne göre yönlendirme yapılır. Kullanıcının rolü 1 ise "UserArea" sayfasına, değilse "AdminArea" sayfasına yönlendirilir.
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
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}