using System.Net.Http.Json;
using ZeemanSport.Core.Contracts.Location;

namespace ZeemanSport.BlazorWasm.Services
{
    public class LocationApiClient
    {
        private readonly HttpClient _httpClient;

        public LocationApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<LocationResponse>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<LocationResponse>>("Location", ApiSerialization.Options)
                ?? new List<LocationResponse>();
        }
    }
}
