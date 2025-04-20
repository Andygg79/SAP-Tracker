using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SAPTracker.Models;


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
        public async Task<bool> SaveMetricsAsync(string email, Dictionary<string, MetricEntry> metrics)
        {
            var safeEmailId = email.Replace(".", "_").Replace("@", "_");
            bool allSuccessful = true;

            foreach (var metric in metrics)
            {
                var documentPath = $"projects/{projectId}/databases/(default)/documents/users/{safeEmailId}/metrics/{metric.Key}";
                var url = $"https://firestore.googleapis.com/v1/{documentPath}";

                var requestData = new
                {
                    fields = new
                    {
                        date = new { stringValue = metric.Value.LastUpdatedDate.ToString("yyyy-MM-dd") },
                        statusColor = new { stringValue = metric.Value.StatusColor }
                    }
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PatchAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    allSuccessful = false;
            }

            return allSuccessful;
        }
        public async Task<Dictionary<string, MetricEntry>> LoadMetricsAsync(string email)
        {
            var safeEmailId = email.Replace(".", "_").Replace("@", "_");
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{safeEmailId}/metrics";

            var response = await httpClient.GetAsync(url);
            var result = new Dictionary<string, MetricEntry>();

            if (!response.IsSuccessStatusCode)
                return result;

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseString);

            if (jsonDoc.RootElement.TryGetProperty("documents", out JsonElement documents))
            {
                foreach (var doc in documents.EnumerateArray())
                {
                    var nameSegments = doc.GetProperty("name").GetString()?.Split('/');
                    var metricName = nameSegments?.LastOrDefault() ?? "";

                    if (doc.TryGetProperty("fields", out var fields))
                    {
                        var date = fields.GetProperty("date").GetProperty("stringValue").GetString();
                        var statusColor = fields.GetProperty("statusColor").GetProperty("stringValue").GetString();

                        if (DateTime.TryParse(date, out var parsedDate))
                        {
                            result[metricName] = new MetricEntry
                            {
                                MetricName = metricName,
                                LastUpdatedDate = parsedDate,
                                StatusColor = statusColor ?? "Gray"
                            };
                        }
                    }
                }
            }

            return result;
        }


    }
}
