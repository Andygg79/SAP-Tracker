using SAPTracker.Services;

namespace SAPTracker;

public partial class SelectBranchPage : ContentPage
{
    private readonly string UserEmail = "";

    public SelectBranchPage(string email)
    {
        InitializeComponent();
        UserEmail = email;
    }

    private async void OnSaveBranchClicked(object sender, EventArgs e)
    {
        var selectedBranch = BranchPicker.SelectedItem?.ToString();

        if (string.IsNullOrWhiteSpace(selectedBranch))
        {
            await DisplayAlert("Error", "Please select a branch.", "OK");
            return;
        }

        var firestoreService = new FirestoreService();
        bool success = await firestoreService.SaveBranchAsync(UserEmail, selectedBranch);

        if (success)
        {
            await DisplayAlert("Success", "Branch saved successfully!", "OK");
            // Navigate to Main Dashboard (SelectionPage)
            await Navigation.PushAsync(new SelectionPage(UserEmail));
        }
        else
        {
            await DisplayAlert("Error", "Failed to save branch. Please try again.", "OK");
        }
    }
}
