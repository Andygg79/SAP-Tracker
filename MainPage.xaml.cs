using Android.Content.Res;
using Microsoft.Maui.Controls;
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

            var firestoreService = new FirestoreService();
            bool profileComplete = await firestoreService.CheckProfileCompleteAsync(email);

            if (profileComplete)
            {
                await Navigation.PushAsync(new SelectionPage(email));
            }
            else
            {
                await Navigation.PushAsync(new SelectionPage(email));// (Temproary for now!)
            }
        }
    }


    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new RegisterPage());
    }
    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        var googleAuth = new GoogleAuthService();
        var (success, message) = await googleAuth.LoginWithGoogleAsync();

        if (success)
        {
            await DisplayAlert("Login Success", "You logged in with Google!", "Continue");
            await Navigation.PushAsync(new SelectionPage("temp@email.com"));// <-- later we can pass real email
        }
        else
        {
            await DisplayAlert("Login Failed", message, "OK");
        }
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
