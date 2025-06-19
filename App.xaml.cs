using SAPTracker.Services;

namespace SAPTracker
{
    public partial class App : Application
    {
        private readonly FirebaseAuthService authService;
        private readonly FirestoreService firestoreService;

        public App()
        {
            InitializeComponent();

            // Initialize services once
            authService = new FirebaseAuthService();
            firestoreService = new FirestoreService();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window
            {
                Page = new NavigationPage(new LoginPage(authService, firestoreService))
            };
        }
    }
}
