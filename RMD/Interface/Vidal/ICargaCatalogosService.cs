using RMD.Models.Responses;

namespace RMD.Interface.Vidal
{
    public interface ICargaCatalogosService
    {
        Task<ResponseFromService<string>> ReloadCatalogs();
        //Task<ResponseFromService<string>> GetAllVMPsAsync();
        //Task<ResponseFromService<string>> GetAllProductsAsync();
        //Task<ResponseFromService<string>> GetAllPackagesAsync(); // Nuevo método para Packages
        //Task<ResponseFromService<string>> GetAllUnitsAsync();
        //Task<ResponseFromService<string>> GetAllAllergiesAsync();
        //Task<ResponseFromService<string>> GetAllMoleculesAsync();
        //Task<ResponseFromService<string>> GetAllRoutesAsync();
        //Task<ResponseFromService<string>> GetAllCIM10Async();
        //Task<ResponseFromService<string>> GetAllVTMsAsync();
        //Task<ResponseFromService<string>> GetAllATCClassificationsAsync();
        //Task<ResponseFromService<string>> GetAllUCDVsAsync();
        //Task<ResponseFromService<string>> GetAllUCDsAsync();
        //Task<ResponseFromService<string>> GetAllSideEffectsAsync();
    }
}
