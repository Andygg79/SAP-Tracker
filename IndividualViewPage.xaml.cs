using SAPTracker.Models;
namespace SAPTracker;
using SAPTracker.Services;


public partial class IndividualViewPage : ContentPage
{
    private Dictionary<string, MetricEntry> metrics = new();
    private string CurrentUserEmail = "";

    public IndividualViewPage(string userEmail)
    {
        InitializeComponent();
        CurrentUserEmail = userEmail;


        // Initialize button states
        UpdateStatus(WeaponsDatePicker, WeaponsStatusButton, "Weapons");
        UpdateStatus(DentalDatePicker, DentalStatusButton, "Dental");
        UpdateStatus(PHADatePicker, PHAStatusButton, "PHA");
        UpdateStatus(VisionDatePicker, VisionStatusButton, "VISION");
        UpdateStatus(HearingDatePicker, HearingStatusButton, "HEARING");
        UpdateStatus(DD93DatePicker, DD93StatusButton, "DD93");
        UpdateStatus(DA5960DatePicker, DA5960StatusButton, "DA5960");
        UpdateStatus(PRRDatePicker, PRRStatusButton, "PRR");
        UpdateStatus(SGLVDatePicker, SGLVStatusButton, "SGLV");
        UpdateStatus(ARBDatePicker, ARBStatusButton, "ARB");
        UpdateStatus(EvalDatePicker, EvalStatusButton, "EVAL");
    }    



    private async void OnMetricDateChanged(object sender, DateChangedEventArgs e)
    {
        Button? statusButton = null;
        string metricName = "";

        if (sender is DatePicker datePicker)
        {
            if (datePicker == WeaponsDatePicker) { statusButton = WeaponsStatusButton; metricName = "Weapons"; }
            else if (datePicker == DentalDatePicker) { statusButton = DentalStatusButton; metricName = "Dental"; }
            else if (datePicker == PHADatePicker) { statusButton = PHAStatusButton; metricName = "PHA"; }
            else if (datePicker == VisionDatePicker) { statusButton = VisionStatusButton; metricName = "VISION"; }
            else if (datePicker == HearingDatePicker) { statusButton = HearingStatusButton; metricName = "HEARING"; }
            else if (datePicker == DD93DatePicker) { statusButton = DD93StatusButton; metricName = "DD93"; }
            else if (datePicker == DA5960DatePicker) { statusButton = DA5960StatusButton; metricName = "DA5960"; }
            else if (datePicker == PRRDatePicker) { statusButton = PRRStatusButton; metricName = "PRR"; }
            else if (datePicker == SGLVDatePicker) { statusButton = SGLVStatusButton; metricName = "SGLV"; }
            else if (datePicker == ARBDatePicker) { statusButton = ARBStatusButton; metricName = "ARB"; }
            else if (datePicker == EvalDatePicker) { statusButton = EvalStatusButton; metricName = "EVAL"; }

            if (statusButton != null && metricName != "")
            {
                UpdateStatus(datePicker, statusButton, metricName);
                var firestoreService = new FirestoreService();
                bool saved = await firestoreService.SaveMetricsAsync(CurrentUserEmail, metrics); // <-- we'll define CurrentUserEmail next

                if (!saved)
                {
                    await DisplayAlert("Warning", "Failed to save metrics to server. Please check your connection.", "OK");
                    await CheckIndividualAlertsAsync();

                }
            }
        }

    }

    private void UpdateStatus(DatePicker picker, Button button, string metricName)
    {
        var expirationDate = picker.Date.AddYears(1);
        var daysUntilExpiration = (expirationDate - DateTime.Now).TotalDays;
        string status;

        if (daysUntilExpiration <= 30)
        {
            status = "RED";
            button.BackgroundColor = Colors.Red;
        }
        else if (daysUntilExpiration <= 90)
        {
            status = "AMBER";
            button.BackgroundColor = Colors.LightCoral;
        }
        else
        {
            status = "GREEN";
            button.BackgroundColor = Colors.Green;
        }

        button.Text = status;
        button.TextColor = Colors.White;

        // Save to our local metric dictionary
        metrics[metricName] = new MetricEntry
        {
            MetricName = metricName,
            LastUpdatedDate = picker.Date,
            StatusColor = status
        };
    }
    private async Task LoadMetrics()
    {
        var firestoreService = new FirestoreService();
        var loadedMetrics = await firestoreService.LoadMetricsAsync(CurrentUserEmail);

        foreach (var metric in loadedMetrics)
        {
            if (metric.Key == "Weapons")
            {
                WeaponsDatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(WeaponsDatePicker, WeaponsStatusButton, "Weapons");
            }
            else if (metric.Key == "Dental")
            {
                DentalDatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(DentalDatePicker, DentalStatusButton, "Dental");
            }
            else if (metric.Key == "PHA")
            {
                PHADatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(PHADatePicker, PHAStatusButton, "PHA");
            }
            else if (metric.Key == "VISION")
            {
                VisionDatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(VisionDatePicker, VisionStatusButton, "VISION");
            }
            else if (metric.Key == "HEARING")
            {
                HearingDatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(HearingDatePicker, HearingStatusButton, "HEARING");
            }
            else if (metric.Key == "DD93")
            {
                DD93DatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(DD93DatePicker, DD93StatusButton, "DD93");
            }
            else if (metric.Key == "DA5960")
            {
                DA5960DatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(DA5960DatePicker, DA5960StatusButton, "DA5960");
            }
            else if (metric.Key == "PRR")
            {
                PRRDatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(PRRDatePicker, PRRStatusButton, "PRR");
            }
            else if (metric.Key == "SGLV")
            {
                SGLVDatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(SGLVDatePicker, SGLVStatusButton, "SGLV");
            }
            else if (metric.Key == "ARB")
            {
                ARBDatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(ARBDatePicker, ARBStatusButton, "ARB");
            }
            else if (metric.Key == "EVAL")
            {
                EvalDatePicker.Date = metric.Value.LastUpdatedDate;
                UpdateStatus(EvalDatePicker, EvalStatusButton, "EVAL");
            }
        }
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
            await DisplayAlert("⚠️ Warning Alert", "One or more of your metrics is AMBER. Plan to update soon!", "OK");
        }
    }
    protected override async void OnAppearing()
{
    base.OnAppearing();

    if (!SessionService.IsLoggedIn)
    {
        await DisplayAlert("Session Expired", "Please login again.", "OK");
        await Navigation.PopToRootAsync(); // Go back to Login
        return; // Stop execution
    }

    await LoadMetrics(); // Only load if logged in
}





}
