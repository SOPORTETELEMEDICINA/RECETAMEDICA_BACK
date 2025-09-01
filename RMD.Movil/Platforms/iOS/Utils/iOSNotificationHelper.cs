using UIKit;
using UserNotifications;

namespace RMD.Movil.Platforms.iOS.Utils
{
    public static class iOSNotificationHelper
    {
        public static Task<bool> EnsureAsync()
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var center = UNUserNotificationCenter.Current;
            center.GetNotificationSettings(settings =>
            {
                if (settings.AuthorizationStatus == UNAuthorizationStatus.Authorized ||
                    settings.AuthorizationStatus == UNAuthorizationStatus.Provisional)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                        UIApplication.SharedApplication.RegisterForRemoteNotifications());
                    tcs.TrySetResult(true);
                    return;
                }

                center.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (approved, error) =>
                    {
                        if (approved)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                                UIApplication.SharedApplication.RegisterForRemoteNotifications());
                            tcs.TrySetResult(true);
                        }
                        else
                        {
                            tcs.TrySetResult(false);
                        }
                    });
            });

            return tcs.Task;
        }
    }
}