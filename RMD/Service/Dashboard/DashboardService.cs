using Microsoft.Data.SqlClient;
using RMD.Interface.Dashboard;
using RMD.Interface.Security;
using RMD.Shared.Models.Dashboard;

namespace RMD.Service.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;

        public DashboardService(ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService)
        {
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }
        public async Task<ResponseFromService<IEnumerable<SucursalPacientes>>> GetSucursalesPacientesAsync(
            Guid idGemp,
            Guid idSucursal,
            Guid idTipoUsuario)
        {
            try
            {
                var parameters = new
                {
                    IdGEMP = idGemp,
                    IdSucursal = idSucursal,
                    IdTipoUsuario = idTipoUsuario
                };

                using var multi = await _dapperService.QueryMultipleAsync("Dashboard_CountPacientesBySucursal", parameters);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<SucursalPacientes>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<SucursalPacientes>>.Success(new List<SucursalPacientes>(), notificacion);

                var lista = multi.Read<SucursalPacientes>().ToList();

                return ResponseFromService<IEnumerable<SucursalPacientes>>.Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<SucursalPacientes>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<SucursalPacientes>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>> GetKPIPacientesRecetasAsync(
            Guid idUsuario,
            Guid idRol)
        {
            try
            {
                var parameters = new
                {
                    IdUsuario = idUsuario,
                    IdTipoUsuario = idRol
                };

                using var multi = await _dapperService.QueryMultipleAsync("Dashboard_GetPacientesRecetasByIdUsuario", parameters);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>.Success(new List<DashBoardKPIPacientesRecetas>(), notificacion);

                var lista = multi.Read<DashBoardKPIPacientesRecetas>().ToList();

                return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>.Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>.Exeption(ex, error);
            }
        }
    }
}
