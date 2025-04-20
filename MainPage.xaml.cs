using SAPTracker.Services;

namespace SAPTracker;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Login Failed", "Please enter both email and password.", "OK");
            return;
        }

        var authService = new FirebaseAuthService();
        var (success, message) = await authService.LoginAsync(email, password);

        if (success)
        {
            await DisplayAlert("Login Success", $"Welcome, {email}!", "Continue");
            await Navigation.PushAsync(new SelectionPage(email));
         }
        else
        {
            await DisplayAlert("Login Failed", message, "Try again.");
        }
    }



    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new RegisterPage());
    }
    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Coming Soon", "Google login is not yet implemented.", "OK");
    }
    private async void OnAppleLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Coming Soon", "Apple ID login is not yet implemented.", "OK");
    }
    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        string email = await DisplayPromptAsync("Forgot Password", "Enter your email address:", "Send Reset Link", "Cancel", "Email", maxLength: 100, keyboard: Keyboard.Email);

        if (string.IsNullOrWhiteSpace(email))
            return;

        var authService = new FirebaseAuthService();
        var (success, message) = await authService.SendPasswordResetEmailAsync(email);

        if (success)
        {
            await DisplayAlert("Success", "Password reset email sent! Check your inbox.", "OK");
        }
        else
        {
            await DisplayAlert("Error", message, "Try Again");
        }
    }

}
