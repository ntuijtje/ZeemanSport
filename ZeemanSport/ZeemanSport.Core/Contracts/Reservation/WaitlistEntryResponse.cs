using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.Reservation
{
    public class WaitlistEntryResponse
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public int Position { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
