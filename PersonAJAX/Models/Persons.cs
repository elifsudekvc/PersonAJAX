using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonAJAX.Models
{
    public class Persons
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        public string Email { get; set; }

        public Nullable<int> DepartmentId { get; set; }

        public string DepartmentNames { get; set; }

        public virtual Departments Departments { get; set; }

    }
}