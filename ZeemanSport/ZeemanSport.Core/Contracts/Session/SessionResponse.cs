using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Location;
using ZeemanSport.Core.Session;

namespace ZeemanSport.Core.Contracts.Session
{
    public class SessionResponse
    {
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public string? WorkoutName { get; set; }
        public int? InstructorId { get; set; }
        public string? InstructorName { get; set; }
        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public LocationType LocationType { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public SessionStatus Status { get; set; }
        public int Capacity { get; set; }
        public int ReservedCount { get; set; }
        public int WaitlistCount { get; set; }

        /// <summary>
        /// Seat layout, only populated for spinning sessions where a member picks a specific bike.
        /// Null for regular group/outdoor sessions.
        /// </summary>
        public IReadOnlyList<Seat>? Seats { get; set; }
    }
}
