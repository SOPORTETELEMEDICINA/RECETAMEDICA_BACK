using RMD.Models.Recetas;

namespace RMD.Interface.Recetas
{
    public interface IDetalleRecetaService
    {
        Task<DetalleReceta> GetDetalleRecetaByIdAsync(Guid id);
        Task<IEnumerable<DetalleReceta>> GetDetalleRecetasByRecetaAsync(Guid idReceta);
    }
}
