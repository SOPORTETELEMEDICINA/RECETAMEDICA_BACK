using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.Catalogos;


namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IRecetaCatalogosControllerService
    {
        Task<ResponseFromService<List<FrecuencyTypeListModel>>> GetFrecuencyTypesAsync();
        Task<ResponseFromService<FrecuencyTypeListModel>> GetFrecuencyTypeByIdAsync(int id);
    }
}
