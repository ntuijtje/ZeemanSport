using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.Workout
{
    public class CreateWorkoutRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
