using SAPTracker.Services;

namespace SAPTracker;

public partial class LoginPage : ContentPage
{
    public LoginPage()
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
            SessionService.IsLoggedIn = true;
            SessionService.CurrentUserEmail = email;

            await DisplayAlert("Login Success", $"Welcome, {email}!", "Continue");

            var firestoreService = new FirestoreService();
            bool profileComplete = await firestoreService.CheckProfileCompleteAsync(email);

            if (profileComplete)
            {
                await Navigation.PushAsync(new SelectionPage(email));
            }
            else
            {
                await Navigation.PushAsync(new SelectionPage(email)); // (Temporary for now!)
            }
        }
        else
        {
            await DisplayAlert("Login Failed", message, "OK");
        }
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new RegisterPage());
    }
    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ForgotPasswordPage());
    }

}
