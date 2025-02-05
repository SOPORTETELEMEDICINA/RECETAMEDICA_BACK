using RMD.Models.Responses;

namespace RMD.Interface.CargaCatalogos
{
    public interface ICargaCatalogosService
    {
        Task<ResponseFromService<string>> ReloadCatalogs();
    }
}
