using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPractices.Entities;

namespace WebApiPractices.DTOs
{
    public class JobPositionGetDTO
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
