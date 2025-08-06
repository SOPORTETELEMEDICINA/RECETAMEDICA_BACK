using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.Detalle.Response;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IDetalleRecetasControllerService
    {
        Task<ResponseFromService<List<DetalleResponse>>> GetDetallesByIdRecetaAsync(Guid idReceta);
    }
}
