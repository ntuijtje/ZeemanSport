using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Workout;
using ZeemanSport.Runtime.Helpers;

namespace ZeemanSport.Runtime.Repositories
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public WorkoutRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IReadOnlyCollection<Workout>> GetAllAsync()
        {
            List<Workout> workouts = new List<Workout>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_workouts();", connection);
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                workouts.Add(MapWorkout(reader));

            return workouts;
        }

        public async Task<Workout?> GetByIdAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_workout_by_id(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", id);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapWorkout(reader);
        }

        public async Task<Workout> SaveAsync(Workout workout)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_save_workout(@p_id, @p_name, @p_description, @p_is_active);", connection);

            command.Parameters.AddWithValue("p_id", workout.Id);
            command.Parameters.AddWithValue("p_name", workout.Name ?? string.Empty);
            command.Parameters.AddWithValue("p_description", (object?)workout.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("p_is_active", workout.IsActive);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Workout could not be saved.");

            return MapWorkout(reader);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT dbo.usp_delete_workout(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", id);

            object? result = await command.ExecuteScalarAsync();

            return result is bool deleted && deleted;
        }

        private static Workout MapWorkout(NpgsqlDataReader reader)
        {
            Workout workout = new Workout
            {
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = DataReaderHelper.ReadNullableString(reader, "description"),
                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"))
            };

            workout.SetId(reader.GetInt32(reader.GetOrdinal("id")));

            return workout;
        }
    }
}
