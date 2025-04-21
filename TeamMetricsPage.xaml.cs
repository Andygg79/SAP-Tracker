using SAPTracker.Services;
namespace SAPTracker;

public class Teammate
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public partial class TeamMetricsPage : ContentPage
{
    private string CurrentUserEmail = "";
    private List<Teammate> teamMembers = new();


    public TeamMetricsPage(string userEmail)
    {
        InitializeComponent();
        CurrentUserEmail = userEmail;

        LoadTeamMembers();
    }


    private async void OnAddMemberClicked(object sender, EventArgs e)
    {
        string newMemberEmail = await DisplayPromptAsync(
            "Add Team Member",
            "Enter the email address of the soldier you want to add:",
            "Add", "Cancel", "example@soldier.com", maxLength: 100, keyboard: Keyboard.Email);

        if (string.IsNullOrWhiteSpace(newMemberEmail))
            return;

        var firestoreService = new FirestoreService();
        bool success = await firestoreService.AddTeamMemberAsync(CurrentUserEmail, newMemberEmail);

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

        var soldier = teamMembers.FirstOrDefault(t => t.Email == email);
        if (soldier != null)
        {
            teamMembers.Remove(soldier);
            RefreshTeamList();
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
    private async Task LoadTeamMembers()
    {
        var firestoreService = new FirestoreService();
        var members = await firestoreService.GetTeamMembersAsync(CurrentUserEmail);

        teamMembers.Clear();
        foreach (var memberEmail in members)
        {
            teamMembers.Add(new Teammate { Name = "Unknown Soldier", Email = memberEmail });
        }

        RefreshTeamList();
    }

}
