using Npgsql;
using System.Text.Json.Serialization;
using ZeemanSport.Core.User;
using ZeemanSport.Runtime.Repositories;
using ZeemanSport.Runtime.Services.UserService;

namespace ZeemanSport.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
