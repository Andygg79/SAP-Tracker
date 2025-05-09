using SAPTracker.Services;

namespace SAPTracker;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text ?? "";
        string username = UsernameEntry.Text?.Trim() ?? "";
        string branch = BranchPicker.SelectedItem?.ToString() ?? "";

        if (string.IsNullOrWhiteSpace(username))
        {
            await DisplayAlert("Error", "Please choose a username.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Error", "Please fill all fields.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(branch))
        {
            await DisplayAlert("Error", "Please select a branch.", "OK");
            return;
        }

        var authService = new FirebaseAuthService();
        var (success, message) = await authService.RegisterAsync(email, password);

        if (success)
        {
            var firestoreService = new FirestoreService();
            await firestoreService.SaveUserProfileAsync(email, branch, username);

            await DisplayAlert("Success", "Account created!", "OK");

            // ✅ Safely close the modal and navigate from root
            await Navigation.PopModalAsync();

            if (Application.Current?.MainPage is NavigationPage navPage)
            {
                await navPage.Navigation.PushAsync(new SelectionPage(email));
            }
        }
        else
        {
            await DisplayAlert("Registration Failed", message, "OK");
        }
    }


    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
