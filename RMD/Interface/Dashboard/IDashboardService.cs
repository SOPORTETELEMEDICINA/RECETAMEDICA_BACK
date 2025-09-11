using RMD.Shared.Models.Dashboard;

namespace RMD.Interface.Dashboard
{
    public interface IDashboardService
    {
        /// <summary>
        /// Obtiene el conteo de pacientes por sucursal.
        /// </summary>
        /// <param name="idGEMP">Identificador del grupo empresarial.</param>
        /// <param name="idSucursal">Identificador de la sucursal.</param>
        /// <param name="idTipoUsuario">Identificador del tipo de usuario.</param>
        Task<ResponseFromService<IEnumerable<SucursalPacientes>>> GetSucursalesPacientesAsync(
            Guid idGEMP,
            Guid idSucursal,
            Guid idTipoUsuario);

        /// <summary>
        /// Obtiene los KPIs de pacientes y recetas para un usuario dado.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario (médico).</param>
        /// <param name="idTipoUsuario">Identificador del tipo de usuario.</param>
        Task<ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>> GetKPIPacientesRecetasAsync(
            Guid idUsuario,
            Guid idTipoUsuario);
    }
}
