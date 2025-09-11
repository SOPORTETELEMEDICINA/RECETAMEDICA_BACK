using Android.App;
using Android.Content;
using Android.OS;

namespace RMD.Movil.Platforms.Android.Receivers
{
    [BroadcastReceiver(Enabled = true, Exported = false, DirectBootAware = true)]
    [IntentFilter(new[] {
        Intent.ActionBootCompleted,
        "android.intent.action.LOCKED_BOOT_COMPLETED"
    })]
    public sealed class BootCompletedReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null) return;

            // Reinicia tu servicio si así lo deseas
            var svc = new Intent(context, typeof(Services.AlwaysOnForegroundService));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                context.StartForegroundService(svc);
            else
                context.StartService(svc);
        }
    }
}
