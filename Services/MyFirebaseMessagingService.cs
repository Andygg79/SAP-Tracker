#if ANDROID
#pragma warning disable CA1416

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using Firebase.Messaging;
using Microsoft.Maui.Storage;
using SAPTracker;
using SAPTracker.Platforms.Android;

namespace SAPTracker.Services
{
    [Service(Name = "com.saptracker.app.MyFirebaseMessagingService")]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]

    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFCMService";
        private readonly FirestoreService _firestoreService = new();

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            Log.Debug(TAG, $"From: {message.From}");

            RemoteMessage.Notification? notification = message.GetNotification();
            if (notification != null)
            {
                string title = notification.Title ?? "SAPTracker";
                string body = notification.Body ?? "You have a new message.";
                SendNotification(title, body);
            }
            else
            {
                Log.Warn(TAG, "FCM message received without notification payload.");
            }
        }

        public override async void OnNewToken(string token)
        {
            base.OnNewToken(token);
            Log.Debug(TAG, $"Refreshed token: {token}");

            try
            {
                string? email = await SecureStorage.GetAsync("userEmail");

                if (!string.IsNullOrWhiteSpace(email))
                {
                    await _firestoreService.SaveDeviceTokenAsync(email, token);
                    Log.Debug(TAG, "Device token saved to Firestore.");
                }
                else
                {
                    Log.Warn(TAG, "User email is null or empty. Cannot save token.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"Error saving FCM token: {ex.Message}");
            }
        }

        private void SendNotification(string title, string messageBody)
        {
            Intent? intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);

            PendingIntent? pendingIntent;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                pendingIntent = PendingIntent.GetActivity(
                    this,
                    0,
                    intent,
                    flags: PendingIntentFlags.OneShot | PendingIntentFlags.Immutable
                );
            }
            else
            {
                pendingIntent = PendingIntent.GetActivity(
                    this,
                    0,
                    intent,
                    PendingIntentFlags.OneShot
                );
            }

            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, "fcm_default_channel")
                .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                .SetContentTitle(title ?? "SAPTracker")
                .SetContentText(messageBody ?? "You have a new message.")
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            NotificationManager notificationManager = NotificationManager.FromContext(this)!;
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
#pragma warning restore CA1416
#endif
