using RMD.Models.Dashboard;

namespace RMD.Interface.Dashborad
{
    public interface IDashboardService
    {
        Task<IEnumerable<SucursalPacientes>> GetSucursalesPacientesAsync(Guid idGemp, Guid idSucursal, Guid idTipoUsuario);

        Task<List<DashBoardKPIPacientesRecetas>> GetKPIPacientesRecetas(Guid idUsuario, Guid idRol);
    }
}
