using Npgsql;
using System.Text.Json.Serialization;
using ZeemanSport.Core.Instructor;
using ZeemanSport.Core.Location;
using ZeemanSport.Core.Reservation;
using ZeemanSport.Core.Session;
using ZeemanSport.Core.Subscription;
using ZeemanSport.Core.User;
using ZeemanSport.Core.Workout;
using ZeemanSport.Runtime.Repositories;
using ZeemanSport.Runtime.Services;

namespace ZeemanSport.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            string? connectionString = builder.Configuration.GetConnectionString("ZeemanSportDatabase");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing connection string: ZeemanSportDatabase");

            builder.Services.AddSingleton<NpgsqlDataSource>(serviceProvider =>
            {
                NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                return dataSourceBuilder.Build();
            });

            //Repositories
            builder.Services.AddSingleton<IInstructorRepository, InstructorRepository>();
            builder.Services.AddSingleton<ILocationRepository, LocationRepository>();
            builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
            builder.Services.AddSingleton<ISessionRepository, SessionRepository>();
            builder.Services.AddSingleton<ISubscriptionRepository, SubscriptionRepository>();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<IWorkoutRepository, WorkoutRepository>();

            //Services
            builder.Services.AddScoped<IInstructorService, InstructorService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IWorkoutService, WorkoutService>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
