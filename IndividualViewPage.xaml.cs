namespace SAPTracker;

public partial class IndividualViewPage : ContentPage
{
    public IndividualViewPage()
    {
        InitializeComponent();

        // Initialize button states
        UpdateStatus(WeaponsDatePicker, WeaponsStatusButton);
        UpdateStatus(DentalDatePicker, DentalStatusButton);
        UpdateStatus(PHADatePicker, PHAStatusButton);
        UpdateStatus(VisionDatePicker, VisionStatusButton);
        UpdateStatus(HearingDatePicker, HearingStatusButton);
        UpdateStatus(DD93DatePicker, DD93StatusButton);
        UpdateStatus(DA5960DatePicker, DA5960StatusButton);
        UpdateStatus(PRRDatePicker, PRRStatusButton);
        UpdateStatus(SGLVDatePicker, SGLVStatusButton);
        UpdateStatus(ARBDatePicker, ARBStatusButton);
        UpdateStatus(EvalDatePicker, EvalStatusButton);

    }

    private void OnMetricDateChanged(object sender, DateChangedEventArgs e)
    {
        if (sender is DatePicker datePicker)
        {
            Button? statusButton = datePicker switch
            {
                var d when d == WeaponsDatePicker => WeaponsStatusButton,
                var d when d == DentalDatePicker => DentalStatusButton,
                var d when d == PHADatePicker => PHAStatusButton,
                var d when d == WeaponsDatePicker => WeaponsStatusButton,
                var d when d == DentalDatePicker => DentalStatusButton,
                var d when d == PHADatePicker => PHAStatusButton,
                var d when d == VisionDatePicker => VisionStatusButton,
                var d when d == HearingDatePicker => HearingStatusButton,
                var d when d == DD93DatePicker => DD93StatusButton,
                var d when d == DA5960DatePicker => DA5960StatusButton,
                var d when d == PRRDatePicker => PRRStatusButton,
                var d when d == SGLVDatePicker => SGLVStatusButton,
                var d when d == ARBDatePicker => ARBStatusButton,
                var d when d == EvalDatePicker => EvalStatusButton,
                _ => null
            };

            if (statusButton != null)
                UpdateStatus(datePicker, statusButton);
        }
    }

    private void UpdateStatus(DatePicker picker, Button button)
    {
        var expirationDate = picker.Date.AddYears(1);
        var daysUntilExpiration = (expirationDate - DateTime.Now).TotalDays;

        if (daysUntilExpiration <= 30)
        {
            button.Text = "RED";
            button.BackgroundColor = Colors.Red;
        }
        else if (daysUntilExpiration <= 90)
        {
            button.Text = "YELLOW";
            button.BackgroundColor = Colors.Gold;
        }
        else
        {
            button.Text = "GREEN";
            button.BackgroundColor = Colors.Green;
        }

        button.TextColor = Colors.White;
    }

}
