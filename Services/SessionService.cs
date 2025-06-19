namespace SAPTracker.Services
{
    public static class SessionService
    {
        public static bool IsLoggedIn { get; private set; } = false;
        public static string CurrentUserEmail { get; private set; } = string.Empty;

        public static void StartSession(string email)
        {
            IsLoggedIn = true;
            CurrentUserEmail = email;
        }

        public static void EndSession()
        {
            IsLoggedIn = false;
            CurrentUserEmail = string.Empty;
        }

        public static bool HasValidSession()
        {
            return IsLoggedIn && !string.IsNullOrWhiteSpace(CurrentUserEmail);
        }
    }
}
