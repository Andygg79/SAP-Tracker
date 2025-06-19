using SAPTracker.Services;

namespace SAPTracker;

public partial class SoldierProfilePage : ContentPage
{
    private readonly string SoldierEmail;

    public SoldierProfilePage(string email)
    {
        InitializeComponent();
        SoldierEmail = email;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSoldierData();
    }

    private async Task LoadSoldierData()
    {
        var firestoreService = new FirestoreService();

        var (firstName, lastName) = await firestoreService.GetUserProfileAsync(SoldierEmail);
        var profile = await firestoreService.GetFullUserProfileAsync(SoldierEmail);
        var metrics = await firestoreService.LoadMetricsAsync(SoldierEmail);

        NameLabel.Text = $"{firstName} {lastName}";
        RankLabel.Text = $"Rank: {profile.Rank}";
        UnitLabel.Text = $"Unit: {profile.Unit}";
        DutyTitleLabel.Text = $"Duty Title: {profile.DutyTitle}";

        var viewModelList = new List<MetricEntryViewModel>();
        string overallStatus = "GREEN";

        foreach (var (metricName, entry) in metrics)
        {
            viewModelList.Add(new MetricEntryViewModel
            {
                MetricName = metricName,
                StatusColor = entry.StatusColor.ToUpper() switch
                {
                    "RED" => Colors.Red,
                    "AMBER" => Colors.Orange,
                    "GREEN" => Colors.Green,
                    _ => Colors.Gray
                },
                LastUpdatedDate = entry.LastUpdatedDate.ToString("yyyy-MM-dd")
            });

            if (entry.StatusColor.ToUpper() == "RED")
                overallStatus = "RED";
            else if (entry.StatusColor.ToUpper() == "AMBER" && overallStatus != "RED")
                overallStatus = "AMBER";
        }

        // Set overall status color in top-level dot
        StatusBox.Color = overallStatus switch
        {
            "RED" => Colors.Red,
            "AMBER" => Colors.Orange,
            "GREEN" => Colors.Green,
            _ => Colors.Gray
        };

        MetricsList.ItemsSource = viewModelList;
    }
}

public class MetricEntryViewModel
{
    public string MetricName { get; set; } = "";
    public string LastUpdatedDate { get; set; } = "";
    public Color StatusColor { get; set; } = Colors.Gray;
}
