using SAPTracker.Services;

namespace SAPTracker;

public partial class ForgotPasswordPage : ContentPage
{
    private readonly FirebaseAuthService _authService;

    public ForgotPasswordPage(FirebaseAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Error", "Please enter your email address.", "OK");
            return;
        }

        // Show loading spinner
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        var (success, message) = await _authService.SendPasswordResetEmailAsync(email);

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
