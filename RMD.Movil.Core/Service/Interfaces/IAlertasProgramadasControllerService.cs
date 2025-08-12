using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IAlertasProgramadasControllerService
    {
        Task<ResponseFromService<List<AlertaProgramadaResponse>>> GetAlertasProgramadasByIdPacienteAsync(GetAlertasProgramadasRequest filterRequest);
    }
}
