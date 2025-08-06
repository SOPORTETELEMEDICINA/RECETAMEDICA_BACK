using RMD.Shared.Models.Receta.Detalle.Request;
using RMD.Shared.Models.Receta.Detalle.Response;

namespace RMD.Interface.Receta  
{
    public interface IDetalleRecetaService
    {
        Task<ResponseFromService<string>> CreateUpdateReaccionAsync(DetalleRequest request, Guid idUsuario);
        Task<ResponseFromService<string>> DeleteReaccionAsync(DetalleRequest request, Guid idUsuario);
        Task<ResponseFromService<List<DetalleResponse>>> GetDetallesByIdReceta(Guid idUsuario, Guid idReceta);
        Task<ResponseFromService<IEnumerable<MedicamentoActivoResponse>>> GetSoloMedicamentosActivos(Guid idUsuario);
        Task<ResponseFromService<IEnumerable<ReaccionesPacienteResponse>>> GetReaccionMedicamentoPrevioAsync(Guid idPaciente);

    }
}
