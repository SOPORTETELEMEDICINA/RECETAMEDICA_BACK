using RMD.Models.Consulta;
using RMD.Models.Vidal.ByPackage;
using System.Threading.Tasks;

namespace RMD.Interface.Vidal
{
    public interface IPackageService
    {
        Task<List<Package>> GetAllPackagesAsync();
        Task<PackageById> GetPackageByIdAsync(int packageId);
        Task<List<PackageRoute>> GetPackageRoutesAsync(int packageId);
        Task<List<PackageIndicator>> GetPackageIndicatorsAsync(int packageId);
        Task<List<PackageUnit>> GetPackageUnitsByPackageIdAsync(int packageId);
        Task<List<PackageIndication>> GetPackageIndicationsByPackageIdAsync(int packageId);
        Task<List<PackageSideEffect>> GetPackageSideEffectsAsync(int packageId);
        Task<List<PackageAtcClassification>> GetPackageAtcClassificationsAsync(int packageId);
        Task<PackageVmp> GetVmpByPackageIdAsync(int packageId);
        Task<PackageProduct> GetPackageProductByIdAsync(string packageId);
        Task<List<Package>> GetPackagesByNameAsync(string packageName);
        Task<List<PackageUnit>> GetPackageUnitsByLinkAsync(string link);

        Task<List<PackageEntry>> GetPackagesByName(string name);


    }
}
