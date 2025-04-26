using SAPTracker.Services;

namespace SAPTracker;

public partial class SoldierProfilePage : ContentPage
{
    private string SoldierEmail = "";

    public SoldierProfilePage(string email)
    {
        InitializeComponent();
        SoldierEmail = email;
        LoadSoldierData();
    }

    private async void LoadSoldierData()
    {
        var firestoreService = new FirestoreService();

        var (firstName, lastName) = await firestoreService.GetUserProfileAsync(SoldierEmail);
        var metrics = await firestoreService.LoadMetricsAsync(SoldierEmail);

        NameLabel.Text = $"{firstName} {lastName}";

        // Fetch full profile fields
        var profile = await firestoreService.GetFullUserProfileAsync(SoldierEmail);

        RankLabel.Text = $"Rank: {profile.Rank ?? "Unknown"}";
        UnitLabel.Text = $"Unit: {profile.Unit ?? "Unknown"}";
        DutyTitleLabel.Text = $"Duty Title: {profile.DutyTitle ?? "Unknown"}";

        // Metrics
        MetricsList.ItemsSource = metrics.Select(m => new
        {
            MetricName = m.Key,
            Status = m.Value.StatusColor.ToUpper() switch
            {
                "RED" => Colors.Red,
                "AMBER" => Colors.LightCoral,
                "GREEN" => Colors.Green,
                _ => Colors.Gray
            },
            LastUpdate = m.Value.LastUpdatedDate.ToString("yyyy-MM-dd")
        }).ToList();
    }

}
