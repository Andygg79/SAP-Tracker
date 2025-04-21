using SAPTracker.Services;
namespace SAPTracker;

public class Teammate
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string StatusColor { get; set; } = "Gray"; // New field!
}



public partial class TeamMetricsPage : ContentPage
{
    private string CurrentUserEmail = "";
    private List<Teammate> teamMembers = new();


    public TeamMetricsPage(string userEmail)
    {
        InitializeComponent();
        CurrentUserEmail = userEmail;
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

        if (string.IsNullOrWhiteSpace(email))
            return;

        var soldier = teamMembers.FirstOrDefault(t => t.Email == email);
        if (soldier != null)
        {
            // Remove from local list
            teamMembers.Remove(soldier);
            RefreshTeamList();

            // Remove from Firestore
            var firestoreService = new FirestoreService();
            bool success = await firestoreService.RemoveTeamMemberAsync(CurrentUserEmail, email);

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
    private async Task LoadTeamMembers()
    {
        var firestoreService = new FirestoreService();
        var memberEmails = await firestoreService.GetTeamMembersAsync(CurrentUserEmail);

        teamMembers.Clear();
        foreach (var memberEmail in memberEmails)
        {
            var (firstName, lastName) = await firestoreService.GetUserProfileAsync(memberEmail);
            var metrics = await firestoreService.LoadMetricsAsync(memberEmail);

            string overallColor = "Green"; // Start assuming best

            if (metrics.Any(m => m.Value.StatusColor == "RED"))
                overallColor = "Red";
            else if (metrics.Any(m => m.Value.StatusColor == "AMBER"))
                overallColor = "Gold";

            teamMembers.Add(new Teammate
            {
                Name = $"{firstName} {lastName}",
                Email = memberEmail,
                StatusColor = overallColor
            });

        }

        RefreshTeamList();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTeamMembers();
    }
    private async void OnTeamMemberSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Teammate selectedSoldier)
        {
            await Navigation.PushAsync(new SoldierProfilePage(selectedSoldier.Email));
        }

    // Deselect after click
    ((CollectionView)sender).SelectedItem = null;
    }




}
