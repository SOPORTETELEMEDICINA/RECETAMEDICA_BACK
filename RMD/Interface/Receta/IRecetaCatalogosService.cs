using RMD.Shared.Models.Receta.Catalogos;

namespace RMD.Interface.Receta
{
    public interface IRecetaCatalogosService
    {
        Task<ResponseFromService<List<FrecuencyTypeListModel>>> ObtenerFrecuencyTypesAsync();
        Task<ResponseFromService<FrecuencyTypeListModel>> ObtenerFrecuencyTypePorIdAsync(int id);
    }
}