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
using Dapper;

namespace PersonAJAX
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly string _connectionString;

        public MvcApplication()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        }
        protected void Application_Start()
        {

            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();

                
                int count = dbConnection.ExecuteScalar<int>("SELECT COUNT(*) FROM UserRoles");

                if (count == 0)
                {
                    dbConnection.Execute("INSERT INTO UserRoles (RoleName) VALUES (@RoleName)", new { RoleName = "User" });
                    dbConnection.Execute("INSERT INTO UserRoles (RoleName) VALUES (@RoleName)", new { RoleName = "Admin" });
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
