using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatalogoService
    {
        // Búsqueda de Asentamientos
        Task<ResponseFromService<IEnumerable<AsentamientoResultModel>>> BuscarAsentamientosAsync(AsentamientoSearchModel searchModel);
        Task<ResponseFromService<int>> InsertarDomicilioCompletoAsync(DomicilioRequest model);
    }
}
