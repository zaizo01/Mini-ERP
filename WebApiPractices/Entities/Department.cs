using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
