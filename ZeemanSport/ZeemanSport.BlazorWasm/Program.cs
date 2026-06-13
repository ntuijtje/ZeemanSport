using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ZeemanSport.BlazorWasm.Services;

namespace ZeemanSport.BlazorWasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // The API base address; configurable via wwwroot/appsettings.json, defaulting to the
            // API's local HTTP endpoint.
            string apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5236/";

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

            builder.Services.AddScoped<WorkoutApiClient>();
            builder.Services.AddScoped<InstructorApiClient>();
            builder.Services.AddScoped<LocationApiClient>();
            builder.Services.AddScoped<SessionApiClient>();
            builder.Services.AddScoped<SubscriptionApiClient>();
            builder.Services.AddScoped<UserApiClient>();

            await builder.Build().RunAsync();
        }
    }
}
