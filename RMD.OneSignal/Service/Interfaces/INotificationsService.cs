using RMD.OneSignal.Models;

namespace RMD.OneSignal.Service.Interfaces;

public interface INotificationsService
{
    // Envío inmediato a un usuario
    Task SendNowAsync(NotificationRequest req);

    // Envío programado a un usuario (una o varias horas)
    Task ScheduleAsync(NotificationRequest req, ScheduleOptions schedule);

    // “Paquete del día”: mandar varias notificaciones a varios pacientes
    Task SendDailyPackageAsync(IEnumerable<string> externalUserIds, IEnumerable<(string title, string body, DateTimeOffset at)> alerts, string? timeZone = null);
}