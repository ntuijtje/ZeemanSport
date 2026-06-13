using ZeemanSport.Core.Contracts.Subscription;
using ZeemanSport.Core.Subscription;

namespace ZeemanSport.Runtime.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<IReadOnlyCollection<SubscriptionPlanResponse>> GetPlansAsync()
        {
            return await _subscriptionRepository.GetPlansAsync();
        }

        public async Task<IReadOnlyCollection<SubscriptionResponse>> GetForUserAsync(int userId)
        {
            return await _subscriptionRepository.GetByUserAsync(userId);
        }

        public async Task<SubscriptionResponse?> GetActiveForUserAsync(int userId)
        {
            return await _subscriptionRepository.GetActiveByUserAsync(userId);
        }

        public async Task<SubscriptionResponse> PurchaseAsync(CreateSubscriptionRequest request)
        {
            SubscriptionPlanResponse? plan = await _subscriptionRepository.GetPlanByIdAsync(request.PlanId);

            if (plan == null)
                throw new InvalidOperationException("The selected subscription plan does not exist.");

            DateTime startDate = request.StartDate.Date;

            Subscription subscription = new Subscription
            {
                UserId = request.UserId,
                PlanId = plan.Id,
                StartDate = startDate,
                EndDate = CalculateEndDate(startDate, plan.BillingInterval),
                // A subscription that starts in the future stays pending until it begins.
                Status = startDate <= DateTime.UtcNow.Date ? SubscriptionStatus.Active : SubscriptionStatus.Pending
            };

            return await _subscriptionRepository.SaveAsync(subscription);
        }

        public async Task<SubscriptionResponse?> UpgradeAsync(int userId, UpgradeSubscriptionRequest request)
        {
            SubscriptionResponse? active = await _subscriptionRepository.GetActiveByUserAsync(userId);

            if (active == null)
                return null;

            if (active.AccessTier == AccessTier.Unlimited)
                throw new InvalidOperationException("The subscription already has unlimited access.");

            SubscriptionPlanResponse? targetPlan = await _subscriptionRepository.GetPlanByIdAsync(request.TargetPlanId);

            if (targetPlan == null)
                throw new InvalidOperationException("The selected subscription plan does not exist.");

            if (targetPlan.AccessTier != AccessTier.Unlimited || targetPlan.BillingInterval != active.BillingInterval)
                throw new InvalidOperationException("A subscription can only be upgraded to the unlimited tier of the same billing interval.");

            // Keep the existing period; only the access tier (plan) changes. The price difference is
            // settled through the (simulated) payment flow and is out of scope for this service.
            Subscription subscription = new Subscription
            {
                UserId = active.UserId,
                PlanId = targetPlan.Id,
                StartDate = active.StartDate,
                EndDate = active.EndDate,
                Status = active.Status
            };
            subscription.SetId(active.Id);

            return await _subscriptionRepository.SaveAsync(subscription);
        }

        public async Task<SubscriptionResponse?> CancelAsync(int userId)
        {
            SubscriptionResponse? active = await _subscriptionRepository.GetActiveByUserAsync(userId);

            if (active == null)
                return null;

            if (active.BillingInterval == BillingInterval.Yearly)
                throw new InvalidOperationException("A yearly subscription cannot be cancelled before it expires.");

            Subscription subscription = new Subscription
            {
                UserId = active.UserId,
                PlanId = active.PlanId,
                StartDate = active.StartDate,
                EndDate = active.EndDate,
                Status = SubscriptionStatus.Cancelled
            };
            subscription.SetId(active.Id);

            return await _subscriptionRepository.SaveAsync(subscription);
        }

        private static DateTime CalculateEndDate(DateTime startDate, BillingInterval billingInterval)
        {
            return billingInterval switch
            {
                BillingInterval.Monthly => startDate.AddMonths(1),
                BillingInterval.Yearly => startDate.AddYears(1),
                _ => startDate
            };
        }
    }
}
