using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PersonAJAX.Models
{
    public class Email
    {
        [DisplayName("Enter Your Email")]
        public string SenderEmail { get; set; }

        [DisplayName("Email Password")]
        [DataType(DataType.Password)]
        public string SenderEmailPassword { get; set; }

        [DisplayName("Name Surname")]
        public string NameSurname { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Message")]
        public string Message { get; set; }
    }
}