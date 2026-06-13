using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Subscription
{
    public enum AccessTier
    {
        TwiceWeekly = 1,
        Unlimited = 2
    }

    public enum BillingInterval
    {
        Monthly = 1,
        Yearly = 2
    }

    public enum SubscriptionStatus
    {
        Pending = 1,
        Active = 2,
        Cancelled = 4,
        Expired = 8
    }
}
