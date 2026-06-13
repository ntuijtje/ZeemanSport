using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Subscription;

namespace ZeemanSport.Core.Subscription
{
    public interface ISubscriptionRepository
    {
        Task<IReadOnlyCollection<SubscriptionPlanResponse>> GetPlansAsync();
        Task<SubscriptionPlanResponse?> GetPlanByIdAsync(int planId);
        Task<IReadOnlyCollection<SubscriptionResponse>> GetByUserAsync(int userId);
        Task<SubscriptionResponse?> GetActiveByUserAsync(int userId);
        Task<SubscriptionResponse?> GetByIdAsync(int id);
        Task<SubscriptionResponse> SaveAsync(Subscription subscription);
    }
}
