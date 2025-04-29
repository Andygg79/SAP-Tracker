using System.Text;
using System.Text.Json;

namespace SAPTracker.Services
{
    public class FirebaseAuthService
    {
        private readonly string apiKey;

        public FirebaseAuthService()
        {
            var settingsPath = Path.Combine(FileSystem.AppDataDirectory, "appsettings.json");

            if (!File.Exists(settingsPath))
                throw new Exception("appsettings.json not found in app directory.");

            var json = File.ReadAllText(settingsPath);
            var settings = JsonSerializer.Deserialize<Models.AppSettings>(json);

            apiKey = settings?.FirebaseApiKey ?? throw new Exception("Firebase API Key missing from appsettings.json.");
        }

        private readonly HttpClient httpClient = new();

        public async Task<(bool Success, string Message)> RegisterAsync(string email, string password)
        {
            var requestData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}",
                content
            );

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return (true, "Account created successfully!");
            }
            else
            {
                var error = JsonSerializer.Deserialize<JsonElement>(responseString);
                var message = error.GetProperty("error").GetProperty("message").GetString();
                return (false, message ?? "Failed to create account.");
            }
        }

        public async Task<(bool Success, string Message)> LoginAsync(string email, string password)
        {
            var requestData = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}",
                content
            );

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return (true, "Login successful!");
            }
            else
            {
                var error = JsonSerializer.Deserialize<JsonElement>(responseString);
                var message = error.GetProperty("error").GetProperty("message").GetString();
                return (false, message ?? "Failed to login.");
            }
        }

        public async Task<(bool Success, string Message)> SendPasswordResetEmailAsync(string email)
        {
            var requestData = new
            {
                requestType = "PASSWORD_RESET",
                email
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}",
                content
            );

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return (true, "Password reset email sent.");
            }
            else
            {
                var error = JsonSerializer.Deserialize<JsonElement>(responseString);
                var message = error.GetProperty("error").GetProperty("message").GetString();
                return (false, message ?? "Failed to send reset email.");
            }
        }

        // ⭐ ADD THIS GOOGLE LOGIN METHOD:
        public async Task<(bool Success, string Message)> LoginWithGoogleAsync(string idToken)
        {
            var requestData = new
            {
                postBody = $"id_token={idToken}&providerId=google.com",
                requestUri = "http://localhost",
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={apiKey}",
                content
            );

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return (true, "Google login successful!");
            }
            else
            {
                var error = JsonSerializer.Deserialize<JsonElement>(responseString);
                var message = error.GetProperty("error").GetProperty("message").GetString();
                return (false, message ?? "Failed to login with Google.");
            }
        }

    }
}
