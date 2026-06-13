using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.Reservation
{
    public class CreateReservationRequest
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }

        /// <summary>Chosen bike position, only used for spinning sessions.</summary>
        public int? SeatRow { get; set; }
        public int? SeatColumn { get; set; }
    }
}
