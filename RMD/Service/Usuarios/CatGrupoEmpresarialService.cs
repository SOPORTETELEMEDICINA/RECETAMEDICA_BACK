using RMD.Data;
using RMD.Extensions; // Asegúrate de que ToDataTable() esté implementado
using RMD.Interface.Notificaciones;
using RMD.Interface.Usuarios;
using RMD.Models.Responses;
using RMD.Models.Usuarios;

namespace RMD.Service.Usuarios
{
    public class CatGrupoEmpresarialService : ICatGrupoEmpresarialService
    {
        private readonly UsuariosDBContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly string _connectionString;

        public CatGrupoEmpresarialService(
            UsuariosDBContext context,
            ICatalogoNotificacionService catalogoNotificacionService,
            string connectionString)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
            _connectionString = connectionString;
        }

        public async Task<ResponseFromService<IEnumerable<CatGrupoEmpresarial>>> GetAllGrupoEmpresarialAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_GetAllGrupoEmpresarial", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Failure(notificacion);
                }

                var grupos = new List<CatGrupoEmpresarial>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        grupos.Add(CatGrupoEmpresarial.FromDataReader((SqlDataReader)reader));
                    }
                }

                return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Success(grupos, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatGrupoEmpresarial>> GetGrupoEmpresarialByIdAsync(Guid id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_GetGrupoEmpresarialById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@IdGEMP", id));

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<CatGrupoEmpresarial>.Failure(notificacion);
                }

                CatGrupoEmpresarial grupo = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    grupo = CatGrupoEmpresarial.FromDataReader((SqlDataReader)reader);
                }
                return ResponseFromService<CatGrupoEmpresarial>.Success(grupo, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatGrupoEmpresarial>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatGrupoEmpresarial>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> CreateGrupoEmpresarialAsync(CatGrupoEmpresarial grupoEmpresarial)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_CreateGrupoEmpresarial", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var parameter = new SqlParameter("@GrupoEmpresarial", SqlDbType.Structured)
                {
                    TypeName = "dbo.CatGrupoEmpresarialType",
                    Value = new List<CatGrupoEmpresarial> { grupoEmpresarial }.ToDataTable()
                };
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = 0;
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(0);
                }

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<string>.Failure(notificacion);
                }

                // Intentar leer el ID generado (si es que se retorna)
                Guid generatedId = Guid.Empty;
                if (await reader.ReadAsync())
                {
                    generatedId = reader.GetGuid(0);
                }
                return ResponseFromService<string>.Success($"Grupo empresarial ({generatedId}) creado exitosamente.", notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateGrupoEmpresarialAsync(CatGrupoEmpresarial grupoEmpresarial)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_UpdateGrupoEmpresarial", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var parameter = new SqlParameter("@GrupoEmpresarial", SqlDbType.Structured)
                {
                    TypeName = "dbo.CatGrupoEmpresarialType",
                    Value = new List<CatGrupoEmpresarial> { grupoEmpresarial }.ToDataTable()
                };
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = 0;
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(0);
                }
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                return notificacion.ToastType.ToUpperInvariant() == "ERROR"
                    ? ResponseFromService<string>.Failure(notificacion)
                    : ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
    }
}
