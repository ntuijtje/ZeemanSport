using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Reservation;

namespace ZeemanSport.Core.Contracts.Reservation
{
    public class CheckInRequest
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public CheckInMethod CheckInMethod { get; set; }
    }
}
