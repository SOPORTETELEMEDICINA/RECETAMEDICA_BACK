using RMD.Models.Sucursales;

namespace RMD.Interface.Sucursales
{
    public interface ISucursalService
    {
        Task<bool> CreateSucursalAsync(CreateSucursalModel model);
        Task<bool> UpdateSucursalAsync(Guid idSucursal, UpdateSucursalModel model);
        Task<bool> DeleteSucursalAsync(Guid idSucursal);
        Task<SucursalDomicilioModel> GetSucursalByIdSucursalAsync(Guid idSucursal);
        Task<IEnumerable<SucursalDomicilioModel>> GetSucursalesByIdGEMPAsync(Guid idGEMP);
        Task<IEnumerable<SucursalDomicilioModel>> GetSucursalesByIdGEMPAndIdAsentamientoAsync(Guid idGEMP, int idAsentamiento);

    }

}
