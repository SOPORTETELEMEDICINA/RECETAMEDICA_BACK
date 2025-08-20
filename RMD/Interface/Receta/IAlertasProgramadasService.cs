using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;

namespace RMD.Interface.Receta
{
    public interface IAlertasProgramadasService
    {
        Task<ResponseFromService<List<AlertaProgramadaResponse>>> GetAlertasProgramadasAsync(GetAlertasProgramadasRequest request);
        Task<ResponseFromService<IEnumerable<AlertaProgramadaResponse>>> GetAlertasProgramadasEnFechaAsync(GetAlertasProgramadasEnFechaRequest request);
    }
}
