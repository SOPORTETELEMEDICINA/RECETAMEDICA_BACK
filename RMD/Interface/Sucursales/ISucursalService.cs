using RMD.Models.Sucursales;

namespace RMD.Interface.Sucursales
{
    public interface ISucursalService
    {
        Task<bool> CreateSucursalAsync(CreateSucursalModel model);
        Task<bool> UpdateSucursalAsync(Guid idSucursal, UpdateSucursalModel model);
        Task<bool> DeleteSucursalAsync(Guid idSucursal);
        Task<SucursalRequest> GetSucursalByIdSucursalAsync(Guid idSucursal);
        Task<IEnumerable<SucursalRequest>> GetSucursalesByIdGEMPAsync(Guid idGEMP);
        Task<IEnumerable<SucursalRequest>> GetSucursalesByIdGEMPAndIdAsentamientoAsync(Guid idGEMP, int idAsentamiento);

    }

}
