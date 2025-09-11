using RMD.Shared.Models.OneSignal.Request;

namespace RMD.Interface.OneSignal
{
    public interface IOneSignalNotificationService
    {
        Task<ResponseFromService<bool>> ProgramarRecordatorioTomaAsync(
            List<ProgramarTomaRequest> req, CancellationToken ct = default);
        Task<ResponseFromService<bool>> CancelarProgramadasAsync(List<CancelarNotificacionRequest> porCancelar, CancellationToken ct = default);
        Task<ResponseFromService<bool>> PosponerNotificacionAsync(PosponerNotificacionRequest req);

    }
}
