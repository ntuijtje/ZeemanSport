using ZeemanSport.Core.Contracts.Location;

namespace ZeemanSport.Core.Location
{
    public interface ILocationService
    {
        Task<IReadOnlyCollection<LocationResponse>> GetAllAsync();
        Task<LocationResponse> GetByIdAsync(int id);
        Task<LocationResponse> CreateAsync(CreateLocationRequest request);
        Task<LocationResponse> UpdateAsync(int id, UpdateLocationRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
