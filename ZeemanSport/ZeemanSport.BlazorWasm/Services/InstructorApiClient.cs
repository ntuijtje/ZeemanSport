using System.Net.Http.Json;
using ZeemanSport.Core.Contracts.Instructor;

namespace ZeemanSport.BlazorWasm.Services
{
    public class InstructorApiClient
    {
        private readonly HttpClient _httpClient;

        public InstructorApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<InstructorResponse>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<InstructorResponse>>("Instructor", ApiSerialization.Options)
                ?? new List<InstructorResponse>();
        }

        public async Task<InstructorResponse?> CreateAsync(CreateInstructorRequest request)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Instructor", request, ApiSerialization.Options);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<InstructorResponse>(ApiSerialization.Options);
        }

        public async Task<InstructorResponse?> UpdateAsync(int id, UpdateInstructorRequest request)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Instructor/{id}", request, ApiSerialization.Options);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<InstructorResponse>(ApiSerialization.Options);
        }

        public async Task DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Instructor/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
