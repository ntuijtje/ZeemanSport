using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.User;

namespace ZeemanSport.Core.Contracts.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole UserRole { get; set; }
    }
}
