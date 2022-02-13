using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class EmployeePostDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Extension { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime BirthDate { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid JobPositionId { get; set; }
    }
}
