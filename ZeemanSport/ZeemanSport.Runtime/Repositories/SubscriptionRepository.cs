using Npgsql;
using ZeemanSport.Core.Contracts.Subscription;
using ZeemanSport.Core.Subscription;
using ZeemanSport.Runtime.Helpers;

namespace ZeemanSport.Runtime.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public SubscriptionRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IReadOnlyCollection<SubscriptionPlanResponse>> GetPlansAsync()
        {
            List<SubscriptionPlanResponse> plans = new List<SubscriptionPlanResponse>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_subscription_plans();", connection);
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                plans.Add(MapPlan(reader));

            return plans;
        }

        public async Task<SubscriptionPlanResponse?> GetPlanByIdAsync(int planId)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_subscription_plan_by_id(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", planId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapPlan(reader);
        }

        public async Task<IReadOnlyCollection<SubscriptionResponse>> GetByUserAsync(int userId)
        {
            List<SubscriptionResponse> subscriptions = new List<SubscriptionResponse>();

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_subscriptions_by_user(@p_user_id);", connection);

            command.Parameters.AddWithValue("p_user_id", userId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                subscriptions.Add(MapSubscription(reader));

            return subscriptions;
        }

        public async Task<SubscriptionResponse?> GetActiveByUserAsync(int userId)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_active_subscription(@p_user_id);", connection);

            command.Parameters.AddWithValue("p_user_id", userId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapSubscription(reader);
        }

        public async Task<SubscriptionResponse?> GetByIdAsync(int id)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_get_subscription_by_id(@p_id);", connection);

            command.Parameters.AddWithValue("p_id", id);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return MapSubscription(reader);
        }

        public async Task<SubscriptionResponse> SaveAsync(Subscription subscription)
        {
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            await using NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM dbo.usp_save_subscription(@p_id, @p_user_id, @p_plan_id, @p_start_date, @p_end_date, @p_status);", connection);

            command.Parameters.AddWithValue("p_id", subscription.Id);
            command.Parameters.AddWithValue("p_user_id", subscription.UserId);
            command.Parameters.AddWithValue("p_plan_id", subscription.PlanId);
            command.Parameters.AddWithValue("p_start_date", subscription.StartDate);
            command.Parameters.AddWithValue("p_end_date", subscription.EndDate);
            command.Parameters.AddWithValue("p_status", (int)subscription.Status);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Subscription could not be saved.");

            return MapSubscription(reader);
        }

        private static SubscriptionPlanResponse MapPlan(NpgsqlDataReader reader)
        {
            return new SubscriptionPlanResponse
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                AccessTier = (AccessTier)reader.GetInt32(reader.GetOrdinal("access_tier")),
                BillingInterval = (BillingInterval)reader.GetInt32(reader.GetOrdinal("billing_interval")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"))
            };
        }

        private static SubscriptionResponse MapSubscription(NpgsqlDataReader reader)
        {
            return new SubscriptionResponse
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                PlanId = reader.GetInt32(reader.GetOrdinal("plan_id")),
                PlanName = DataReaderHelper.ReadNullableString(reader, "plan_name"),
                AccessTier = (AccessTier)reader.GetInt32(reader.GetOrdinal("access_tier")),
                BillingInterval = (BillingInterval)reader.GetInt32(reader.GetOrdinal("billing_interval")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("end_date")),
                Status = (SubscriptionStatus)reader.GetInt32(reader.GetOrdinal("status"))
            };
        }
    }
}
