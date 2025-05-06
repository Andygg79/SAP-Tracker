using SAPTracker.Helpers;
using SAPTracker.Models;
using SAPTracker.Services;

namespace SAPTracker;

public partial class IndividualViewPage : ContentPage
{
    private string UserEmail = "";
    private List<MetricEntry> MetricsListData = new(); // Local cache for updates

    public IndividualViewPage(string email)
    {
        InitializeComponent();
        UserEmail = email;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var firestoreService = new FirestoreService();
        var branch = await firestoreService.GetBranchAsync(UserEmail);

        var metrics = MetricsManagerService.GetMetricsForBranch(branch);

        // Load existing saved metrics if available
        var savedMetrics = await firestoreService.LoadMetricsAsync(UserEmail);

        MetricsListData.Clear();
        foreach (var metric in metrics)
        {
            if (savedMetrics.TryGetValue(metric.MetricName, out var saved))
            {
                MetricsListData.Add(new MetricEntry
                {
                    MetricName = metric.MetricName,
                    LastUpdatedDate = saved.LastUpdatedDate,
                    StatusColor = saved.StatusColor
                });
            }
            else
            {
                MetricsListData.Add(new MetricEntry
                {
                    MetricName = metric.MetricName,
                    LastUpdatedDate = DateTime.Today,
                    StatusColor = "Gray"
                });
            }
        }

        MetricsCollectionView.ItemsSource = MetricsListData;
    }

    private void OnMetricDateChanged(object sender, DateChangedEventArgs e)
    {
        if (sender is DatePicker picker)
        {
            if (picker.BindingContext is MetricEntry metric)
            {
                var newColor = StatusHelper.GetStatusColor(picker.Date);

                metric.LastUpdatedDate = picker.Date;
                metric.StatusColor = newColor;

                // Force the CollectionView to refresh its items
                MetricsCollectionView.ItemsSource = null;
                MetricsCollectionView.ItemsSource = MetricsListData;
            }
        }
    }
}
