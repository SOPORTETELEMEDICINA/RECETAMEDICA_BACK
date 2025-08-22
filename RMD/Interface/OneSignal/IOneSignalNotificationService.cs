using RMD.Shared.Models.OneSignal.Request;

namespace RMD.Interface.OneSignal
{
    public interface IOneSignalNotificationService
    {
        Task<ResponseFromService<bool>> ProgramarRecordatorioTomaAsync(
            List<ProgramarTomaRequest> req, CancellationToken ct = default);
    }
}
