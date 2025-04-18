using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SAPTracker.Services
{
    public class FirebaseAuthService
    {
        private readonly string apiKey = "YOUR_FIREBASE_WEB_API_KEY";
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
    }
}
