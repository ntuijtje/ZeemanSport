using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.Reservation
{
    public class WaitlistRequest
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
    }
}
