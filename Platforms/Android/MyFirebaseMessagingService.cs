using Android.App;
using Android.Util;
using Firebase.Messaging;

namespace SAPTracker.Platforms.Android
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug("FCM", $"📩 Received message: {message.GetNotification()?.Body}");
        }

        public override void OnNewToken(string token)
        {
            Log.Debug("FCM", $"🔑 New FCM Token: {token}");
        }
    }
}
