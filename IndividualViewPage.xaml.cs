using System.Collections.ObjectModel;
using SAPTracker.Helpers;
using SAPTracker.Models;
using SAPTracker.Services;

namespace SAPTracker;

public partial class IndividualViewPage : ContentPage
{
    private string UserEmail = "";
    private ObservableCollection<MetricEntry> MetricsListData = new();// Local cache for updates

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
        if (sender is DatePicker picker && picker.BindingContext is MetricEntry metric)
        {
            metric.LastUpdatedDate = picker.Date;

            var (color, label) = StatusHelper.GetStatus(picker.Date);
            metric.StatusColor = color;
            metric.StatusName = label;
        }
    }



    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var firestoreService = new FirestoreService();

        // Save the current metrics list to Firestore
        bool success = await firestoreService.SaveMetricsAsync(UserEmail, MetricsListData.ToList());

        if (success)
        {
            await DisplayAlert("Success", "Metrics updated successfully!", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Failed to update metrics. Try again.", "OK");
        }
    }

}
