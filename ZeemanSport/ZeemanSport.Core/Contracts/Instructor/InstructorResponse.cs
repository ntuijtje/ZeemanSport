using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.Instructor
{
    public class InstructorResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
