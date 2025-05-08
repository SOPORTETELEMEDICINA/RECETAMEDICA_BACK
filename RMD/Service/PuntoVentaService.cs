using RMD.Data;
using RMD.Interface.Notificaciones;
using RMD.Interface.PuntoVenta;
using RMD.Models.PuntoVenta;
using RMD.Models.Responses;

namespace RMD.Service
{
    public class PuntoVentaService : IPuntoVentaService
    {
        private readonly PuntoVentaDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly string _connectionString;

        public PuntoVentaService(PuntoVentaDbContext context, ICatalogoNotificacionService catalogoNotificacionService, string connectionString)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
            _connectionString = connectionString;
        }

        

        public async Task<ResponseFromService<PuntoVentaRecetaResponse>> ObtenerRecetaAsync(Guid idReceta, Guid idMedico, DateTime fechaUltimaModificacion)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand("PuntoVenta_GetReceta", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("@IdReceta", idReceta);
                command.Parameters.AddWithValue("@IdMedico", idMedico);
                command.Parameters.AddWithValue("@FechaUltimaModificacion", fechaUltimaModificacion);
                PuntoVentaRecetaModel recetaModel = null;
                List<DetalleRecetaModel> detalles = new List<DetalleRecetaModel>();
                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    recetaModel = PuntoVentaRecetaModel.FromDataReader(reader);
                }
                if (recetaModel == null)
                {
                    var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                    return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notificacion);
                }
                using var commandDetalles = new SqlCommand("PuntoVenta_GetDetalleReceta", connection) { CommandType = CommandType.StoredProcedure };
                commandDetalles.Parameters.AddWithValue("@IdReceta", recetaModel.IdReceta);
                using (var readerDetalles = await commandDetalles.ExecuteReaderAsync())
                {
                    while (await readerDetalles.ReadAsync())
                    {
                        detalles.Add(DetalleRecetaModel.FromDataReader(readerDetalles));
                    }
                }
                var notificacionFinal = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                var responseData = new PuntoVentaRecetaResponse { Receta = recetaModel, Detalles = detalles };
                return ResponseFromService<PuntoVentaRecetaResponse>.Success(responseData, notificacionFinal);
            }
            catch (SqlException sqlEx)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "ERROR_SURTIDO");
                return ResponseFromService<PuntoVentaRecetaResponse>.Exeption(sqlEx, notificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "ERROR_SURTIDO");
                return ResponseFromService<PuntoVentaRecetaResponse>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<string>> SurtirMedicamentosAsync(Guid idReceta, List<Guid> detallesReceta)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("PuntoVenta_SurtirMedicamentos", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IdReceta", idReceta);

                var detalleTable = new DataTable();
                detalleTable.Columns.Add("Id", typeof(Guid));
                detallesReceta.ForEach(id => detalleTable.Rows.Add(id));

                var detallesParam = new SqlParameter("@DetallesReceta", SqlDbType.Structured)
                {
                    TypeName = "dbo.GuidList",
                    Value = detalleTable
                };
                command.Parameters.Add(detallesParam);

                using var reader = await command.ExecuteReaderAsync();

                // Primer conjunto: Código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                // Retornar respuesta exitosa utilizando el SP de notificaciones
                return ResponseFromService<string>.Success("Medicamentos surtidos", notificacion);
            }
            catch (SqlException sqlEx)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "ERROR_SURTIDO");
                return ResponseFromService<string>.Exeption(sqlEx, errorNotificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "ERROR_SURTIDO");
                return ResponseFromService<string>.Exeption(ex, errorNotificacion);
            }
        }


        public async Task<ResponseFromService<PuntoVentaRecetaResponse>> ConsultarRecetaPorIdAsync(string folio)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                // Ejecutar el SP para consultar la receta por folio
                using var command = new SqlCommand("PuntoVenta_ConsultarPorId", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@Folio", folio);

                PuntoVentaRecetaModel recetaModel = null;
                int codigoNotificacion = 0;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Leer el primer conjunto para obtener el código de notificación
                    codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                    var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                    if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                        return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notificacion);

                    // Leer el segundo conjunto para obtener los datos de la receta
                    if (await reader.NextResultAsync() && await reader.ReadAsync())
                    {
                        recetaModel = PuntoVentaRecetaModel.FromDataReader(reader);
                    }
                    else
                    {
                        // Si no se obtuvo data, retornar fallo
                        return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notificacion);
                    }
                }
                CatalogoNotificacion notificacionDetalles;
                // Ejecutar el SP para obtener los detalles de la receta
                List<DetalleRecetaModel> detalles = new List<DetalleRecetaModel>();
                using (var commandDetalles = new SqlCommand("PuntoVenta_GetDetalleReceta", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    commandDetalles.Parameters.AddWithValue("@IdReceta", recetaModel.IdReceta);
                    using var readerDetalles = await commandDetalles.ExecuteReaderAsync();
                    // Leer el primer conjunto para el código de notificación
                    int codigoNotificacionDetalles = await ValidationHelper.ReadErrorCodeAsync(readerDetalles);
                    notificacionDetalles = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacionDetalles);
                    if (notificacionDetalles.ToastType.ToUpperInvariant() == "ERROR")
                        return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notificacionDetalles);

                    // Leer los detalles en el segundo conjunto
                    if (await readerDetalles.NextResultAsync())
                    {
                        while (await readerDetalles.ReadAsync())
                        {
                            detalles.Add(DetalleRecetaModel.FromDataReader(readerDetalles));
                        }
                    }
                }
                var responseData = new PuntoVentaRecetaResponse
                {
                    Receta = recetaModel,
                    Detalles = detalles
                };
                return ResponseFromService<PuntoVentaRecetaResponse>.Success(responseData, notificacionDetalles);
            }
            catch (SqlException sqlEx)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PuntoVentaRecetaResponse>.Exeption(sqlEx, errNotificacion);
            }
            catch (Exception ex)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PuntoVentaRecetaResponse>.Exeption(ex, errNotificacion);
            }
        }

    }
}
