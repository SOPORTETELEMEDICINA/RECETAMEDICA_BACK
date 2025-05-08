using RMD.Models.Responses;

namespace RMD.Interface.Notificaciones
{
    public interface ICatalogoNotificacionService
    {
        Task<CatalogoNotificacion?> GetNotificationByCodeAsync (int codigoNotificacion);
        Task<CatalogoNotificacion?> GetNotificationByTipoAndFuncionAsync(string tipo, string funcion);
    }

}
