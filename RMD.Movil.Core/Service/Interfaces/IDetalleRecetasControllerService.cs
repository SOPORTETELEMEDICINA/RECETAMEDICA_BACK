using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.Detalle.Request;
using RMD.Shared.Models.Receta.Detalle.Response;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IDetalleRecetasControllerService
    {
        Task<ResponseFromService<List<DetalleResponse>>> GetDetallesByIdRecetaAsync(Guid idReceta);
        Task<ResponseFromService<IEnumerable<MedicamentoActivoResponse>>> GetSoloMedicamentosActivosAsync();
        Task<ResponseFromService<string>> CreateUpdateReaccionAsync(DetalleRequest request);
        Task<ResponseFromService<List<DetalleResponse>>> GetDetalleByPacienteAsync(Guid? idPaciente = null);
    }
}
