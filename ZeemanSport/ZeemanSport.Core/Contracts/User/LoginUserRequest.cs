using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.User
{
    public class LoginUserRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
