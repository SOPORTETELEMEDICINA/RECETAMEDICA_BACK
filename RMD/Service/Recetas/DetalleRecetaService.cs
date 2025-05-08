using RMD.Data;
using RMD.Interface.Notificaciones;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;
using RMD.Models.Responses;

namespace RMD.Service.Recetas
{
    public class DetalleRecetaService : IDetalleRecetaService
    {
        private readonly RecetasDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly string _connectionString;

        public DetalleRecetaService(
            RecetasDbContext context,
            ICatalogoNotificacionService catalogoNotificacionService,
            string connectionString)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
            _connectionString = connectionString;
        }

        /// <summary>
        /// Crea o actualiza la reacción (respuesta) de un paciente para un detalle de receta mediante un SP.
        /// Se utiliza la lectura del primer conjunto para el código de notificación y el siguiente para el mensaje.
        /// </summary>
        public async Task<ResponseFromService<string>> CreateUpdateReaccionAsync(DetalleRecetaRequest request, Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DetalleReceta_CreateUpdatePacienteReaccion", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdDetalleReceta", request.IdDetalleReceta);
                command.Parameters.AddWithValue("@IdReceta", request.IdReceta);
                command.Parameters.AddWithValue("@MedicamentoId", request.MedicamentoId);
                command.Parameters.AddWithValue("@MedicamentoType", request.MedicamentoType);
                command.Parameters.AddWithValue("@Descripcion", request.Descripcion);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                // Como ya quitamos el parámetro de salida, no se retorna ningún mensaje adicional.
                // Se retorna la descripción de la notificación obtenida.
                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, errNotificacion);
            }
            catch (Exception ex)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, errNotificacion);
            }
        }


        /// <summary>
        /// Elimina la reacción (respuesta) de un paciente para un detalle de receta mediante un SP.
        /// Se lee el primer conjunto para el código de notificación y luego el mensaje.
        /// </summary>
        public async Task<ResponseFromService<string>> DeleteReaccionAsync(DetalleRecetaRequest request, Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DetalleReceta_DeletePacienteReaccion", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IdDetalleReceta", request.IdDetalleReceta);
                command.Parameters.AddWithValue("@IdReceta", request.IdReceta);
                command.Parameters.AddWithValue("@MedicamentoId", request.MedicamentoId);
                command.Parameters.AddWithValue("@MedicamentoType", request.MedicamentoType);
                command.Parameters.AddWithValue("@Descripcion", request.Descripcion);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                // Con el nuevo SP, ya no se retorna un mensaje adicional, se usa la descripción de la notificación.
                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, errNotificacion);
            }
            catch (Exception ex)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, errNotificacion);
            }
        }


        /// <summary>
        /// Obtiene los detalles (detalle reducido) para una receta mediante un SP.
        /// Se lee el primer conjunto para el código de notificación y luego la lista de detalles.
        /// </summary>
        public async Task<ResponseFromService<List<Detalle_RecetaDetalleRequest>>> GetDetallesByIdReceta(Guid idUsuario, Guid idReceta)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DetalleReceta_GetDetallesByIdReceta", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@IdReceta", idReceta);

                var detalles = new List<Detalle_RecetaDetalleRequest>();

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<List<Detalle_RecetaDetalleRequest>>.Failure(notificacion);

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detalles.Add(Detalle_RecetaDetalleRequest.FromDataReader(reader));
                    }
                }

                if (detalles == null || detalles.Count == 0)
                    return ResponseFromService<List<Detalle_RecetaDetalleRequest>>.Failure(notificacion);

                return ResponseFromService<List<Detalle_RecetaDetalleRequest>>.Success(detalles, notificacion);
            }
            catch (Exception ex)
            {
                var errNotificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<Detalle_RecetaDetalleRequest>>.Exeption(ex, errNotificacion);
            }
        }


        /// <summary>
        /// Obtiene únicamente los medicamentos activos para un paciente mediante un SP.
        /// Primero se obtiene el IdPaciente y luego se ejecuta el SP que retorna el listado.
        /// </summary>
        public async Task<ResponseFromService<IEnumerable<MedicamentoActivoConsulta>>> GetSoloMedicamentosActivos(Guid idUsuario)
        {
            try
            {
                // Obtener el IdPaciente asociado al IdUsuario
                Guid idPaciente;
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT IdPaciente FROM Pacientes WITH (NOLOCK) WHERE IdUsuario = @IdUsuario", connection))
                    {
                        command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        var result = await command.ExecuteScalarAsync();
                        if (result == null)
                        {
                            // Si no se encuentra el paciente, se retorna la notificación correspondiente
                            var notificacion = await _catalogoNotificacionService
                                .GetNotificationByTipoAndFuncionAsync("RECETASSP", "PACIENTE_NO_ENCONTRADO");
                            return ResponseFromService<IEnumerable<MedicamentoActivoConsulta>>.Failure(notificacion);
                        }
                        idPaciente = (Guid)result;
                    }
                }

                var medicamentos = new List<MedicamentoActivoConsulta>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("MedicamentoActivo_GetByPaciente", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Leer el primer conjunto: Código de notificación
                            int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                            var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                            if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                                return ResponseFromService<IEnumerable<MedicamentoActivoConsulta>>.Failure(notificacion);

                            // Leer el segundo conjunto: Datos de los medicamentos activos
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    medicamentos.Add(MedicamentoActivoConsulta.FromDataReader(reader));
                                }
                            }

                            if (medicamentos == null || medicamentos.Count == 0)
                            {
                                var noMedicamentoNotificacion = await _catalogoNotificacionService
                                    .GetNotificationByTipoAndFuncionAsync("MEDACTSP", "MEDICAMENTO_ACTIVO_NO_ENCONTRADO");
                                return ResponseFromService<IEnumerable<MedicamentoActivoConsulta>>.Failure(noMedicamentoNotificacion);
                            }

                            return ResponseFromService<IEnumerable<MedicamentoActivoConsulta>>.Success(medicamentos, notificacion);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicamentoActivoConsulta>>.Exeption(sqlEx, errorNotificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicamentoActivoConsulta>>.Exeption(ex, errorNotificacion);
            }
        }

    }
}
