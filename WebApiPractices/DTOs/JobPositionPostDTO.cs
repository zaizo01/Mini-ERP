using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class JobPositionPostDTO
    {
        public string JobTitle { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
    }
}
