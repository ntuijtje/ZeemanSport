using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Reservation
{
    public enum ReservationStatus
    {
        Reserved = 1,
        Cancelled = 2,
        CheckedIn = 4
    }

    public enum CheckInMethod
    {
        Manual = 1,
        Rfid = 2,
        Gps = 4
    }
}
