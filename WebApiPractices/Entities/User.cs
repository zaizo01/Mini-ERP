using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.Entities
{
    public class User: IdentityUser
    {

        public int ConfirmNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
