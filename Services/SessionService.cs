namespace SAPTracker.Services
{
    public static class SessionService
    {
        public static bool IsLoggedIn { get; set; } = false;
        public static string CurrentUserEmail { get; set; } = string.Empty;
    }
}
