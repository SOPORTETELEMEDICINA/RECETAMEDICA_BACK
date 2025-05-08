using RMD.Data;
using RMD.Interface.Dashboard;
using RMD.Interface.Notificaciones;
using RMD.Models.Dashboard;
using RMD.Models.Responses;

namespace RMD.Service.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly DashboardDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public DashboardService(
            DashboardDbContext context,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        public async Task<ResponseFromService<IEnumerable<SucursalPacientes>>> GetSucursalesPacientesAsync(
            Guid idGemp,
            Guid idSucursal,
            Guid idTipoUsuario)
        {
            try
            {
                // 1) Abrir conexión y preparar comando
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Dashboard_CountPacientesBySucursal", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdGEMP", idGemp);
                command.Parameters.AddWithValue("@IdSucursal", idSucursal);
                command.Parameters.AddWithValue("@IdTipoUsuario", idTipoUsuario);

                // 2) Ejecutar y leer primer conjunto: Código de notificación
                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<SucursalPacientes>>.Failure(notificacion);
                }

                // 3) Leer segundo conjunto: resultados de sucursales + conteo
                var lista = new List<SucursalPacientes>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new SucursalPacientes
                        {
                            IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                            IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                            NombreSucursal = reader.GetString(reader.GetOrdinal("NombreSucursal")),
                            TotalPacientes = reader.GetInt32(reader.GetOrdinal("TotalPacientes"))
                        });
                    }
                }

                return ResponseFromService<IEnumerable<SucursalPacientes>>
                    .Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<SucursalPacientes>>
                    .Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<SucursalPacientes>>
                    .Exeption(ex, error);
            }
        }


        public async Task<ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>> GetKPIPacientesRecetasAsync(
     Guid idUsuario,
     Guid idRol)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Dashboard_GetPacientesRecetasByIdUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@IdTipoUsuario", idRol);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>.Failure(notificacion);

                // 2) Leer segundo conjunto: KPI de pacientes y recetas
                var lista = new List<DashBoardKPIPacientesRecetas>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new DashBoardKPIPacientesRecetas
                        {
                            IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                            IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                            CountPacientes = reader.GetInt32(reader.GetOrdinal("CountPacientes")),
                            CountRecetas = reader.GetInt32(reader.GetOrdinal("CountRecetas"))
                        });
                    }
                }

                return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>
                    .Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>
                    .Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<DashBoardKPIPacientesRecetas>>
                    .Exeption(ex, error);
            }
        }

    }
}
