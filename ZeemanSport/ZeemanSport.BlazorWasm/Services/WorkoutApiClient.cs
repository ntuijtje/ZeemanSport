using System.Net.Http.Json;
using ZeemanSport.Core.Contracts.Workout;

namespace ZeemanSport.BlazorWasm.Services
{
    public class WorkoutApiClient
    {
        private readonly HttpClient _httpClient;

        public WorkoutApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<WorkoutResponse>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<WorkoutResponse>>("Workout", ApiSerialization.Options)
                ?? new List<WorkoutResponse>();
        }

        public async Task<WorkoutResponse?> CreateAsync(CreateWorkoutRequest request)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Workout", request, ApiSerialization.Options);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<WorkoutResponse>(ApiSerialization.Options);
        }

        public async Task<WorkoutResponse?> UpdateAsync(int id, UpdateWorkoutRequest request)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Workout/{id}", request, ApiSerialization.Options);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<WorkoutResponse>(ApiSerialization.Options);
        }

        public async Task DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Workout/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
