using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonAJAX.Models
{
    public partial class Departments
    {
        public Departments()
        {
            this.Person = new HashSet<Persons>();
        }

        public int DepartmentId { get; set; }
        public string DepartmentNames { get; set; }

        public virtual ICollection<Persons> Person { get; set; }
    }
}