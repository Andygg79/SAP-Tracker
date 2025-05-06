using SAPTracker.Models;
using SAPTracker.Services;

namespace SAPTracker;

public partial class ServiceMemberProfilePage : ContentPage
{
    private string MemberEmail = "";

    public ServiceMemberProfilePage(string email)
    {
        InitializeComponent();
        MemberEmail = email;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var firestoreService = new FirestoreService();

        // Get service member full profile
        var (firstName, lastName, rank, unit, dutyTitle) = await firestoreService.GetFullUserProfileAsync(MemberEmail);

        NameLabel.Text = $"{firstName} {lastName}";
        RankLabel.Text = $"Rank: {rank}";
        UnitLabel.Text = $"Unit: {unit}";
        DutyTitleLabel.Text = $"Duty Title: {dutyTitle}";

        // Get their branch
        var branch = await firestoreService.GetBranchAsync(MemberEmail);

        // Load expected metrics for their branch
        var expectedMetrics = MetricsManagerService.GetMetricsForBranch(branch);

        // Load actual saved metrics from Firestore
        var savedMetrics = await firestoreService.LoadMetricsAsync(MemberEmail);

        // Merge expected + actual to show complete picture
        List<MetricEntry> displayMetrics = new();

        foreach (var expectedMetric in expectedMetrics)
        {
            if (savedMetrics.TryGetValue(expectedMetric.MetricName, out var savedMetric))
            {
                displayMetrics.Add(new MetricEntry
                {
                    MetricName = expectedMetric.MetricName,
                    LastUpdatedDate = savedMetric.LastUpdatedDate,
                    StatusColor = savedMetric.StatusColor
                });
            }
            else
            {
                displayMetrics.Add(new MetricEntry
                {
                    MetricName = expectedMetric.MetricName,
                    LastUpdatedDate = DateTime.MinValue, // No record yet
                    StatusColor = "Gray"
                });
            }
        }

        MetricsList.ItemsSource = displayMetrics;
    }
}
