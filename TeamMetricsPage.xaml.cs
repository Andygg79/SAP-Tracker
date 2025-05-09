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
    private readonly string CurrentUserId = "";
    private readonly List<Teammate> teamMembers = [];

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
            "Enter the email address of the member you want to add:",
            "Add", "Cancel", "example@service.com", maxLength: 100, keyboard: Keyboard.Email);

        if (string.IsNullOrWhiteSpace(newMemberEmail))
            return;

        var firestoreService = new FirestoreService();
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

        if (string.IsNullOrWhiteSpace(email))
            return;

        var member = teamMembers.FirstOrDefault(t => t.Email == email);
        if (member != null)
        {
            teamMembers.Remove(member);
            RefreshTeamList();

            var firestoreService = new FirestoreService();
            bool success = await firestoreService.RemoveTeamMemberAsync(CurrentUserId, email);

            if (success)
            {
                await DisplayAlert("Success", "Team member removed successfully!", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to remove from database.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Not Found", "No team member with that email.", "OK");
        }
    }

    private void RefreshTeamList()
    {
        TeamList.ItemsSource = null;
        TeamList.ItemsSource = teamMembers;
    }

    private void UpdateSummary()
    {
        int total = teamMembers.Count;
        int red = teamMembers.Count(t => t.StatusColor == "Red");
        int amber = teamMembers.Count(t => t.StatusColor == "Amber");
        int green = teamMembers.Count(t => t.StatusColor == "Green");

        TotalMembersLabel.Text = $"👥 Total Members: {total}";
        RedCountLabel.Text = $"🔴 Red: {red}";
        AmberCountLabel.Text = $"🟠 Amber: {amber}";
        GreenCountLabel.Text = $"🟢 Green: {green}";
    }

    private void CheckForAlerts()
    {
        int redCount = teamMembers.Count(t => t.StatusColor == "Red");
        int amberCount = teamMembers.Count(t => t.StatusColor == "Amber");

        if (redCount > 0 || amberCount > 0)
        {
            _ = DisplayAlert(
                "⚠️ Team Readiness Warning",
                $"🔴 Red: {redCount}\n🟠 Amber: {amberCount}\n\nImmediate action recommended.",
                "OK"
            );
        }
    }

    private async Task LoadTeamMembers()
    {
        var firestoreService = new FirestoreService();
        var memberEmails = await firestoreService.GetTeamMembersAsync(CurrentUserId);

        teamMembers.Clear();
        foreach (var memberEmail in memberEmails)
        {
            var username = await firestoreService.GetUsernameAsync(memberEmail);
            var branch = await firestoreService.GetBranchAsync(memberEmail); // <-- NEW: Get branch
            var expectedMetrics = MetricsManagerService.GetMetricsForBranch(branch); // <-- NEW: branch-specific metrics

            var savedMetrics = await firestoreService.LoadMetricsAsync(memberEmail);

            string overallColor = "Green";

            // Check each expected metric for this branch
            foreach (var expectedMetric in expectedMetrics)
            {
                if (savedMetrics.TryGetValue(expectedMetric.MetricName, out var savedMetric))
                {
                    if (savedMetric.StatusColor.Equals("RED", StringComparison.OrdinalIgnoreCase))
                    {
                        overallColor = "Red";
                        break; // Highest priority
                    }
                    else if (savedMetric.StatusColor.Equals("AMBER", StringComparison.OrdinalIgnoreCase) && overallColor != "Red")
                    {
                        overallColor = "Amber"; // Set to Amber if no Red yet
                    }
                }
                else
                {
                    // If missing important metric, treat as Amber (or Red if you want)
                    overallColor = "Amber";
                }
            }

            teamMembers.Add(new Teammate
            {
                Name = username,
                Email = memberEmail,
                StatusColor = overallColor
            });
        }

        RefreshTeamList();
        UpdateSummary();
        CheckForAlerts();
    }


    private async void OnTeamMemberSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0 &&
            e.CurrentSelection[0] is Teammate selectedMember &&
            !string.IsNullOrWhiteSpace(selectedMember.Email))
        {
            await Navigation.PushAsync(new ServiceMemberProfilePage(selectedMember.Email));
        }


        ((CollectionView)sender).SelectedItem = null;
    }
}
