using System.Threading.Tasks;

// ALIAS para evitar colisiones con tu namespace que contiene "Android"
using AOS = global::Android.OS;
using ACPM = global::Android.Content.PM;
using AMan = global::Android.Manifest;
using AXApp = global::AndroidX.Core.App.ActivityCompat;
using AXCompat = global::AndroidX.Core.Content.ContextCompat;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace RMD.Movil.Platforms.Android.Utils
{
    public static class NotificationPermissionHelper
    {
        public static Task EnsureAsync()
        {
            // Android 13+ (Tiramisu)
            if (AOS.Build.VERSION.SdkInt < AOS.BuildVersionCodes.Tiramisu)
                return Task.CompletedTask;

            var activity = Platform.CurrentActivity;
            if (activity == null)
                return Task.CompletedTask;

            var granted = AXCompat.CheckSelfPermission(activity, AMan.Permission.PostNotifications)
                          == ACPM.Permission.Granted;
            if (granted)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            AXApp.RequestPermissions(activity, new[] { AMan.Permission.PostNotifications }, 1001);

            // “Best effort”: resolvemos inmediato; si quieres esperar el callback,
            // maneja OnRequestPermissionsResult en MainActivity.
            tcs.TrySetResult(true);
            return tcs.Task;
        }
    }
}