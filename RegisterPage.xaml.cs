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

        var authService = new FirebaseAuthService();
        var (success, message) = await authService.RegisterAsync(email, password);

        if (success)
        {
            await DisplayAlert("Success", "Account created successfully!", "OK");
            await Navigation.PopAsync(); // Go back to MainPage after successful registration
        }
        else
        {
            await DisplayAlert("Registration Failed", message, "Try Again");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

}
