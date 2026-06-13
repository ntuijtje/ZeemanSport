using System.Net.Http.Json;
using ZeemanSport.Core.Contracts.Session;

namespace ZeemanSport.BlazorWasm.Services
{
    public class SessionApiClient
    {
        private readonly HttpClient _httpClient;

        public SessionApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<SessionResponse>> GetScheduleAsync(DateTime from, DateTime to)
        {
            string url = $"Session?from={from:yyyy-MM-ddTHH:mm:ss}&to={to:yyyy-MM-ddTHH:mm:ss}";

            return await _httpClient.GetFromJsonAsync<List<SessionResponse>>(url, ApiSerialization.Options)
                ?? new List<SessionResponse>();
        }

        public async Task<SessionResponse?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<SessionResponse>($"Session/{id}", ApiSerialization.Options);
        }

        public async Task<IReadOnlyList<SessionResponse>> CreateAsync(CreateSessionRequest request)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Session", request, ApiSerialization.Options);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<SessionResponse>>(ApiSerialization.Options)
                ?? new List<SessionResponse>();
        }

        public async Task<SessionResponse?> UpdateAsync(int id, UpdateSessionRequest request)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Session/{id}", request, ApiSerialization.Options);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<SessionResponse>(ApiSerialization.Options);
        }

        public async Task DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Session/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
