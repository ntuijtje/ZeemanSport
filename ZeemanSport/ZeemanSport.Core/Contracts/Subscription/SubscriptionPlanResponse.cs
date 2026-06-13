using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Subscription;

namespace ZeemanSport.Core.Contracts.Subscription
{
    public class SubscriptionPlanResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public AccessTier AccessTier { get; set; }
        public BillingInterval BillingInterval { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
