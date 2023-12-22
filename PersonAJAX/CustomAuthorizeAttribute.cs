using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PersonAJAX
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _roles;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var user = httpContext.User;

                var userRoles = httpContext.Session["UserRoles"] as List<string>;
                if (userRoles == null)
                {
                    userRoles = GetUserRolesFromDatabase(httpContext.User.Identity.Name);
                    httpContext.Session["UserRoles"] = userRoles;
                }


                return _roles.Any(user.IsInRole);
            }

            return false;
        }

        private List<string> GetUserRolesFromDatabase(string userEmail)
        {
            using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString))
            {
                dbConnection.Open();
                var roles = dbConnection.Query<string>("SELECT RoleName FROM UserRoles " +
                                                       "INNER JOIN Users ON UserRoles.RoleId = Users.RoleId " +
                                                       "WHERE Users.UserEmail = @UserEmail", new { UserEmail = userEmail }).ToList();

                return roles;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Login", action = "Login" }));
        }
    }



}