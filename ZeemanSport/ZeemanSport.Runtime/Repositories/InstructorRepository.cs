using Npgsql;
using ZeemanSport.Core.Instructor;
using ZeemanSport.Runtime.Helpers;

namespace ZeemanSport.Runtime.Repositories
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public InstructorRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IReadOnlyCollection<Instructor>> GetAllAsync()
        {
            List<Instructor> instructors = new List<Instructor>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_instructors();", connection);
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                instructors.Add(MapInstructor(reader));

            return instructors;
        }

        public async Task<Instructor?> GetByIdAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_instructor_by_id(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", id);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapInstructor(reader);
        }

        public async Task<Instructor> SaveAsync(Instructor instructor)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_save_instructor(@p_id, @p_name, @p_photo_url, @p_is_active);", connection);

            command.Parameters.AddWithValue("p_id", instructor.Id);
            command.Parameters.AddWithValue("p_name", instructor.Name ?? string.Empty);
            command.Parameters.AddWithValue("p_photo_url", (object?)instructor.PhotoUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("p_is_active", instructor.IsActive);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Instructor could not be saved.");

            return MapInstructor(reader);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT dbo.usp_delete_instructor(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", id);

            object? result = await command.ExecuteScalarAsync();

            return result is bool deleted && deleted;
        }

        private static Instructor MapInstructor(NpgsqlDataReader reader)
        {
            Instructor instructor = new Instructor
            {
                Name = reader.GetString(reader.GetOrdinal("name")),
                PhotoUrl = DataReaderHelper.ReadNullableString(reader, "photo_url"),
                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"))
            };

            instructor.SetId(reader.GetInt32(reader.GetOrdinal("id")));

            return instructor;
        }
    }
}
