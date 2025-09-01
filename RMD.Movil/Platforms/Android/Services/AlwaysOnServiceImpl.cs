using RMD.Movil.Core.Service.Interfaces;
using AApp = global::Android.App;
using AContext = global::Android.Content;
using AOS = global::Android.OS;
using ASettings = global::Android.Provider.Settings;
using AXCompat = global::AndroidX.Core.Content;
using AUri = global::Android.Net;

namespace RMD.Movil.Platforms.Android.Services
{
    public sealed class AlwaysOnServiceImpl : IAlwaysOnService
    {
        public Task<bool> StartAsync(CancellationToken ct = default)
        {
            var ctx = AApp.Application.Context ?? throw new InvalidOperationException("Application.Context es null");
            var intent = new AContext.Intent(ctx, typeof(AlwaysOnForegroundService));

            if (AOS.Build.VERSION.SdkInt >= AOS.BuildVersionCodes.O)
                ctx.StartForegroundService(intent);
            else
                ctx.StartService(intent);

            return Task.FromResult(true);
        }

        public Task StopAsync(CancellationToken ct = default)
        {
            var ctx = AApp.Application.Context ?? throw new InvalidOperationException("Application.Context es null");
            var intent = new AContext.Intent(ctx, typeof(AlwaysOnForegroundService));
            ctx.StopService(intent);
            return Task.CompletedTask;
        }

        public Task<bool> IsRunningAsync()
        {
            // Implementación simple; si necesitas certeza, usa un flag persistente o Binder.
            return Task.FromResult(true);
        }

        public Task<bool> RequestIgnoreBatteryOptimizationsAsync()
        {
            var ctx = AApp.Application.Context ?? throw new InvalidOperationException("Application.Context es null");
            var pm = (AOS.PowerManager?)ctx.GetSystemService(AContext.Context.PowerService);
            var pkg = ctx.PackageName ?? string.Empty;

            if (pm?.IsIgnoringBatteryOptimizations(pkg) == true)
                return Task.FromResult(true);

            var intent = new AContext.Intent(ASettings.ActionRequestIgnoreBatteryOptimizations);
            intent.SetData(AUri.Uri.Parse($"package:{pkg}"));
            intent.AddFlags(AContext.ActivityFlags.NewTask);

            AXCompat.ContextCompat.StartActivity(ctx, intent, null);

            return Task.FromResult(false);
        }
    }
}
