using Microsoft.Extensions.Logging;
using SAPTracker; // ✅ Add this line
using SAPTracker.Services;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>() // ✅ Now this will resolve correctly
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<FirebaseAuthService>();
        builder.Services.AddSingleton<FirestoreService>();

        return builder.Build();
    }
}
