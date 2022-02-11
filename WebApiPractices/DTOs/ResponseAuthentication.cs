using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.DTOs
{
    public class ResponseAuthentication
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
