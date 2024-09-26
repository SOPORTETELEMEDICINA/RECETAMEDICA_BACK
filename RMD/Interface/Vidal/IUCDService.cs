using RMD.Models.Vidal.ByUCD;

namespace RMD.Interface.Vidal
{
    public interface IUCDService
    {
        Task<List<Ucd>> GetAllUcdsAsync();
        Task<UCDByIdPackage> GetUcdByIdPackageAsync(int packageId);
        Task<UCDById> GetUcdByIdAsync(int ucdId);
        Task<List<UcdPackage>> GetUcdPackagesByIdAsync(int ucdId);
        Task<List<UcdProduct>> GetUcdProductsByIdAsync(int ucdId);
        Task<List<UCDSideEffect>> GetSideEffectsByUcdIdAsync(int ucdId);
    }
}
