using SAPTracker.Services;

namespace SAPTracker;

public partial class SelectionPage : ContentPage
{
    private readonly FirestoreService _firestoreService;
    private readonly string _userEmail;

    public SelectionPage(FirestoreService firestoreService, string userEmail)
    {
        InitializeComponent();
        _firestoreService = firestoreService;
        _userEmail = userEmail;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!SessionService.IsLoggedIn)
        {
            await DisplayAlert("Session Expired", "Please login again.", "OK");
            await Navigation.PopToRootAsync(); // Return to login page
        }
    }

    #region Navigation Handlers

    private async void OnIndividualClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new IndividualViewPage(_userEmail));
    }

    private async void OnTeamClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TeamMetricsPage(_userEmail));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        SessionService.EndSession();
        await DisplayAlert("Logged Out", "You have been logged out.", "OK");
        await Navigation.PopToRootAsync(); // Return to login page
    }

    #endregion
}
