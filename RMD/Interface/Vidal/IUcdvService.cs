using RMD.Models.Vidal.ByUCD;
using RMD.Models.Vidal.ByUCDV;

namespace RMD.Interface.Vidal
{
    public interface IUcdvService
    {
        Task<List<UCDVS>> GetAllUcdvsAsync();
        Task<UCDV> GetUcdvByIdAsync(int id);
        Task<List<UCDVRoute>> GetRoutesForUcdvAsync(int ucdvId);
        Task<List<UCDVUnit>> GetUnitsForUcdvAsync(int ucdvId); 
        Task<List<UCDVPackage>> GetUcdvPackagesAsync(int ucdvId);
        Task<List<UCDVProduct>> GetUcdvProductsAsync(int ucdvId);
        Task<List<UCDVMolecule>> GetMoleculesByUCDVIdAsync(int ucdvId);
    }
}
