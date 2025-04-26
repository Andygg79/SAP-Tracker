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
        public async Task<bool> CheckProfileCompleteAsync(string email)
        {
            var safeEmailId = email.Replace(".", "_").Replace("@", "_");
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return false; // Assume incomplete if can't read

            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("fields", out JsonElement fields))
            {
                if (fields.TryGetProperty("profileComplete", out JsonElement profileCompleteField))
                {
                    return profileCompleteField.GetProperty("booleanValue").GetBoolean();
                }
            }

            return false;
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
        public async Task<bool> SaveProfileAsync(string email, string firstName, string lastName, string rank, string unit, string dutyTitle)
        {
            var safeEmailId = email.Replace(".", "_").Replace("@", "_");
            var documentPath = $"projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";
            var url = $"https://firestore.googleapis.com/v1/{documentPath}?updateMask.fieldPaths=firstName&updateMask.fieldPaths=lastName&updateMask.fieldPaths=rank&updateMask.fieldPaths=unit&updateMask.fieldPaths=dutyTitle&updateMask.fieldPaths=profileComplete";

            var requestData = new
            {
                fields = new
                {
                    firstName = new { stringValue = firstName },
                    lastName = new { stringValue = lastName },
                    rank = new { stringValue = rank },
                    unit = new { stringValue = unit },
                    dutyTitle = new { stringValue = dutyTitle },
                    profileComplete = new { booleanValue = true }
                }
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(url, content);

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> AddTeamMemberAsync(string leaderEmail, string memberEmail)
        {
            var safeLeaderId = leaderEmail.Replace(".", "_").Replace("@", "_");
            var safeMemberId = memberEmail.Replace(".", "_").Replace("@", "_");

            var documentPath = $"projects/{projectId}/databases/(default)/documents/leaders/{safeLeaderId}/teamMembers/{safeMemberId}";
            var url = $"https://firestore.googleapis.com/v1/{documentPath}";

            var requestData = new
            {
                fields = new
                {
                    email = new { stringValue = memberEmail }
                }
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(url, content);

            return response.IsSuccessStatusCode;
        }
        public async Task<List<string>> GetTeamMembersAsync(string leaderEmail)
        {
            var safeLeaderId = leaderEmail.Replace(".", "_").Replace("@", "_");
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/leaders/{safeLeaderId}/teamMembers";

            var response = await httpClient.GetAsync(url);
            var result = new List<string>();

            if (!response.IsSuccessStatusCode)
                return result;

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseString);

            if (jsonDoc.RootElement.TryGetProperty("documents", out JsonElement documents))
            {
                foreach (var doc in documents.EnumerateArray())
                {
                    if (doc.TryGetProperty("fields", out var fields))
                    {
                        if (fields.TryGetProperty("email", out var emailField))
                        {
                            var email = emailField.GetProperty("stringValue").GetString();
                            if (!string.IsNullOrWhiteSpace(email))
                                result.Add(email);
                        }
                    }
                }
            }

            return result;
        }
        public async Task<(string firstName, string lastName)> GetUserProfileAsync(string email)
        {
            var safeEmailId = email.Replace(".", "_").Replace("@", "_");
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return ("Unknown", "Soldier");

            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);

            string firstName = "Unknown";
            string lastName = "Soldier";

            if (document.RootElement.TryGetProperty("fields", out JsonElement fields))
            {
                if (fields.TryGetProperty("firstName", out JsonElement fnField))
                    firstName = fnField.GetProperty("stringValue").GetString() ?? "Unknown";

                if (fields.TryGetProperty("lastName", out JsonElement lnField))
                    lastName = lnField.GetProperty("stringValue").GetString() ?? "Soldier";
            }

            return (firstName, lastName);
        }
        public async Task<bool> RemoveTeamMemberAsync(string leaderEmail, string memberEmail)
        {
            var safeLeaderId = leaderEmail.Replace(".", "_").Replace("@", "_");
            var safeMemberId = memberEmail.Replace(".", "_").Replace("@", "_");

            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/leaders/{safeLeaderId}/teamMembers/{safeMemberId}";

            var response = await httpClient.DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }
        public async Task<(string FirstName, string LastName, string Rank, string Unit, string DutyTitle)> GetFullUserProfileAsync(string email)
        {
            var safeEmailId = email.Replace(".", "_").Replace("@", "_");
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return ("Unknown", "Unknown", "Unknown", "Unknown", "Unknown");

            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);

            string firstName = "Unknown";
            string lastName = "Unknown";
            string rank = "Unknown";
            string unit = "Unknown";
            string dutyTitle = "Unknown";

            if (document.RootElement.TryGetProperty("fields", out JsonElement fields))
            {
                if (fields.TryGetProperty("firstName", out var firstNameField))
                    firstName = firstNameField.GetProperty("stringValue").GetString() ?? "Unknown";

                if (fields.TryGetProperty("lastName", out var lastNameField))
                    lastName = lastNameField.GetProperty("stringValue").GetString() ?? "Unknown";

                if (fields.TryGetProperty("rank", out var rankField))
                    rank = rankField.GetProperty("stringValue").GetString() ?? "Unknown";

                if (fields.TryGetProperty("unit", out var unitField))
                    unit = unitField.GetProperty("stringValue").GetString() ?? "Unknown";

                if (fields.TryGetProperty("dutyTitle", out var dutyTitleField))
                    dutyTitle = dutyTitleField.GetProperty("stringValue").GetString() ?? "Unknown";
            }

            return (firstName, lastName, rank, unit, dutyTitle);
        }

    }
}
