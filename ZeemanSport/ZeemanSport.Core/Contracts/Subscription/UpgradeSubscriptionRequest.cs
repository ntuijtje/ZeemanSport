using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Contracts.Subscription
{
    /// <summary>
    /// Upgrades an active subscription to the unlimited tier of the same billing interval.
    /// Members can only extend their access (twice-weekly -> unlimited), never downgrade.
    /// </summary>
    public class UpgradeSubscriptionRequest
    {
        public int TargetPlanId { get; set; }
    }
}
