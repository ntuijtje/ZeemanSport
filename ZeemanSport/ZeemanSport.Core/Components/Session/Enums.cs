using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Session
{
    public enum SessionStatus
    {
        Scheduled = 1,
        Cancelled = 2,
        Completed = 4
    }

    public enum RecurrenceType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 4
    }
}
