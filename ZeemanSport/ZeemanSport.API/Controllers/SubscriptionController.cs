using Microsoft.AspNetCore.Mvc;
using ZeemanSport.Core.Contracts.Subscription;
using ZeemanSport.Core.Subscription;

namespace ZeemanSport.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("Plans", Name = "GetSubscriptionPlans")]
        public async Task<ActionResult<IReadOnlyCollection<SubscriptionPlanResponse>>> GetPlans()
        {
            IReadOnlyCollection<SubscriptionPlanResponse> plans = await _subscriptionService.GetPlansAsync();

            return Ok(plans);
        }

        [HttpGet("User/{userId:int}", Name = "GetUserSubscriptions")]
        public async Task<ActionResult<IReadOnlyCollection<SubscriptionResponse>>> GetForUser(int userId)
        {
            IReadOnlyCollection<SubscriptionResponse> subscriptions = await _subscriptionService.GetForUserAsync(userId);

            return Ok(subscriptions);
        }

        [HttpGet("User/{userId:int}/Active", Name = "GetActiveSubscription")]
        public async Task<ActionResult<SubscriptionResponse>> GetActiveForUser(int userId)
        {
            SubscriptionResponse? subscription = await _subscriptionService.GetActiveForUserAsync(userId);

            if (subscription == null)
                return NotFound();

            return Ok(subscription);
        }

        [HttpPost(Name = "PurchaseSubscription")]
        public async Task<ActionResult<SubscriptionResponse>> Purchase(CreateSubscriptionRequest request)
        {
            try
            {
                SubscriptionResponse subscription = await _subscriptionService.PurchaseAsync(request);

                return CreatedAtAction(nameof(GetActiveForUser), new { userId = subscription.UserId }, subscription);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("User/{userId:int}/Upgrade", Name = "UpgradeSubscription")]
        public async Task<ActionResult<SubscriptionResponse>> Upgrade(int userId, UpgradeSubscriptionRequest request)
        {
            try
            {
                SubscriptionResponse? subscription = await _subscriptionService.UpgradeAsync(userId, request);

                if (subscription == null)
                    return NotFound();

                return Ok(subscription);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("User/{userId:int}/Cancel", Name = "CancelSubscription")]
        public async Task<ActionResult<SubscriptionResponse>> Cancel(int userId)
        {
            try
            {
                SubscriptionResponse? subscription = await _subscriptionService.CancelAsync(userId);

                if (subscription == null)
                    return NotFound();

                return Ok(subscription);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
