using RMD.Models.Sucursales;

namespace RMD.Interface.Sucursales
{
    public interface ISucursalService
    {
        Task<Sucursal> GetSucursalByIdAsync(Guid id);
        Task<IEnumerable<Sucursal>> GetSucursalesByGEMPAsync(Guid idGEMP);
        Task<IEnumerable<Sucursal>> GetSucursalesByAsentamientoAsync(int idAsentamiento);
    }

}
