using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class UserForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
