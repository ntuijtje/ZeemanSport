using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Reservation;

namespace ZeemanSport.Core.Contracts.Reservation
{
    /// <summary>
    /// A member that has reserved a session, as shown to other members and the instructor.
    /// </summary>
    public class ParticipantResponse
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int? SeatRow { get; set; }
        public int? SeatColumn { get; set; }
        public bool IsCheckedIn { get; set; }
    }
}
