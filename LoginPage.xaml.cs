using Plugin.Firebase.CloudMessaging;
using SAPTracker.Services;


namespace SAPTracker;

public partial class LoginPage : ContentPage
{
    private readonly FirebaseAuthService _authService;
    private readonly FirestoreService _firestoreService;

    public LoginPage(FirebaseAuthService authService, FirestoreService firestoreService)
    {
        InitializeComponent();
        _authService = authService;
        _firestoreService = firestoreService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Login Failed", "Please enter both email and password.", "OK");
            return;
        }

        var (success, message) = await _authService.LoginAsync(email, password);

        if (!success)
        {
            await DisplayAlert("Login Failed", message, "OK");
            return;
        }

        bool profileComplete = await _firestoreService.CheckProfileCompleteAsync(email);

        if (!profileComplete)
        {
            await DisplayAlert("Profile Incomplete", "Please complete your profile before proceeding.", "OK");
            return;
        }

        SessionService.StartSession(email);

        // ✅ Register the FCM token after login
        await RegisterDeviceTokenAsync(email);

        await DisplayAlert("Login Success", $"Welcome, {email}!", "Continue");

        var selectionPage = new SelectionPage(_firestoreService, email);
        await Navigation.PushAsync(selectionPage);
    }

    private async Task RegisterDeviceTokenAsync(string userEmail)
    {
        try
        {

            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            Console.WriteLine($"✅ Registered FCM Token: {token}");

            // Optionally: Save to Firestore
            await _firestoreService.SaveDeviceTokenAsync(userEmail, token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ FCM Token Registration Failed: {ex.Message}");
        }
    }



    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        var registerPage = new RegisterPage(_authService, _firestoreService);
        await Navigation.PushModalAsync(registerPage);
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        var forgotPage = new ForgotPasswordPage(_authService);
        await Navigation.PushModalAsync(forgotPage);
    }
}
