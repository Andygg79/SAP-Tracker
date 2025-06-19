using SAPTracker.Services;
namespace SAPTracker;

public class Teammate
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string StatusColor { get; set; } = "Gray";
}

public partial class TeamMetricsPage : ContentPage
{
    private string CurrentUserId = "";
    private List<Teammate> teamMembers = new();
    private readonly FirestoreService firestoreService = new();

    public TeamMetricsPage(string userEmail)
    {
        InitializeComponent();
        CurrentUserId = userEmail;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!SessionService.IsLoggedIn)
        {
            await DisplayAlert("Session Expired", "Please login again.", "OK");
            await Navigation.PopToRootAsync();
            return;
        }

        await LoadTeamMembers();
    }

    private async void OnAddMemberClicked(object sender, EventArgs e)
    {
        string newMemberEmail = await DisplayPromptAsync(
            "Add Team Member",
            "Enter the email address of the soldier you want to add:",
            "Add", "Cancel", "example@soldier.com", maxLength: 100, keyboard: Keyboard.Email);

        if (string.IsNullOrWhiteSpace(newMemberEmail)) return;

        if (teamMembers.Any(m => m.Email?.Equals(newMemberEmail, StringComparison.OrdinalIgnoreCase) == true))
        {
            await DisplayAlert("Already Added", "This soldier is already in your team list.", "OK");
            return;
        }

        bool success = await firestoreService.AddTeamMemberAsync(CurrentUserId, newMemberEmail);

        if (success)
        {
            await DisplayAlert("Success", "Team member added successfully!", "OK");
            await LoadTeamMembers();
        }
        else
        {
            await DisplayAlert("Error", "Failed to add team member. Check the email or try again.", "OK");
        }
    }

    private async void OnRemoveMemberClicked(object sender, EventArgs e)
    {
        string email = await DisplayPromptAsync("Remove Member", "Enter email to remove:");

        if (string.IsNullOrWhiteSpace(email)) return;

        var soldier = teamMembers.FirstOrDefault(t => t.Email == email);
        if (soldier != null)
        {
            teamMembers.Remove(soldier);
            RefreshTeamList();

            bool success = await firestoreService.RemoveTeamMemberAsync(CurrentUserId, email);

            if (success)
                await DisplayAlert("Success", "Team member removed successfully!", "OK");
            else
                await DisplayAlert("Error", "Failed to remove from database.", "OK");
        }
        else
        {
            await DisplayAlert("Not Found", "No team member with that email.", "OK");
        }
    }

    private void RefreshTeamList()
    {
        TeamList.ItemsSource = null;
        TeamList.ItemsSource = teamMembers.OrderByDescending(t => t.StatusColor).ToList();
    }

    private void UpdateSummary()
    {
        int total = teamMembers.Count;
        int red = teamMembers.Count(t => t.StatusColor == "Red");
        int amber = teamMembers.Count(t => t.StatusColor == "Amber");
        int green = teamMembers.Count(t => t.StatusColor == "Green");

        TotalSoldiersLabel.Text = $"\uD83E\uDE96 Total Soldiers: {total}";
        RedCountLabel.Text = $"\uD83D\uDD34 Red: {red}";
        AmberCountLabel.Text = $"\uD83D\uDFE0 Amber: {amber}";
        GreenCountLabel.Text = $"\uD83D\uDFE2 Green: {green}";
    }

    private void CheckForAlerts()
    {
        int redCount = teamMembers.Count(t => t.StatusColor == "Red");
        int amberCount = teamMembers.Count(t => t.StatusColor == "Amber");

        if (redCount > 0 || amberCount > 0)
        {
            _ = DisplayAlert(
                "\u26A0\uFE0F Team Readiness Warning",
                $"\uD83D\uDD34 Red: {redCount}\n\uD83D\uDFE0 Amber: {amberCount}\n\nImmediate action recommended.",
                "OK");
        }
    }

    private async Task LoadTeamMembers()
    {
        var memberEmails = await firestoreService.GetTeamMembersAsync(CurrentUserId);

        var tasks = memberEmails.Select(async memberEmail =>
        {
            var (firstName, lastName) = await firestoreService.GetUserProfileAsync(memberEmail);
            var metrics = await firestoreService.LoadMetricsAsync(memberEmail);

            string overallColor = "Green";
            var normalizedColors = metrics.Select(m => m.Value.StatusColor.ToUpperInvariant());

            if (normalizedColors.Contains("RED")) overallColor = "Red";
            else if (normalizedColors.Contains("AMBER")) overallColor = "Amber";

            return new Teammate
            {
                Name = $"{firstName} {lastName}",
                Email = memberEmail,
                StatusColor = overallColor
            };
        });

        teamMembers = (await Task.WhenAll(tasks)).ToList();

        RefreshTeamList();
        UpdateSummary();
        CheckForAlerts();
    }

    private async void OnTeamMemberSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Teammate selectedSoldier && !string.IsNullOrWhiteSpace(selectedSoldier.Email))
        {
            await Navigation.PushAsync(new SoldierProfilePage(selectedSoldier.Email));
        }

        ((CollectionView)sender).SelectedItem = null;
    }
}
