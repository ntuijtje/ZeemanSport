using System.Net.Http.Json;
using ZeemanSport.Core.Contracts.Subscription;

namespace ZeemanSport.BlazorWasm.Services
{
    public class SubscriptionApiClient
    {
        private readonly HttpClient _httpClient;

        public SubscriptionApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<SubscriptionPlanResponse>> GetPlansAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<SubscriptionPlanResponse>>("Subscription/Plans", ApiSerialization.Options)
                ?? new List<SubscriptionPlanResponse>();
        }

        public async Task<IReadOnlyList<SubscriptionResponse>> GetForUserAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<List<SubscriptionResponse>>($"Subscription/User/{userId}", ApiSerialization.Options)
                ?? new List<SubscriptionResponse>();
        }

        public async Task<SubscriptionResponse?> PurchaseAsync(CreateSubscriptionRequest request)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Subscription", request, ApiSerialization.Options);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<SubscriptionResponse>(ApiSerialization.Options);
        }
    }
}
