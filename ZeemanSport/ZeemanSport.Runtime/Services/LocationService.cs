using ZeemanSport.Core.Contracts.Location;
using ZeemanSport.Core.Location;

namespace ZeemanSport.Runtime.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IReadOnlyCollection<LocationResponse>> GetAllAsync()
        {
            IReadOnlyCollection<Location> locations = await _locationRepository.GetAllAsync();

            return locations.Select(mapToResponse).ToArray();
        }

        public async Task<LocationResponse> GetByIdAsync(int id)
        {
            Location? location = await _locationRepository.GetByIdAsync(id);

            if (location == null)
                return null;

            return mapToResponse(location);
        }

        public async Task<LocationResponse> CreateAsync(CreateLocationRequest request)
        {
            Location location = new Location
            {
                Name = request.Name,
                LocationType = request.LocationType,
                Capacity = request.Capacity,
                WidthInSeats = request.WidthInSeats,
                HeightInSeats = request.HeightInSeats,
                IsActive = true
            };

            Location savedLocation = await _locationRepository.SaveAsync(location);

            return mapToResponse(savedLocation);
        }

        public async Task<LocationResponse> UpdateAsync(int id, UpdateLocationRequest request)
        {
            Location? location = await _locationRepository.GetByIdAsync(id);

            if (location == null)
                return null;

            location.Name = request.Name;
            location.LocationType = request.LocationType;
            location.Capacity = request.Capacity;
            location.WidthInSeats = request.WidthInSeats;
            location.HeightInSeats = request.HeightInSeats;
            location.IsActive = request.IsActive;

            Location savedLocation = await _locationRepository.SaveAsync(location);

            return mapToResponse(savedLocation);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _locationRepository.DeleteAsync(id);
        }

        private static LocationResponse mapToResponse(Location location)
        {
            return new LocationResponse
            {
                Id = location.Id,
                Name = location.Name,
                LocationType = location.LocationType,
                Capacity = location.Capacity,
                WidthInSeats = location.WidthInSeats,
                HeightInSeats = location.HeightInSeats,
                IsActive = location.IsActive
            };
        }

    }
}
