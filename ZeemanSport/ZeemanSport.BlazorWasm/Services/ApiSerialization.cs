using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZeemanSport.BlazorWasm.Services
{
    /// <summary>
    /// Shared JSON options matching the API, which serialises enums as strings.
    /// </summary>
    public static class ApiSerialization
    {
        public static readonly JsonSerializerOptions Options = CreateOptions();

        private static JsonSerializerOptions CreateOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }
    }
}
