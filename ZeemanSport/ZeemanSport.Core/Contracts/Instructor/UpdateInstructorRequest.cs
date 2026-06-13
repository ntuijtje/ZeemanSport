using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.Instructor
{
    public class UpdateInstructorRequest
    {
        public string? Name { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
