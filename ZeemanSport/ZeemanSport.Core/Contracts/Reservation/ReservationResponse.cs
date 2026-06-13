using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Location;
using ZeemanSport.Core.Reservation;

namespace ZeemanSport.Core.Contracts.Reservation
{
    public class ReservationResponse
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public ReservationStatus Status { get; set; }
        public int? SeatRow { get; set; }
        public int? SeatColumn { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public CheckInMethod? CheckInMethod { get; set; }

        // Enriched session details so a member's reservation list is self-contained.
        public string? WorkoutName { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public string? LocationName { get; set; }
        public LocationType LocationType { get; set; }
    }
}
