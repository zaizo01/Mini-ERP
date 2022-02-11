using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.Entities
{
    public class JobPosition
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
