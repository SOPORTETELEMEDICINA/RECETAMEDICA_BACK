namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IAlwaysOnService
    {
        Task<bool> StartAsync(CancellationToken ct = default); // true si quedó activo
        Task StopAsync(CancellationToken ct = default);
        Task<bool> IsRunningAsync();
        Task<bool> RequestIgnoreBatteryOptimizationsAsync(); // Android: abre Settings
    }
}
