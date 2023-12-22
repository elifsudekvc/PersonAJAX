using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PersonAJAX.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string UserEmail { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public void HashPassword()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public UserRole Role { get; set; }

        public User()
        {
            RoleId = 1;
        }
    }
}