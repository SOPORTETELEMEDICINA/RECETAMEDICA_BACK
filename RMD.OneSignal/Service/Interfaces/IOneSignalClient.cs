using System.Threading.Tasks;

namespace RMD.OneSignal.Service.Interfaces;

public interface IOneSignalClient
{
    /// Registra/inicia sesión en OneSignal para asociar el dispositivo al ExternalUserId.
    Task LoginAsync(string externalUserId);

    /// Cierra sesión / desasocia (opcional)
    Task LogoutAsync();

    /// Obtiene el device/player id si está disponible
    Task<string?> GetDeviceIdAsync();
}