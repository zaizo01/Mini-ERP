using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class DepartmentGetDTO
    {
        public Guid Id { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }
    }
}
