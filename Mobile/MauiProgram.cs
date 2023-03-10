using CommunityToolkit.Maui;

using DevExpress.Maui;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.LifecycleEvents;

using ShareInvest.Configures;

using System.Diagnostics;

namespace ShareInvest;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseDevExpress()
            .UseMauiCompatibility()
            .UseMauiCommunityToolkit(o =>
            {
                o.SetShouldSuppressExceptionsInConverters(false);
                o.SetShouldSuppressExceptionsInBehaviors(false);
                o.SetShouldSuppressExceptionsInAnimations(true);
            })
            .UseMauiMaps()
            .ConfigureEssentials(o =>
            {
                o.UseVersionTracking();
            })
            .ConfigureServices()
            .ConfigureViewModels()
            .ConfigurePages()
            .ConfigureFonts(o =>
            {
                o.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                o.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                o.AddFont("univia-pro-regular.ttf", "Univia-Pro");
                o.AddFont("roboto-bold.ttf", "Roboto-Bold");
                o.AddFont("roboto-regular.ttf", "Roboto");
            })
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android =>
                {
                    android.OnActivityResult((activity, requestCode, resultCode, data) => LogEvent(nameof(AndroidLifecycle.OnActivityResult),
                                                                                                   requestCode.ToString()))
                           .OnStart(activity => LogEvent(nameof(AndroidLifecycle.OnStart)))
                           .OnCreate((activity, bundle) => LogEvent(nameof(AndroidLifecycle.OnCreate)))
                           .OnBackPressed(activity => LogEvent(nameof(AndroidLifecycle.OnBackPressed)) && false)
                           .OnStop(activity => LogEvent(nameof(AndroidLifecycle.OnStop)));
                });
#elif IOS
                events.AddiOS(ios =>
                {
                    ios.OnActivated(app => LogEvent(nameof(iOSLifecycle.OnActivated)))
                       .OnResignActivation(app => LogEvent(nameof(iOSLifecycle.OnResignActivation)))
                       .DidEnterBackground(app => LogEvent(nameof(iOSLifecycle.DidEnterBackground)))
                       .WillTerminate(app => LogEvent(nameof(iOSLifecycle.WillTerminate)));
                });
#endif
                static bool LogEvent(string eventName, string? type = null)
                {
#if DEBUG
                    Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? string.Empty : $" ({type})")}");
#endif
                    return true;
                }
            });
#if DEBUG        
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}