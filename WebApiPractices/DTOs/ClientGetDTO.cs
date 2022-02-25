using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class ClientGetDTO
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public string Status { get; set; }
    }
}
