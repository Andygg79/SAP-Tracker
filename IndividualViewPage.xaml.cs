using SAPTracker.Models;
namespace SAPTracker;

public partial class IndividualViewPage : ContentPage
{
    private Dictionary<string, MetricEntry> metrics = new();
    public IndividualViewPage()
    {
        InitializeComponent();

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

    private void OnMetricDateChanged(object sender, DateChangedEventArgs e)
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
            status = "YELLOW";
            button.BackgroundColor = Colors.Gold;
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


}
