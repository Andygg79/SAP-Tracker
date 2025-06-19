namespace SAPTracker.Models
{
    public class AppSettings
    {
        public FirebaseSettings Firebase { get; set; } = new();
        // Future config blocks can go here
        // public NotificationSettings Notifications { get; set; } = new();
    }

    public class FirebaseSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
    }

    // Optional future example
    // public class NotificationSettings
    // {
    //     public string FcmServerKey { get; set; } = string.Empty;
    //     public string SenderId { get; set; } = string.Empty;
    // }
}
