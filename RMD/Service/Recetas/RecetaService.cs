using RMD.Data;
using RMD.Extensions; // Asegúrate de tener la extensión ToDataTable()
using RMD.Interface.Notificaciones;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;
using RMD.Models.Responses;

namespace RMD.Service.Recetas
{
    public class RecetaService : IRecetaService
    {
        private readonly RecetasDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly string _connectionString;

        public RecetaService(RecetasDbContext context, ICatalogoNotificacionService catalogoNotificacionService, string connectionString)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
            _connectionString = connectionString;
        }

        public async Task<ResponseFromService<string>> GetRecetaByIdRecetaAsync(Guid idReceta, Guid idPaciente)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Recetas_GetRecetaByIdReceta", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdReceta", idReceta);
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                Receta_PacienteRequest receta = null;
                var detalles = new List<Receta_RecetaDetalleRequest>();
                Receta_FormatoRequest formato = null;

                using var reader = await command.ExecuteReaderAsync();

                // Primer conjunto: Código de Notificación.
                int codigoNotificacion = 0;
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(0);
                }

                // Segundo conjunto: Datos de la receta.
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    receta = Receta_PacienteRequest.FromDataReader(reader);
                }

                // Tercer conjunto: Detalles de la receta.
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detalles.Add(Receta_RecetaDetalleRequest.FromDataReader(reader));
                    }
                }

                // Cuarto conjunto: Formato de la receta.
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    formato = Receta_FormatoRequest.FromDataReader(reader);
                }

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (receta != null && formato != null)
                {
                    var htmlContent = RecetaPdfExtension.GenerarHtmlReceta(receta, detalles, formato);
                    return ResponseFromService<string>.Success(htmlContent, notificacion);
                }

                return ResponseFromService<string>.Failure(notificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, errorNotificacion);
            }
        }

        public async Task<ResponseFromService<IEnumerable<RecetaList>>> GetFilteredRecetasAsync( string roleId, Guid idGEMP, Guid? idSucursal, string? folio, DateTime? startDate, DateTime? endDate, string dateFilter)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Receta_GetFilteredRecetas", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@RoleId", roleId);
                command.Parameters.AddWithValue("@IdGEMP", idGEMP);
                command.Parameters.Add("@IdSucursal", SqlDbType.UniqueIdentifier).Value = idSucursal ?? (object)DBNull.Value;
                command.Parameters.Add("@Folio", SqlDbType.NVarChar, 20).Value = !string.IsNullOrEmpty(folio) ? folio : (object)DBNull.Value;

                if (!string.IsNullOrEmpty(folio))
                {
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = DBNull.Value;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DBNull.Value;
                    command.Parameters.Add("@DateFilter", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                }
                else
                {
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate ?? (object)DBNull.Value;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate ?? (object)DBNull.Value;
                    command.Parameters.Add("@DateFilter", SqlDbType.NVarChar, 50).Value =
                        string.IsNullOrEmpty(dateFilter) ? (object)DBNull.Value : dateFilter;
                }

                using var reader = await command.ExecuteReaderAsync();

                // Primer conjunto: Código de notificación
                int codigoNotificacion = 0;
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(0);
                }

                // Segundo conjunto: Recetas filtradas
                var recetas = new List<RecetaList>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        recetas.Add(RecetaList.FromDataReader(reader));
                    }
                }

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<RecetaList>>.Failure(notificacion);
                }

                return ResponseFromService<IEnumerable<RecetaList>>.Success(recetas, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RecetaList>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<RecetaList>>> GetFilteredRecetasByIdMedicoAsync( Guid idUsuario, Guid idGEMP, Guid? idSucursal, string? folio, DateTime? startDate, DateTime? endDate, string dateFilter)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Receta_GetFilteredRecetasByIdMedico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@IdGEMP", idGEMP);
                command.Parameters.Add("@IdSucursal", SqlDbType.UniqueIdentifier).Value = idSucursal ?? (object)DBNull.Value;

                if (!string.IsNullOrWhiteSpace(folio))
                {
                    command.Parameters.Add("@Folio", SqlDbType.NVarChar, 20).Value = folio;
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = DBNull.Value;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DBNull.Value;
                    command.Parameters.Add("@DateFilter", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                }
                else
                {
                    command.Parameters.Add("@Folio", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate ?? (object)DBNull.Value;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate ?? (object)DBNull.Value;
                    command.Parameters.Add("@DateFilter", SqlDbType.NVarChar, 50).Value =
                        string.IsNullOrEmpty(dateFilter) ? (object)DBNull.Value : dateFilter;
                }

                using var reader = await command.ExecuteReaderAsync();

                // Primer conjunto: Código de notificación
                int codigoNotificacion = 0;
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(0);
                }

                // Segundo conjunto: Listado de recetas
                var recetas = new List<RecetaList>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        recetas.Add(RecetaList.FromDataReader(reader));
                    }
                }

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<RecetaList>>.Failure(notificacion);
                }

                return ResponseFromService<IEnumerable<RecetaList>>.Success(recetas, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RecetaList>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<List<RecetaList>>> GetRecetasByIdPacienteAsync( Guid idUsuario, Guid idPaciente, DateTime? startDate, DateTime? endDate, string dateFilter)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Recetas_GetRecetasByIdPaciente", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IdPaciente", idPaciente);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate ?? (object)DBNull.Value;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate ?? (object)DBNull.Value;
                command.Parameters.Add("@DateFilter", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrEmpty(dateFilter) ? (object)DBNull.Value : dateFilter;

                int codigoNotificacion = 0;
                var recetas = new List<RecetaList>();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(0);
                }

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        recetas.Add(RecetaList.FromDataReader(reader));
                    }
                }

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "SUCCESS" ||
                    notificacion.ToastType.ToUpperInvariant() == "INFO")
                {
                    return ResponseFromService<List<RecetaList>>.Success(recetas, notificacion);
                }
                return ResponseFromService<List<RecetaList>>.Failure(notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<RecetaList>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<Guid?>> GetIdPacienteByUsuarioAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Recetas_GetPacienteByIdUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                int codigoNotificacion = 0;
                Guid? idPaciente = null;

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(0);
                }
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    idPaciente = reader.GetGuid(0);
                }

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "SUCCESS" || notificacion.ToastType.ToUpperInvariant() == "INFO")
                {
                    return ResponseFromService<Guid?>.Success(idPaciente, notificacion);
                }
                return ResponseFromService<Guid?>.Failure(notificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<Guid?>.Exeption(ex, errorNotificacion);
            }
        }

        public async Task<ResponseFromService<string>> GetQRAsync(Guid idReceta, Guid idPaciente)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Recetas_GetQRByIdReceta", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdReceta", idReceta);
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                int codigoNotificacion = 0;
                string qr = string.Empty;

                using var reader = await command.ExecuteReaderAsync();
                // Primer conjunto: Código de notificación
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(0);
                }
                // Segundo conjunto: Valor del QR
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    qr = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                }

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "SUCCESS" ||
                    notificacion.ToastType.ToUpperInvariant() == "INFO")
                {
                    return ResponseFromService<string>.Success(qr, notificacion);
                }
                return ResponseFromService<string>.Failure(notificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, errorNotificacion);
            }
        }

    }
}
