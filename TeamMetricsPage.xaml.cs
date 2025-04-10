namespace SAPTracker;

public class Teammate
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public partial class TeamMetricsPage : ContentPage
{
    private List<Teammate> teamMembers = new();

    public TeamMetricsPage()
    {
        InitializeComponent();

        // Sample demo data
        teamMembers.Add(new Teammate { Name = "Sgt. Ramirez", Email = "ramirez@army.mil" });
        teamMembers.Add(new Teammate { Name = "Cpl. Harper", Email = "harper@army.mil" });

        TeamList.ItemsSource = teamMembers;
    }

    private async void OnAddMemberClicked(object sender, EventArgs e)
    {
        string email = await DisplayPromptAsync("Add Member", "Enter soldier's email:");

        if (!string.IsNullOrWhiteSpace(email))
        {
            teamMembers.Add(new Teammate { Name = "New Soldier", Email = email });
            RefreshTeamList();
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
}
