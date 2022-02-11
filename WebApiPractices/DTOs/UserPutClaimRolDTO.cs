using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class UserPutClaimRolDTO
    {
        public UserClaimRol UserClaim { get; set; }

        public enum UserClaimRol
        {
            Admin = 1,
            Manager = 2,
            Invited = 3
        }
    }
}
