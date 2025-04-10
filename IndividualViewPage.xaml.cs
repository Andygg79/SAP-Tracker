namespace SAPTracker;

public partial class IndividualViewPage : ContentPage
{
    public IndividualViewPage()
    {
        InitializeComponent();
        UpdateWeaponsStatus();
    }

    private void OnWeaponsDateChanged(object sender, DateChangedEventArgs e)
    {
        UpdateWeaponsStatus();
    }

    private void UpdateWeaponsStatus()
    {
        var daysUntil = (WeaponsDatePicker.Date - DateTime.Now).TotalDays;

        if (daysUntil < 0)
        {
            // It's an old date → check how old
            var daysSince = Math.Abs(daysUntil);

            if (daysSince <= 30)
            {
                WeaponsStatusButton.Text = "RED";
                WeaponsStatusButton.BackgroundColor = Colors.Red;
            }
            else if (daysSince <= 90)
            {
                WeaponsStatusButton.Text = "YELLOW";
                WeaponsStatusButton.BackgroundColor = Colors.Gold;
            }
            else
            {
                WeaponsStatusButton.Text = "GREEN";
                WeaponsStatusButton.BackgroundColor = Colors.Green;
            }
        }
        else
        {
            // Future date = good to go = green
            WeaponsStatusButton.Text = "GREEN";
            WeaponsStatusButton.BackgroundColor = Colors.Green;
        }

        WeaponsStatusButton.TextColor = Colors.White;

    }

}
