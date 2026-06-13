using System.Net.Http.Json;
using ZeemanSport.Core.Contracts.User;

namespace ZeemanSport.BlazorWasm.Services
{
    public class UserApiClient
    {
        private readonly HttpClient _httpClient;

        public UserApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<UserResponse>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<UserResponse>>("User", ApiSerialization.Options)
                ?? new List<UserResponse>();
        }

        public async Task<UserResponse?> RegisterAsync(RegisterUserRequest request)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("User/Register", request, ApiSerialization.Options);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UserResponse>(ApiSerialization.Options);
        }
    }
}
