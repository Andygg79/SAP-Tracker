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

        var branch = await firestoreService.GetBranchAsync(MemberEmail);
        var expectedMetrics = MetricsManagerService.GetMetricsForBranch(branch);
        var savedMetrics = await firestoreService.LoadMetricsAsync(MemberEmail);

        List<MetricEntry> displayMetrics = new();

        foreach (var expectedMetric in expectedMetrics)
        {
            if (savedMetrics.TryGetValue(expectedMetric.MetricName, out var savedMetric))
            {
                displayMetrics.Add(new MetricEntry
                {
                    MetricName = expectedMetric.MetricName,
                    LastUpdatedDate = savedMetric.LastUpdatedDate,
                    StatusColor = savedMetric.StatusColor,
                    StatusName = savedMetric.StatusName
                });
            }
            else
            {
                displayMetrics.Add(new MetricEntry
                {
                    MetricName = expectedMetric.MetricName,
                    LastUpdatedDate = DateTime.MinValue,
                    StatusColor = "Gray",
                    StatusName = "Unknown"
                });
            }
        }

        MetricsList.ItemsSource = displayMetrics;
    }
}
