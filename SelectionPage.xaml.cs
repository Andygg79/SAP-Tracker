namespace SAPTracker;

public partial class SelectionPage : ContentPage
{
    public SelectionPage()
    {
        InitializeComponent();
    }

    private async void OnIndividualClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new IndividualViewPage());
    }

    private async void OnTeamClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TeamMetricsPage());
    }
}
