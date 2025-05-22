using Android.App;
using Android.Content.PM;
using Microsoft.Maui;
using SAPTracker;

namespace SAPTracker.Platforms.Android;

[Activity(Exported = true, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
public class MainActivity : MauiAppCompatActivity
{
}
