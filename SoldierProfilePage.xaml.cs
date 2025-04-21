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
        RankLabel.Text = $"Rank: {firstName}";  // TODO: Replace with real rank from profile if available
        UnitLabel.Text = $"Unit: {firstName}";  // TODO: Replace with real unit if available
        DutyTitleLabel.Text = $"Duty Title: {firstName}";  // TODO: Replace with real duty title

        // Populate Metrics List
        MetricsList.ItemsSource = metrics.Select(m => new
        {
            MetricName = m.Key,
            Status = m.Value.StatusColor.ToUpper() switch
            {
                "RED" => Colors.Red,
                "AMBER" => Colors.Gold,
                "GREEN" => Colors.Green,
                _ => Colors.Gray
            },
            LastUpdate = m.Value.LastUpdatedDate.ToString("yyyy-MM-dd")
        }).ToList();

    }
}
