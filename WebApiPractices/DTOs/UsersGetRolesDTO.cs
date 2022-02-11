using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class UsersGetRolesDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string ClaimRol { get; set; }
    }
}
