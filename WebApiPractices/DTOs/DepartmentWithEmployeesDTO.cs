using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPractices.Entities;

namespace WebApiPractices.DTOs
{
    public class DepartmentWithEmployeesDTO
    {
        public Guid Id { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }
        public List<EmployeeDepartmentDTO> Employees { get; set; }
    }
}
