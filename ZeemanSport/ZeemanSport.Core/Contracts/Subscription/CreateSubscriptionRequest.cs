using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.Subscription
{
    public class CreateSubscriptionRequest
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
