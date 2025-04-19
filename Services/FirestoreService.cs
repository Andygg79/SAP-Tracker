using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SAPTracker.Services
{
    public class FirestoreService
    {
        private readonly string projectId = "sapt-24jk2y"; // <- Update this line
        private readonly HttpClient httpClient = new();

        public async Task<bool> SaveUserProfileAsync(string email)
        {
            var safeEmailId = email.Replace(".", "_").Replace("@", "_");
            var documentPath = $"projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";
            var url = $"https://firestore.googleapis.com/v1/{documentPath}";

            var requestData = new
            {
                fields = new
                {
                    email = new { stringValue = email }
                    // You can add more profile fields later (like rank, phone, etc.)
                }
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(url, content);

            return response.IsSuccessStatusCode;
        }
    }
}
