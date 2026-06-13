using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Subscription;

namespace ZeemanSport.Core.Subscription
{
    public interface ISubscriptionService
    {
        Task<IReadOnlyCollection<SubscriptionPlanResponse>> GetPlansAsync();
        Task<IReadOnlyCollection<SubscriptionResponse>> GetForUserAsync(int userId);
        Task<SubscriptionResponse?> GetActiveForUserAsync(int userId);
        Task<SubscriptionResponse> PurchaseAsync(CreateSubscriptionRequest request);
        Task<SubscriptionResponse?> UpgradeAsync(int userId, UpgradeSubscriptionRequest request);
        Task<SubscriptionResponse?> CancelAsync(int userId);
    }
}
