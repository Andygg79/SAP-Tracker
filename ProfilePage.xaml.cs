using SAPTracker.Services;

namespace SAPTracker;

public partial class ProfilePage : ContentPage
{
    private string CurrentUserEmail = "";

    public ProfilePage(string email)
    {
        InitializeComponent();
        CurrentUserEmail = email;
    }

    private async void OnSaveProfileClicked(object sender, EventArgs e)
    {
        var firstName = FirstNameEntry.Text?.Trim() ?? "";
        var lastName = LastNameEntry.Text?.Trim() ?? "";
        var rank = RankEntry.Text?.Trim() ?? "";
        var unit = UnitEntry.Text?.Trim() ?? "";
        var dutyTitle = DutyTitleEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(rank) || string.IsNullOrWhiteSpace(unit) || string.IsNullOrWhiteSpace(dutyTitle))
        {
            await DisplayAlert("Error", "Please complete all fields.", "OK");
            return;
        }

        var firestoreService = new FirestoreService();
        bool success = await firestoreService.SaveProfileAsync(CurrentUserEmail, firstName, lastName, rank, unit, dutyTitle);

        if (success)
        {
            await DisplayAlert("Success", "Profile saved successfully!", "OK");

            var firestoreServiceInstance = new FirestoreService(); // Create instance
            await Navigation.PushAsync(new SelectionPage(firestoreServiceInstance, CurrentUserEmail)); // Pass both
        }

        else
        {
            await DisplayAlert("Error", "Failed to save profile. Check your connection.", "OK");
        }
    }
}
