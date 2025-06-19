using System.Text;
using System.Text.Json;

namespace SAPTracker.Services
{
    public class FirebaseAuthService
    {
        private readonly string apiKey;
        private static readonly HttpClient httpClient = new();

        public FirebaseAuthService()
        {
            var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if (!File.Exists(settingsPath))
                throw new Exception($"appsettings.json not found at path: {settingsPath}");

            var json = File.ReadAllText(settingsPath);
            var settings = JsonSerializer.Deserialize<Models.AppSettings>(json);

            apiKey = settings?.Firebase.ApiKey ?? throw new Exception("Firebase API Key missing from appsettings.json.");
        }

        private string ExtractErrorMessage(string responseString)
        {
            try
            {
                var error = JsonSerializer.Deserialize<JsonElement>(responseString);
                return error.GetProperty("error").GetProperty("message").GetString() ?? "Unknown error.";
            }
            catch
            {
                return "Unexpected error format.";
            }
        }

        private async Task<(bool Success, string Message)> PostFirebaseRequestAsync(string endpoint, object requestData)
        {
            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/{endpoint}?key={apiKey}",
                content);

            var responseString = await response.Content.ReadAsStringAsync();

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Firebase Response ({endpoint}): {responseString}");
#endif

            return response.IsSuccessStatusCode
                ? (true, "Success")
                : (false, ExtractErrorMessage(responseString));
        }

        public Task<(bool Success, string Message)> RegisterAsync(string email, string password)
        {
            return PostFirebaseRequestAsync("accounts:signUp", new
            {
                email,
                password,
                returnSecureToken = true
            });
        }

        public Task<(bool Success, string Message)> LoginAsync(string email, string password)
        {
            return PostFirebaseRequestAsync("accounts:signInWithPassword", new
            {
                email,
                password,
                returnSecureToken = true
            });
        }

        public Task<(bool Success, string Message)> SendPasswordResetEmailAsync(string email)
        {
            return PostFirebaseRequestAsync("accounts:sendOobCode", new
            {
                requestType = "PASSWORD_RESET",
                email
            });
        }

        public Task<(bool Success, string Message)> LoginWithGoogleAsync(string idToken)
        {
            return PostFirebaseRequestAsync("accounts:signInWithIdp", new
            {
                postBody = $"id_token={idToken}&providerId=google.com",
                requestUri = "http://localhost",
                returnSecureToken = true
            });
        }
    }
}
