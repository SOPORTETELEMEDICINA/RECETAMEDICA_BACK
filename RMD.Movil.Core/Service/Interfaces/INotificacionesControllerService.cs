using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.OneSignal.Request;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface INotificacionesControllerService
    {
        Task<ResponseFromService<object>> ProgramarTomaAsync(ProgramarTomaRequest req);
    }
}
