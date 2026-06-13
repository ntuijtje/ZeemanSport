using Npgsql;
using ZeemanSport.Core.Contracts.Session;
using ZeemanSport.Core.Location;
using ZeemanSport.Core.Session;
using ZeemanSport.Runtime.Helpers;

namespace ZeemanSport.Runtime.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public SessionRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IReadOnlyCollection<SessionResponse>> GetScheduleAsync(DateTime from, DateTime to)
        {
            List<SessionResponse> sessions = new List<SessionResponse>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_schedule(@p_from, @p_to);", connection);

            command.Parameters.AddWithValue("p_from", from);
            command.Parameters.AddWithValue("p_to", to);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                sessions.Add(MapSession(reader));

            return sessions;
        }

        public async Task<SessionResponse?> GetByIdAsync(int id)
        {
            SessionResponse? session;

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_session_by_id(@p_id);", connection))
            {
                command.Parameters.AddWithValue("p_id", id);

                await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return null;

                session = MapSession(reader);
            }

            if (session.Seats != null)
                await MarkReservedSeatsAsync(connection, session);

            return session;
        }

        public async Task<IReadOnlyCollection<SessionResponse>> GetInstructorSessionsAsync(int instructorId, DateTime from, DateTime to)
        {
            List<SessionResponse> sessions = new List<SessionResponse>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_instructor_sessions(@p_instructor_id, @p_from, @p_to);", connection);

            command.Parameters.AddWithValue("p_instructor_id", instructorId);
            command.Parameters.AddWithValue("p_from", from);
            command.Parameters.AddWithValue("p_to", to);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                sessions.Add(MapSession(reader));

            return sessions;
        }

        public async Task<SessionResponse> SaveAsync(Session session)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_save_session(@p_id, @p_workout_id, @p_instructor_id, @p_location_id, @p_start_time, @p_duration_minutes, @p_status);", connection);

            command.Parameters.AddWithValue("p_id", session.Id);
            command.Parameters.AddWithValue("p_workout_id", session.WorkoutId);
            command.Parameters.AddWithValue("p_instructor_id", session.InstructorId > 0 ? session.InstructorId : (object)DBNull.Value);
            command.Parameters.AddWithValue("p_location_id", session.LocationId);
            command.Parameters.AddWithValue("p_start_time", session.StartTime);
            command.Parameters.AddWithValue("p_duration_minutes", session.DurationMinutes);
            command.Parameters.AddWithValue("p_status", (int)session.Status);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Session could not be saved.");

            return MapSession(reader);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT dbo.usp_delete_session(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", id);

            object? result = await command.ExecuteScalarAsync();

            return result is bool deleted && deleted;
        }

        private static SessionResponse MapSession(NpgsqlDataReader reader)
        {
            LocationType locationType = (LocationType)reader.GetInt32(reader.GetOrdinal("location_type"));

            SessionResponse session = new SessionResponse
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                WorkoutId = reader.GetInt32(reader.GetOrdinal("workout_id")),
                WorkoutName = DataReaderHelper.ReadNullableString(reader, "workout_name"),
                InstructorId = DataReaderHelper.ReadNullableInt32(reader, "instructor_id"),
                InstructorName = DataReaderHelper.ReadNullableString(reader, "instructor_name"),
                LocationId = reader.GetInt32(reader.GetOrdinal("location_id")),
                LocationName = DataReaderHelper.ReadNullableString(reader, "location_name"),
                LocationType = locationType,
                StartTime = reader.GetDateTime(reader.GetOrdinal("start_time")),
                DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                Status = (SessionStatus)reader.GetInt32(reader.GetOrdinal("status")),
                Capacity = reader.GetInt32(reader.GetOrdinal("capacity")),
                ReservedCount = reader.GetInt32(reader.GetOrdinal("reserved_count")),
                WaitlistCount = reader.GetInt32(reader.GetOrdinal("waitlist_count"))
            };

            if (locationType == LocationType.SpinningRoom)
                session.Seats = BuildSeatLayout(
                    session.LocationId,
                    reader.GetInt32(reader.GetOrdinal("width_in_seats")),
                    reader.GetInt32(reader.GetOrdinal("height_in_seats")));

            return session;
        }

        private static List<Seat> BuildSeatLayout(int locationId, int width, int height)
        {
            List<Seat> seats = new List<Seat>(width * height);

            for (int row = 0; row < height; row++)
                for (int column = 0; column < width; column++)
                    seats.Add(new Seat
                    {
                        LocationId = locationId,
                        RowIndex = row,
                        ColumnIndex = column,
                        IsReserved = false,
                        IsSelected = false
                    });

            return seats;
        }

        private static async Task MarkReservedSeatsAsync(NpgsqlConnection connection, SessionResponse session)
        {
            HashSet<(int Row, int Column)> reserved = new HashSet<(int, int)>();

            await using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_reserved_seats(@p_session_id);", connection))
            {
                command.Parameters.AddWithValue("p_session_id", session.Id);

                await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                    reserved.Add((
                        reader.GetInt32(reader.GetOrdinal("seat_row")),
                        reader.GetInt32(reader.GetOrdinal("seat_column"))));
            }

            if (session.Seats == null)
                return;

            foreach (Seat seat in session.Seats)
                seat.IsReserved = reserved.Contains((seat.RowIndex, seat.ColumnIndex));
        }
    }
}
