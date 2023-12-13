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
    public class DepartmentController : Controller
    {
        private readonly string _connectionString;

        public DepartmentController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}