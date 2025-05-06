using SAPTracker.Services;

namespace SAPTracker;

public partial class ProfilePage : ContentPage
{
    private readonly FirestoreService firestoreService = new FirestoreService();
    private readonly string userEmail = "user@example.com"; // <-- You should replace this with the actual signed-in user's email.

    public ProfilePage()
    {
        InitializeComponent();
        LoadBranch();
    }

    private async void LoadBranch()
    {
        try
        {
            var safeEmailId = userEmail.Replace(".", "_").Replace("@", "_");
            var url = $"https://firestore.googleapis.com/v1/projects/sapt-24jk2y/databases/(default)/documents/users/{safeEmailId}";
            var response = await new HttpClient().GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var document = System.Text.Json.JsonDocument.Parse(json);

                if (document.RootElement.TryGetProperty("fields", out var fields))
                {
                    if (fields.TryGetProperty("branch", out var branchField))
                    {
                        var branch = branchField.GetProperty("stringValue").GetString();
                        BranchLabel.Text = branch ?? "Unknown Branch";
                    }
                }
            }
            else
            {
                BranchLabel.Text = "Branch not found.";
            }
        }
        catch
        {
            BranchLabel.Text = "Error loading branch.";
        }
    }

    private async void OnSaveProfileClicked(object sender, EventArgs e)
    {
        string firstName = FirstNameEntry.Text?.Trim() ?? "";
        string lastName = LastNameEntry.Text?.Trim() ?? "";
        string rank = RankEntry.Text?.Trim() ?? "";
        string unit = UnitEntry.Text?.Trim() ?? "";
        string dutyTitle = DutyTitleEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(rank) || string.IsNullOrWhiteSpace(unit) || string.IsNullOrWhiteSpace(dutyTitle))
        {
            await DisplayAlert("Error", "Please fill out all fields.", "OK");
            return;
        }

        bool success = await firestoreService.SaveProfileAsync(userEmail, firstName, lastName, rank, unit, dutyTitle);

        if (success)
        {
            await DisplayAlert("Success", "Profile saved successfully!", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Failed to save profile.", "OK");
        }
    }
}
