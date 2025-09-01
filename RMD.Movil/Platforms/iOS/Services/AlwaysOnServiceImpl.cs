using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.Platforms.iOS.Services
{
    public sealed class AlwaysOnServiceImpl : IAlwaysOnService
    {
        public Task<bool> StartAsync(CancellationToken ct = default)
        {
            // iOS no permite “siempre activo”. Aquí podrías:
            // - Programar BGTask (ya se hace en AppDelegate)
            // - Suscribirte a silent pushes (OneSignal)
            return Task.FromResult(true);
        }

        public Task StopAsync(CancellationToken ct = default)
        {
            // No hay “stop” de BGTask; podrías cancelar flags internos si usas uno
            return Task.CompletedTask;
        }

        public Task<bool> IsRunningAsync() => Task.FromResult(false);

        public Task<bool> RequestIgnoreBatteryOptimizationsAsync() => Task.FromResult(true);
    }
}