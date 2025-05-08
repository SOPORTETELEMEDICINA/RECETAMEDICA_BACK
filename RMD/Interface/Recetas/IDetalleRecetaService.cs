using RMD.Models.Recetas;
using RMD.Models.Responses;

namespace RMD.Interface.Recetas
{
    public interface IDetalleRecetaService
    {
       // Task<ResponseFromService<DetalleReceta>> GetDetalleRecetaByIdAsync(Guid idDetalleReceta);
        //Task<ResponseFromService<IEnumerable<DetalleReceta>>> GetDetalleRecetasByRecetaAsync(Guid idReceta);
        //Task<ResponseFromService<DetalleRecetaResponse>> GetReaccionByIdRecetaAsync(DetalleRecetaRequest request, Guid idUsuario);
        Task<ResponseFromService<string>> CreateUpdateReaccionAsync(DetalleRecetaRequest request, Guid idUsuario);
        Task<ResponseFromService<string>> DeleteReaccionAsync(DetalleRecetaRequest request, Guid idUsuario);
        Task<ResponseFromService<List<Detalle_RecetaDetalleRequest>>> GetDetallesByIdReceta(Guid idUsuario, Guid idReceta);
        Task<ResponseFromService<IEnumerable<MedicamentoActivoConsulta>>> GetSoloMedicamentosActivos(Guid idUsuario);

    }
}
