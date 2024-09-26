using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Dashborad;
using RMD.Models.Dashboard;

namespace RMD.Service.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly DashboardDbContext _context;

        public DashboardService(DashboardDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SucursalPacientes>> GetSucursalesPacientesAsync(Guid idGemp, Guid idSucursal, Guid idTipoUsuario)
        {
            var sucursalesPacientes = new List<SucursalPacientes>();

            var parameters = new[]
            {
                new SqlParameter("@IdGEMP", idGemp),
                new SqlParameter("@IdSucursal", idSucursal),
                new SqlParameter("@IdTipoUsuario", idTipoUsuario)
            };

            sucursalesPacientes = await _context.SucursalPacientes
                .FromSqlRaw("EXEC Dashboard_CountPacientesBySucursal @IdGEMP, @IdSucursal, @IdTipoUsuario", parameters)
                .ToListAsync();

            return sucursalesPacientes;
        }

        public async Task<List<DashBoardKPIPacientesRecetas>> GetKPIPacientesRecetas(Guid idUsuario, Guid idRol)
        {
            var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);
            var tipoUsuarioParam = new SqlParameter("@IdTipoUsuario", idRol);

            var result = await _context.MedicoKPIPacientesRecetas
                .FromSqlRaw("EXEC Dashboard_GetPacientesRecetasByIdUsuario @IdUsuario, @IdTipoUsuario", idUsuarioParam, tipoUsuarioParam)
                .AsNoTracking()
                .ToListAsync();

            return result;
        }

    }
}
