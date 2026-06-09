using Npgsql;
using ZeemanSport.Core.Location;

namespace ZeemanSport.Runtime.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public LocationRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IReadOnlyCollection<Location>> GetAllAsync()
        {
            List<Location> locations = new List<Location>();
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_locations();", connection);
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                locations.Add(MapLocation(reader));

            return locations;
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_location_by_id(@p_id);", connection);
            command.Parameters.AddWithValue("p_id", id);
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapLocation(reader);
        }

        public async Task<Location> SaveAsync(Location location)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_save_location(@p_id, @p_name, @p_location_type, @p_capacity, @p_width_in_seats, @p_height_in_seats, @p_is_active);", connection);
            command.Parameters.AddWithValue("p_id", location.Id);
            command.Parameters.AddWithValue("p_name", location.Name ?? string.Empty);
            command.Parameters.AddWithValue("p_location_type", (int)location.LocationType);
            command.Parameters.AddWithValue("p_capacity", location.Capacity);
            command.Parameters.AddWithValue("p_width_in_seats", location.WidthInSeats);
            command.Parameters.AddWithValue("p_height_in_seats", location.HeightInSeats);
            command.Parameters.AddWithValue("p_is_active", location.IsActive);
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Location could not be saved.");

            return MapLocation(reader);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT dbo.usp_delete_location(@p_id);", connection);
            command.Parameters.AddWithValue("p_id", id);
            object? result = await command.ExecuteScalarAsync();

            return result is bool deleted && deleted;
        }

        private static Location MapLocation(NpgsqlDataReader reader)
        {
            Location location = new Location
            {
                Name = reader.GetString(reader.GetOrdinal("name")),
                LocationType = (LocationType)reader.GetInt32(reader.GetOrdinal("location_type")),
                Capacity = reader.GetInt32(reader.GetOrdinal("capacity")),
                WidthInSeats = reader.GetInt32(reader.GetOrdinal("width_in_seats")),
                HeightInSeats = reader.GetInt32(reader.GetOrdinal("height_in_seats")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"))
            };

            location.SetId(reader.GetInt32(reader.GetOrdinal("id")));

            return location;
        }
    }
}
