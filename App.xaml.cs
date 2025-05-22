namespace SAPTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Routing.RegisterRoute("LoginPage", typeof(MainPage));
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window
            {
                Page = new NavigationPage(new MainPage())
            };
        }

    }
}