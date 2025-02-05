using RMD.Models.Recetas;

namespace RMD.Interface.Recetas
{
    public interface IDetalleRecetaService
    {
        Task<DetalleReceta> GetDetalleRecetaByIdAsync(Guid id);
        Task<IEnumerable<DetalleReceta>> GetDetalleRecetasByRecetaAsync(Guid idReceta);
        Task<DetalleRecetaResponse> GetReaccionByIdRecetaAsync(DetalleRecetaRequest request, Guid IdUsuario);
        Task<(bool, string)> CreateUpdateReaccionAsync(DetalleRecetaRequest request, Guid IdUsuario);
        Task<(bool, string)> DeleteReaccionAsync(DetalleRecetaRequest request, Guid IdUsuario);
    }
}
