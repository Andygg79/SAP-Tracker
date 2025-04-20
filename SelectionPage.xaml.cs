namespace SAPTracker;



public partial class SelectionPage : ContentPage
{
    private string CurrentUserEmail = "";
    public SelectionPage(string userEmail)
    {
        InitializeComponent();
        CurrentUserEmail = userEmail;
    }


    private async void OnIndividualClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new IndividualViewPage(CurrentUserEmail));
    }

    private async void OnTeamClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TeamMetricsPage());
    }
}
