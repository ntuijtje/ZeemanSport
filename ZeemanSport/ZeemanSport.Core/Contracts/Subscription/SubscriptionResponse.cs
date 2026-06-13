using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Subscription;

namespace ZeemanSport.Core.Contracts.Subscription
{
    public class SubscriptionResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public string? PlanName { get; set; }
        public AccessTier AccessTier { get; set; }
        public BillingInterval BillingInterval { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionStatus Status { get; set; }
    }
}
