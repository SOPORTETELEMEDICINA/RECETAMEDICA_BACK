using BackgroundTasks;
using Foundation;
using RMD.Movil.Services.Push;      // PushRegistrar
using UIKit;
using UserNotifications;

namespace RMD.Movil.Platforms.iOS;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    // Debe coincidir con Info.plist -> BGTaskSchedulerPermittedIdentifiers
    private const string RefreshTaskId = "com.rmd.movil.bgrefresh";

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        var ok = base.FinishedLaunching(app, options);

        // Mostrar notificaciones también en foreground (útil para pruebas)
        UNUserNotificationCenter.Current.Delegate = new ForegroundNotifDelegate();

        // 3.1 Background Fetch (el SO decide la cadencia)
        UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

        // 3.2 BGTaskScheduler (iOS 13+)
        if (OperatingSystem.IsIOSVersionAtLeast(13))
        {
            BGTaskScheduler.Shared.Register(RefreshTaskId, null, task =>
            {
                HandleRefreshTask((BGAppRefreshTask)task);
            });

            ScheduleAppRefresh();
        }

        // Ocultar barra de estado como tenías
        if (Window != null)
            Window.RootViewController = new FullscreenViewController();

        return ok;
    }

    // ========= APNs TOKEN (sin override, usando [Export]) =========

    // iOS llama cuando obtiene el token APNs del dispositivo
    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public void DidRegisterForRemoteNotifications(UIApplication application, NSData deviceToken)
    {
        try
        {
            var token = deviceToken.ToHexString();
            if (!string.IsNullOrWhiteSpace(token))
            {
                Preferences.Default.Set("APNsToken", token);
                _ = PushRegistrar.RegisterIosTokenAsync(token);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"APNs token handle error: {ex}");
        }
    }

    // iOS llama si falla el registro en APNs
    [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
    public void DidFailToRegisterForRemoteNotifications(UIApplication application, NSError error)
    {
        System.Diagnostics.Debug.WriteLine($"APNs register failed: {error}");
    }

    // Push silencioso (background) con fetchCompletionHandler
    [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
    public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
    {
        // si necesitas hacer algo al llegar un push silencioso, hazlo aquí
        completionHandler(UIBackgroundFetchResult.NoData);
    }

    // ========= Background Fetch clásico =========
    public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
    {
        _ = DoLightRefreshAsync().ContinueWith(t =>
        {
            if (t.IsFaulted) completionHandler(UIBackgroundFetchResult.Failed);
            else if (t.Result) completionHandler(UIBackgroundFetchResult.NewData);
            else completionHandler(UIBackgroundFetchResult.NoData);
        });
    }

    // ========= BGTaskScheduler =========
    private void ScheduleAppRefresh()
    {
        if (!OperatingSystem.IsIOSVersionAtLeast(13)) return;

        var request = new BGAppRefreshTaskRequest(RefreshTaskId)
        {
            EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(15 * 60) // ~15 min
        };

        BGTaskScheduler.Shared.Submit(request, out var _);
    }

    private void HandleRefreshTask(BGAppRefreshTask task)
    {
        // Reprogramar siguiente
        ScheduleAppRefresh();

        task.ExpirationHandler = () =>
        {
            // Cancela si te pasas de la ventana permitida
        };

        _ = DoLightRefreshAsync().ContinueWith(t => task.SetTaskCompleted(success: !t.IsFaulted));
    }

    // Trabajo breve: renovar token, sync flags, etc.
    private async Task<bool> DoLightRefreshAsync()
    {
        try
        {
            await Task.Delay(500);
            return true; // true si hubo “nuevos datos”
        }
        catch
        {
            return false;
        }
    }

    // ====== Helpers ======
    class FullscreenViewController : UIViewController
    {
        public override bool PrefersStatusBarHidden() => true;
    }

    sealed class ForegroundNotifDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification n, Action<UNNotificationPresentationOptions> done)
            => done(UNNotificationPresentationOptions.Banner | UNNotificationPresentationOptions.List | UNNotificationPresentationOptions.Sound);
    }
}

// Extensión para convertir NSData -> hex string (token APNs)
public static class NSDataExtensions
{
    public static string ToHexString(this NSData data)
    {
        var bytes = data.ToArray();
        var sb = new System.Text.StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) sb.AppendFormat("{0:x2}", b);
        return sb.ToString();
    }
}
