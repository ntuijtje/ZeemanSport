using Npgsql;
using ZeemanSport.Core.User;
using ZeemanSport.Runtime.Helpers;

namespace ZeemanSport.Runtime.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public UserRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IReadOnlyCollection<User>> GetAllAsync()
        {
            List<User> users = new List<User>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_users();", connection);
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                users.Add(MapUser(reader));

            return users;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_user_by_id(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", id);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) 
                return null;

            return MapUser(reader);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_user_by_user_name(@p_user_name);", connection);

            command.Parameters.AddWithValue("p_user_name", userName);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) 
                return null;

            return MapUser(reader);
        }

        public async Task<User> CreateAsync(User user, string password)
        {
            string passwordHash = PasswordHelper.HashPassword(password);

            return await SaveAsync(user, passwordHash);
        }

        public async Task<User> UpdateAsync(User user)
        {
            return await SaveAsync(user, null);
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_user_password_hash(@p_user_id);", connection);

            command.Parameters.AddWithValue("p_user_id", userId);

            object? result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value) 
                return false;

            string passwordHash = (string)result;

            return PasswordHelper.VerifyPassword(password, passwordHash);
        }

        private async Task<User> SaveAsync(User user, string? passwordHash)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_save_user(@p_id, @p_user_name, @p_first_name, @p_last_name, @p_user_role, @p_password_hash);", connection);

            command.Parameters.AddWithValue("p_id", user.Id);
            command.Parameters.AddWithValue("p_user_name", user.UserName ?? string.Empty);
            command.Parameters.AddWithValue("p_first_name", (object?)user.FirstName ?? DBNull.Value);
            command.Parameters.AddWithValue("p_last_name", (object?)user.LastName ?? DBNull.Value);
            command.Parameters.AddWithValue("p_user_role", (int)user.UserRole);
            command.Parameters.AddWithValue("p_password_hash", (object?)passwordHash ?? DBNull.Value);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) 
                throw new InvalidOperationException("User could not be saved.");

            return MapUser(reader);
        }

        private static User MapUser(NpgsqlDataReader reader)
        {
            User user = new User
            {
                UserName = reader.GetString(reader.GetOrdinal("user_name")),
                FirstName = DataReaderHelper.ReadNullableString(reader, "first_name"),
                LastName = DataReaderHelper.ReadNullableString(reader, "last_name"),
                UserRole = (UserRole)reader.GetInt32(reader.GetOrdinal("user_role"))
            };

            user.SetId(reader.GetInt32(reader.GetOrdinal("id")));

            return user;
        }
    }
}
