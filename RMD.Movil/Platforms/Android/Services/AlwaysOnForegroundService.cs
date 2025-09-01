using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace RMD.Movil.Platforms.Android.Services
{
    [Service(
        Exported = false,
        ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeDataSync
    )]
    public sealed class AlwaysOnForegroundService : Service
    {
        public const string ChannelId = "always_on_channel";
        public const int NotificationId = 10001;

        public override void OnCreate()
        {
            base.OnCreate();
            CreateNotificationChannel();
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            var notification = BuildNotification("Servicio activo para tareas en segundo plano.");
            StartForeground(NotificationId, notification);

            // TODO: aquí tu bucle/worker (Timers, WorkManager, etc.)
            // Mantén el trabajo ligero; usa WorkManager para trabajos periódicos.

            return StartCommandResult.Sticky; // pide al sistema re-crear si lo mata
        }

        public override IBinder? OnBind(Intent? intent) => null;

        public override void OnDestroy()
        {
            base.OnDestroy();
            // Limpieza si aplica
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

            var channel = new NotificationChannel(
                ChannelId,
                "RMD Always-On",
                NotificationImportance.Min
            )
            {
                Description = "Mantiene tareas críticas en primer plano."
            };

            var mgr = (NotificationManager?)GetSystemService(NotificationService);
            if (mgr != null)
                mgr.CreateNotificationChannel(channel);
        }

        private Notification BuildNotification(string text)
        {
            var builder = new NotificationCompat.Builder(this, ChannelId)
                .SetContentTitle("RMD en segundo plano")
                .SetContentText(text)
                // por esta temporalmente:
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetOngoing(true)
                .SetOnlyAlertOnce(true)
                .SetCategory(NotificationCompat.CategoryService)
                .SetPriority((int)NotificationPriority.Min);

            return builder.Build();
        }
    }
}
