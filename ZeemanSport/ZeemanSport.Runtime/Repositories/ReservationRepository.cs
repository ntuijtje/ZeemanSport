using Npgsql;
using ZeemanSport.Core.Contracts.Reservation;
using ZeemanSport.Core.Location;
using ZeemanSport.Core.Reservation;
using ZeemanSport.Runtime.Helpers;

namespace ZeemanSport.Runtime.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public ReservationRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IReadOnlyCollection<ReservationResponse>> GetByUserAsync(int userId, DateTime from, DateTime to)
        {
            List<ReservationResponse> reservations = new List<ReservationResponse>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_reservations_by_user(@p_user_id, @p_from, @p_to);", connection);

            command.Parameters.AddWithValue("p_user_id", userId);
            command.Parameters.AddWithValue("p_from", from);
            command.Parameters.AddWithValue("p_to", to);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                reservations.Add(MapReservation(reader));

            return reservations;
        }

        public async Task<ReservationResponse?> GetActiveAsync(int sessionId, int userId)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_active_reservation(@p_session_id, @p_user_id);", connection);

            command.Parameters.AddWithValue("p_session_id", sessionId);
            command.Parameters.AddWithValue("p_user_id", userId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapReservation(reader);
        }

        public async Task<ReservationResponse?> GetByIdAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_reservation_by_id(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", id);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapReservation(reader);
        }

        public async Task<int> CountUserWeekReservationsAsync(int userId, DateTime weekStart, DateTime weekEnd)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT dbo.usp_count_user_week_reservations(@p_user_id, @p_week_start, @p_week_end);", connection);

            command.Parameters.AddWithValue("p_user_id", userId);
            command.Parameters.AddWithValue("p_week_start", weekStart);
            command.Parameters.AddWithValue("p_week_end", weekEnd);

            object? result = await command.ExecuteScalarAsync();

            return result is int count ? count : 0;
        }

        public async Task<bool> IsSeatTakenAsync(int sessionId, int seatRow, int seatColumn)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT dbo.usp_is_seat_taken(@p_session_id, @p_seat_row, @p_seat_column);", connection);

            command.Parameters.AddWithValue("p_session_id", sessionId);
            command.Parameters.AddWithValue("p_seat_row", seatRow);
            command.Parameters.AddWithValue("p_seat_column", seatColumn);

            object? result = await command.ExecuteScalarAsync();

            return result is bool taken && taken;
        }

        public async Task<IReadOnlyCollection<ParticipantResponse>> GetParticipantsAsync(int sessionId)
        {
            List<ParticipantResponse> participants = new List<ParticipantResponse>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_session_participants(@p_session_id);", connection);

            command.Parameters.AddWithValue("p_session_id", sessionId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                participants.Add(new ParticipantResponse
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                    UserName = reader.GetString(reader.GetOrdinal("user_name")),
                    SeatRow = DataReaderHelper.ReadNullableInt32(reader, "seat_row"),
                    SeatColumn = DataReaderHelper.ReadNullableInt32(reader, "seat_column"),
                    IsCheckedIn = reader.GetBoolean(reader.GetOrdinal("is_checked_in"))
                });

            return participants;
        }

        public async Task<ReservationResponse> SaveAsync(Reservation reservation)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_save_reservation(@p_id, @p_session_id, @p_user_id, @p_status, @p_seat_row, @p_seat_column, @p_reserved_at, @p_checked_in_at, @p_check_in_method);", connection);

            command.Parameters.AddWithValue("p_id", reservation.Id);
            command.Parameters.AddWithValue("p_session_id", reservation.SessionId);
            command.Parameters.AddWithValue("p_user_id", reservation.UserId);
            command.Parameters.AddWithValue("p_status", (int)reservation.Status);
            command.Parameters.AddWithValue("p_seat_row", (object?)reservation.SeatRow ?? DBNull.Value);
            command.Parameters.AddWithValue("p_seat_column", (object?)reservation.SeatColumn ?? DBNull.Value);
            command.Parameters.AddWithValue("p_reserved_at", reservation.ReservedAt);
            command.Parameters.AddWithValue("p_checked_in_at", (object?)reservation.CheckedInAt ?? DBNull.Value);
            command.Parameters.AddWithValue("p_check_in_method", reservation.CheckInMethod.HasValue ? (int)reservation.CheckInMethod.Value : (object)DBNull.Value);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Reservation could not be saved.");

            return MapReservation(reader);
        }

        public async Task<WaitlistEntryResponse?> GetWaitlistEntryAsync(int sessionId, int userId)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_waitlist_entry(@p_session_id, @p_user_id);", connection);

            command.Parameters.AddWithValue("p_session_id", sessionId);
            command.Parameters.AddWithValue("p_user_id", userId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapWaitlistEntry(reader);
        }

        public async Task<IReadOnlyCollection<WaitlistEntryResponse>> GetWaitlistAsync(int sessionId)
        {
            List<WaitlistEntryResponse> entries = new List<WaitlistEntryResponse>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_waitlist(@p_session_id);", connection);

            command.Parameters.AddWithValue("p_session_id", sessionId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                entries.Add(MapWaitlistEntry(reader));

            return entries;
        }

        public async Task<WaitlistEntryResponse> AddToWaitlistAsync(int sessionId, int userId)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_add_waitlist(@p_session_id, @p_user_id);", connection);

            command.Parameters.AddWithValue("p_session_id", sessionId);
            command.Parameters.AddWithValue("p_user_id", userId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Waitlist entry could not be saved.");

            return MapWaitlistEntry(reader);
        }

        public async Task<bool> RemoveFromWaitlistAsync(int sessionId, int userId)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT dbo.usp_remove_waitlist(@p_session_id, @p_user_id);", connection);

            command.Parameters.AddWithValue("p_session_id", sessionId);
            command.Parameters.AddWithValue("p_user_id", userId);

            object? result = await command.ExecuteScalarAsync();

            return result is bool removed && removed;
        }

        private static ReservationResponse MapReservation(NpgsqlDataReader reader)
        {
            int? checkInMethod = DataReaderHelper.ReadNullableInt32(reader, "check_in_method");

            return new ReservationResponse
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                SessionId = reader.GetInt32(reader.GetOrdinal("session_id")),
                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                Status = (ReservationStatus)reader.GetInt32(reader.GetOrdinal("status")),
                SeatRow = DataReaderHelper.ReadNullableInt32(reader, "seat_row"),
                SeatColumn = DataReaderHelper.ReadNullableInt32(reader, "seat_column"),
                ReservedAt = reader.GetDateTime(reader.GetOrdinal("reserved_at")),
                CheckedInAt = DataReaderHelper.ReadNullableDateTime(reader, "checked_in_at"),
                CheckInMethod = checkInMethod.HasValue ? (CheckInMethod)checkInMethod.Value : null,
                WorkoutName = DataReaderHelper.ReadNullableString(reader, "workout_name"),
                StartTime = reader.GetDateTime(reader.GetOrdinal("start_time")),
                DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                LocationName = DataReaderHelper.ReadNullableString(reader, "location_name"),
                LocationType = (LocationType)reader.GetInt32(reader.GetOrdinal("location_type"))
            };
        }

        private static WaitlistEntryResponse MapWaitlistEntry(NpgsqlDataReader reader)
        {
            return new WaitlistEntryResponse
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                SessionId = reader.GetInt32(reader.GetOrdinal("session_id")),
                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                Position = reader.GetInt32(reader.GetOrdinal("queue_position")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }
    }
}
