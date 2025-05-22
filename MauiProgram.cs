using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.Configuration.Json;
using System.Text.Json;

namespace SAPTracker
{
    public static class MauiProgram
    {
        // Global app config access
        public static IConfiguration? AppConfiguration { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Load configuration from appsettings.json at runtime
            AppConfiguration = LoadAppSettings();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static IConfiguration LoadAppSettings()
        {
            const string fileName = "appsettings.json";

            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync(fileName).Result;
                using var reader = new StreamReader(stream);

                string jsonContent = reader.ReadToEnd();

                // Deserialize and convert to Dictionary<string, string?>
                var rawDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                var stringDict = rawDict?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString()
                );

                var config = new ConfigurationBuilder()
                    .AddInMemoryCollection(stringDict!)
                    .Build();

                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONFIG ERROR] Failed to load {fileName}: {ex.Message}");
                return new ConfigurationBuilder().Build(); // fallback empty config
            }
        }
    }
}
