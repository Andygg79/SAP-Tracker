using SAPTracker.Services;

namespace SAPTracker;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Error", "Please enter your email address.", "OK");
            return;
        }

        var authService = new FirebaseAuthService();
        var (success, message) = await authService.SendPasswordResetEmailAsync(email);

        // Show loading spinner
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;        

        authService = new FirebaseAuthService();
        (success, message) = await authService.SendPasswordResetEmailAsync(email);

        // Hide loading spinner
        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        if (success)
        {
            await DisplayAlert("Success", "Password reset email sent! Check your inbox.", "OK");
            await Navigation.PopModalAsync();
        }
        else
        {
            await DisplayAlert("Error", message, "OK");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
