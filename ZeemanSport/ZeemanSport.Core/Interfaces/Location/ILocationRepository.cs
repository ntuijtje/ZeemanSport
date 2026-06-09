using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Location
{
    public interface ILocationRepository
    {
        Task<IReadOnlyCollection<Location>> GetAllAsync();
        Task<Location> GetByIdAsync(int id);
        Task<Location> SaveAsync(Location location);
        Task<bool> DeleteAsync(int id);
    }
}
