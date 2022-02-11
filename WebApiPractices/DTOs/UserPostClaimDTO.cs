using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class UserPostClaimDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public UserClaim UserClaim { get; set; }
    }

    public enum UserClaim
    {
        Admin = 1,
        Manager = 2,
        Invited = 3
    }
}
