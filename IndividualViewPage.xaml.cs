using SAPTracker.Models;
using SAPTracker.Services;

namespace SAPTracker;

public partial class IndividualViewPage : ContentPage
{
    private readonly Dictionary<string, MetricEntry> metrics = new();
    private readonly string _currentUserEmail;

    public IndividualViewPage(string userEmail)
    {
        InitializeComponent();
        _currentUserEmail = userEmail;
    }

    #region Life Cycle

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!SessionService.IsLoggedIn)
        {
            await DisplayAlert("Session Expired", "Please login again.", "OK");
            await Navigation.PopToRootAsync();
            return;
        }

        await LoadMetrics();
    }

    #endregion

    #region Metric Logic

    private async Task LoadMetrics()
    {
        var firestoreService = new FirestoreService();
        var loadedMetrics = await firestoreService.LoadMetricsAsync(_currentUserEmail);

        foreach (var metric in loadedMetrics)
        {
            DatePicker? picker = metric.Key switch
            {
                "Weapons" => WeaponsDatePicker,
                "Dental" => DentalDatePicker,
                "PHA" => PHADatePicker,
                "VISION" => VisionDatePicker,
                "HEARING" => HearingDatePicker,
                "DD93" => DD93DatePicker,
                "DA5960" => DA5960DatePicker,
                "PRR" => PRRDatePicker,
                "SGLV" => SGLVDatePicker,
                "ARB" => ARBDatePicker,
                "EVAL" => EvalDatePicker,
                _ => null
            };

            Button? button = metric.Key switch
            {
                "Weapons" => WeaponsStatusButton,
                "Dental" => DentalStatusButton,
                "PHA" => PHAStatusButton,
                "VISION" => VisionStatusButton,
                "HEARING" => HearingStatusButton,
                "DD93" => DD93StatusButton,
                "DA5960" => DA5960StatusButton,
                "PRR" => PRRStatusButton,
                "SGLV" => SGLVStatusButton,
                "ARB" => ARBStatusButton,
                "EVAL" => EvalStatusButton,
                _ => null
            };

            if (picker != null && button != null)
            {
                picker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(picker, button, metric.Key);
            }
        }
    }

    private void UpdateStatus(DatePicker picker, Button button, string metricName)
    {
        DateTime expirationDate = picker.Date.AddYears(1);
        double daysLeft = (expirationDate - DateTime.Now).TotalDays;
        string status = daysLeft <= 30 ? "RED" : daysLeft <= 90 ? "AMBER" : "GREEN";

        button.Text = status;
        button.BackgroundColor = status switch
        {
            "RED" => Colors.Red,
            "AMBER" => Colors.Orange,
            "GREEN" => Colors.Green,
            _ => Colors.Gray
        };
        button.TextColor = Colors.White;

        metrics[metricName] = new MetricEntry
        {
            MetricName = metricName,
            LastUpdatedDate = picker.Date,
            StatusColor = status
        };
    }

    private async Task SaveMetricAsync(DatePicker picker, Button button, string metricName)
    {
        UpdateStatus(picker, button, metricName);
        var firestoreService = new FirestoreService();
        bool saved = await firestoreService.SaveMetricsAsync(_currentUserEmail, metrics);

        if (!saved)
        {
            await DisplayAlert("Warning", "Failed to save metrics. Check your connection.", "OK");
        }

        await CheckIndividualAlertsAsync();
    }

    private async Task CheckIndividualAlertsAsync()
    {
        bool hasRed = metrics.Values.Any(m => m.StatusColor == "RED");
        bool hasAmber = metrics.Values.Any(m => m.StatusColor == "AMBER");

        if (hasRed)
        {
            await DisplayAlert("⚠️ Critical Alert", "One or more of your metrics is RED. Immediate action is needed!", "OK");
        }
        else if (hasAmber)
        {
            await DisplayAlert("⚠️ Warning", "One or more of your metrics is AMBER. Plan to update soon.", "OK");
        }
    }

    #endregion

    #region Event Handlers

    private async void OnMetricDateChanged(object sender, DateChangedEventArgs e)
    {
        if (sender is not DatePicker picker) return;

        var (button, metricName) = picker switch
        {
            _ when picker == WeaponsDatePicker => (WeaponsStatusButton, "Weapons"),
            _ when picker == DentalDatePicker => (DentalStatusButton, "Dental"),
            _ when picker == PHADatePicker => (PHAStatusButton, "PHA"),
            _ when picker == VisionDatePicker => (VisionStatusButton, "VISION"),
            _ when picker == HearingDatePicker => (HearingStatusButton, "HEARING"),
            _ when picker == DD93DatePicker => (DD93StatusButton, "DD93"),
            _ when picker == DA5960DatePicker => (DA5960StatusButton, "DA5960"),
            _ when picker == PRRDatePicker => (PRRStatusButton, "PRR"),
            _ when picker == SGLVDatePicker => (SGLVStatusButton, "SGLV"),
            _ when picker == ARBDatePicker => (ARBStatusButton, "ARB"),
            _ when picker == EvalDatePicker => (EvalStatusButton, "EVAL"),
            _ => (null, "")
        };

        if (button != null && !string.IsNullOrEmpty(metricName))
        {
            await SaveMetricAsync(picker, button, metricName);
        }
    }

    #endregion
}
