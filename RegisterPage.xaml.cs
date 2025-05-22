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

            // 🔧 Get FCM Token
            // var fcmToken = await FirebaseMessaging.DefaultInstance.GetTokenAsync(); //Temporarily commented out
            // Console.WriteLine($"FCM Token: {fcmToken}"); //Temporarily commented out

            // 🔧 Save user profile AND device token to Firestore
            await firestoreService.SaveUserProfileAsync(email, branch, username);
            // await firestoreService.SaveDeviceTokenAsync(email, fcmToken); //Temporarily commented out

            await DisplayAlert("Success", "Account created!", "OK");

            await Navigation.PopModalAsync();
            { await Navigation.PushModalAsync(new MainPage()); } // Navigate to MainPage after registration
            
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
