using Dapper;
using PersonAJAX.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace PersonAJAX
{
    public class Role : RoleProvider
    {
        private readonly string _connectionString;

        public Role()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        }
        public override string ApplicationName { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        //Belirli bir kullanıcının rollerini almak için kullanılır.
        //Bu metod, kullanıcı adını parametre olarak alır, veritabanında ilgili kullanıcıyı bulur ve onun rol adını döndürür.

        public override string[] GetRolesForUser(string username)
        {
            using (SqlConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();

                var userRole = dbConnection.QueryFirstOrDefault<string>("SELECT r.RoleName FROM Users u INNER JOIN UserRoles r ON u.RoleId = r.RoleId WHERE u.UserEmail = @UserEmail", new { UserEmail = username });

                if (userRole != null)
                {
                    return new string[] { userRole };
                }
            }

            return new string[0];
        }


        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}