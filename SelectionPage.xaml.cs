namespace SAPTracker;



public partial class SelectionPage : ContentPage
{
    private string CurrentUserEmail = "";
    public SelectionPage(string email)
    {
        InitializeComponent();
        CurrentUserEmail = email;
    }


    private async void OnIndividualClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new IndividualViewPage(CurrentUserEmail));
    }

    private async void OnTeamClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TeamMetricsPage(CurrentUserEmail));
    }
}
