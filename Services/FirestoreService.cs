using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SAPTracker.Models;
using SAPTracker;
using Microsoft.Maui.Authentication;
using System.IO;
#if ANDROID
using Google.Apis.Auth.OAuth2;
#endif






namespace SAPTracker.Services
{
    
    public class FirestoreService
    {
        private const string BaseFirestoreUrl = "https://firestore.googleapis.com/v1/projects/com.saptracker.app/databases/(default)/documents";
        private readonly string projectId = "saptracker-1979"; // Your Firestore Project ID
        private readonly HttpClient httpClient = new();


        // Save user profile (branch info)
        public async Task<bool> SaveUserProfileAsync(string email, string branch, string username)
        {
            var safeEmailId = SafeEmail(email);
            var documentPath = $"projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";
            var url = $"https://firestore.googleapis.com/v1/{documentPath}";

            var requestData = new
            {
                fields = new
                {
                    Email = new { stringValue = email },
                    Branch = new { stringValue = branch },
                    Username = new { stringValue = username },
                    ProfileComplete = new { booleanValue = false }
                }
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(url, content);
            return response.IsSuccessStatusCode;
        }
        public async Task<string> GetUsernameAsync(string email)
        {
            var safeEmailId = SafeEmail(email);
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return "Unknown";

            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("fields", out var fields))
            {
                if (fields.TryGetProperty("Username", out var usernameField))
                {
                    return usernameField.GetProperty("stringValue").GetString() ?? "Unknown";
                }
            }

            return "Unknown";
        }




        // Save branch only
        public async Task<bool> SaveBranchAsync(string email, string branch)
        {
            var safeEmailId = SafeEmail(email);
            var documentPath = $"projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";
            var url = $"https://firestore.googleapis.com/v1/{documentPath}?updateMask.fieldPaths=branch";

            var requestData = new
            {
                fields = new
                {
                    branch = new { stringValue = branch }
                }
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        // Check if user profile is complete
        public async Task<bool> CheckProfileCompleteAsync(string email)
        {
            var safeEmailId = SafeEmail(email);
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return false;

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

        // Save metrics for a user
        public async Task<bool> SaveMetricsAsync(string email, List<MetricEntry> metrics)
        {
            var safeEmailId = SafeEmail(email);
            bool allSuccessful = true;

            foreach (var metric in metrics)
            {
                var documentPath = $"projects/{projectId}/databases/(default)/documents/users/{safeEmailId}/metrics/{metric.MetricName}";
                var url = $"https://firestore.googleapis.com/v1/{documentPath}";

                var requestData = new
                {
                    fields = new
                    {
                        date = new { stringValue = metric.LastUpdatedDate.ToString("yyyy-MM-dd") },
                        statusColor = new { stringValue = metric.StatusColor }
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

        // Load metrics for a user
        public async Task<Dictionary<string, MetricEntry>> LoadMetricsAsync(string email)
        {
            var safeEmailId = SafeEmail(email);
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

        // Add a team member under a leader
        public async Task<bool> AddTeamMemberAsync(string leaderEmail, string memberEmail)
        {
            var safeLeaderId = SafeEmail(leaderEmail);
            var safeMemberId = SafeEmail(memberEmail);

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

        // Get list of team members
        public async Task<List<string>> GetTeamMembersAsync(string leaderEmail)
        {
            var safeLeaderId = SafeEmail(leaderEmail);
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

        // Remove a team member
        public async Task<bool> RemoveTeamMemberAsync(string leaderEmail, string memberEmail)
        {
            var safeLeaderId = SafeEmail(leaderEmail);
            var safeMemberId = SafeEmail(memberEmail);

            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/leaders/{safeLeaderId}/teamMembers/{safeMemberId}";

            var response = await httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        // Get user profile (FirstName, LastName)
        public async Task<(string FirstName, string LastName)> GetUserProfileAsync(string email)
        {
            var safeEmailId = SafeEmail(email);
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return ("Unknown", "Unknown");

            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);

            string firstName = "Unknown";
            string lastName = "Unknown";

            if (document.RootElement.TryGetProperty("fields", out JsonElement fields))
            {
                if (fields.TryGetProperty("firstName", out var firstNameField))
                    firstName = firstNameField.GetProperty("stringValue").GetString() ?? "Unknown";

                if (fields.TryGetProperty("lastName", out var lastNameField))
                    lastName = lastNameField.GetProperty("stringValue").GetString() ?? "Unknown";
            }

            return (firstName, lastName);
        }
        public async Task<string> GetBranchAsync(string email)
        {
            var safeEmailId = email.Replace(".", "_").Replace("@", "_");
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{safeEmailId}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return "Unknown"; // Default fallback if user not found

            var json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("fields", out JsonElement fields))
            {
                if (fields.TryGetProperty("branch", out JsonElement branchField))
                {
                    return branchField.GetProperty("stringValue").GetString() ?? "Unknown";
                }
            }

            return "Unknown";
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
        public async Task SaveDeviceTokenAsync(string userEmail, string token)
        {
            var documentPath = $"users/{userEmail}";
            var payload = new
            {
                deviceToken = token
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{BaseFirestoreUrl}/{documentPath}?updateMask.fieldPaths=deviceToken")
            {
                Content = content
            };

#if ANDROID
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());
#endif


            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
#if ANDROID
private async Task<string> GetAccessToken()
{
    string jsonPath = Path.Combine(AppContext.BaseDirectory, "Secrets", "service-account.json");

    if (!File.Exists(jsonPath))
        throw new FileNotFoundException("Firebase service account file not found.", jsonPath);

    using var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read);
    var credentials = await GoogleCredential.FromStreamAsync(stream, CancellationToken.None);


    credentials = credentials.CreateScoped("https://www.googleapis.com/auth/datastore");

    var token = await credentials.UnderlyingCredential.GetAccessTokenForRequestAsync();
    return token;
}
#endif





        private static string SafeEmail(string email)
        {
            return email.Replace(".", "_").Replace("@", "_");
        }
    }
}
