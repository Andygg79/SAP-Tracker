using SAPTracker.Services;

namespace SAPTracker;



public partial class SelectionPage : ContentPage
{
    private string CurrentUserEmail = "";
    public SelectionPage(string email)
    {
        InitializeComponent();
        CurrentUserEmail = email;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!SessionService.IsLoggedIn)
        {
            await DisplayAlert("Session Expired", "Please login again.", "OK");
            await Navigation.PopToRootAsync(); // Return to MainPage
        }
    }



    private async void OnIndividualClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new IndividualViewPage(CurrentUserEmail));
    }

    private async void OnTeamClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TeamMetricsPage(CurrentUserEmail));
    }
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        SessionService.IsLoggedIn = false; // Clear login flag

        await DisplayAlert("Logged Out", "You have been logged out.", "OK");

        await Navigation.PopToRootAsync(); // Return to MainPage (Login)
    }



}

