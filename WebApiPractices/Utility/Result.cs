using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.Utility
{
    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public string FriendlyMessage { get; set; }
        public T Data { get; set; }
        public string StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }

        public static Result<T> Success(T data, string message = null)
        {
            return new Result<T> { Succeeded = true, Data = data, FriendlyMessage = message };
        }

        internal static Result<bool> Success(bool v1, string v2)
        {
            return new Result<bool> { Succeeded = v1, Data = v1, FriendlyMessage = v2 };
        }

        public static IActionResult Fail(string v)
        {
            throw new NotImplementedException();
        }
    }
}
