using RMD.Models.Responses;
using RMD.Models.Sucursales;

namespace RMD.Interface.Sucursales
{
    public interface ISucursalService
    {
        Task<ResponseFromService<bool>> CreateSucursalAsync(CreateSucursalModel model);
        Task<ResponseFromService<bool>> UpdateSucursalAsync(Guid idSucursal, UpdateSucursalModel model);
        Task<ResponseFromService<bool>> DeleteSucursalAsync(Guid idSucursal);
        Task<ResponseFromService<SucursalRequest>> GetSucursalByIdSucursalAsync(Guid idSucursal);
        Task<ResponseFromService<IEnumerable<SucursalRequest>>> GetSucursalesByIdGEMPAsync(Guid idGEMP);
        //Task<ResponseFromService<IEnumerable<SucursalRequest>>> GetSucursalesByIdGEMPAndIdAsentamientoAsync(Guid idGEMP, int idAsentamiento);

    }

}
