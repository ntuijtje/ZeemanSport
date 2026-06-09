using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Session;

namespace ZeemanSport.Core.Contracts.Session
{
    public class UpdateSessionRequest
    {
        public int WorkoutId { get; set; }
        public int? InstructorId { get; set; }
        public int LocationId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public SessionStatus Status { get; set; }
    }
}
