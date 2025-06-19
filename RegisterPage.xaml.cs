using SAPTracker.Services;

namespace SAPTracker;

public partial class RegisterPage : ContentPage
{
    private readonly FirebaseAuthService _authService;
    private readonly FirestoreService _firestoreService;

    public RegisterPage(FirebaseAuthService authService, FirestoreService firestoreService)
    {
        InitializeComponent();
        _authService = authService;
        _firestoreService = firestoreService;
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

        if (!email.Contains("@") || !email.Contains("."))
        {
            await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        if (password.Length < 8)
        {
            await DisplayAlert("Error", "Password should be at least 8 characters long.", "OK");
            return;
        }

        CreateAccountButton.IsEnabled = false;

        try
        {
            var (success, message) = await _authService.RegisterAsync(email, password);

            if (success)
            {
                bool profileSaved = await _firestoreService.SaveUserProfileAsync(email);

                if (profileSaved)
                {
                    await DisplayAlert("Success", "Account created and profile saved!", "OK");
                }
                else
                {
                    await DisplayAlert("Partial Success", "Account created but failed to save profile.", "OK");
                }

                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Registration Failed", message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Unexpected Error", "Something went wrong. Try again later.", "OK");
            Console.WriteLine($"Registration error: {ex.Message}");
        }
        finally
        {
            CreateAccountButton.IsEnabled = true;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
