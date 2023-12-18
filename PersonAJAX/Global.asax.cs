using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using PersonAJAX.Models;

namespace PersonAJAX
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            using (DB db = new DB())
            {
                if (db.Roles.Count()==0)
                {
                    var role1 = new UserRole();
                    var role2 = new UserRole();
                    role1.RoleName = "User";
                    role2.RoleName = "Admin";
                    db.Roles.Add(role1);
                    db.Roles.Add(role2);
                    db.SaveChanges();
                }
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        }

    }
}
