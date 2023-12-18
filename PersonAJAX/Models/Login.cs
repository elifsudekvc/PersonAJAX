using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PersonAJAX.Models
{
    public class Login
    {

        public string UserEmail { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}