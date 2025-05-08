using System;
using Newtonsoft.Json;
using RMD.Data;
using RMD.Interface.Notificaciones;
using RMD.Interface.Pacientes;
using RMD.Models.Pacientes;
using RMD.Models.Responses;

namespace RMD.Service.Pacientes
{
    public class PacienteService(PacientesDbContext context,
           ICatalogoNotificacionService catalogoNotificacionService) : IPacienteService
    {
        private readonly PacientesDbContext _context = context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService = catalogoNotificacionService;

        public async Task<ResponseFromService<PacienteConsultaRequest>> GetPacienteByIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Paciente_GetPacienteByIdUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<PacienteConsultaRequest>.Failure(spNotificacion);

                PacienteConsultaRequest paciente = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    // Se asume que UsuarioPaciente.FromDataReader(reader) mapea el registro
                    var usuarioPaciente = UsuarioPaciente.FromDataReader(reader);
                    paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                }
                return ResponseFromService<PacienteConsultaRequest>.Success(paciente, spNotificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PacienteConsultaRequest>.Exeption(ex, notificacion);
            }
        }
        
        public async Task<ResponseFromService<PacienteConsultaRequest>> GetPacienteByIdPacienteAsync(Guid idPaciente)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Paciente_GetPacienteByIdPaciente", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@IdPaciente", idPaciente));

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<PacienteConsultaRequest>.Failure(spNotificacion);

                PacienteConsultaRequest paciente = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    var usuarioPaciente = UsuarioPaciente.FromDataReader(reader);
                    paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                }
                return ResponseFromService<PacienteConsultaRequest>.Success(paciente, spNotificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PacienteConsultaRequest>.Exeption(ex, notificacion);
            }
        }

        //public async Task<ResponseFromService<IEnumerable<Paciente>>> GetPacientesByEntidadNacimientoAsync(int idEntidadNacimiento)
        //{
        //    try
        //    {
        //        // Aunque el SP no se usa aquí, se recomienda estandarizar la consulta usando ADO.NET.
        //        using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
        //        await connection.OpenAsync();
        //        using var command = new SqlCommand("Paciente_GetPacientesByEntidadNacimiento", connection)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        command.Parameters.Add(new SqlParameter("@IdEntidadNacimiento", idEntidadNacimiento));

        //        using var reader = await command.ExecuteReaderAsync();
        //        int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
        //        var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
        //        if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
        //            return ResponseFromService<IEnumerable<Paciente>>.Failure(spNotificacion);

        //        var lista = new List<Paciente>();
        //        if (await reader.NextResultAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                // Se asume que existe un método de mapeo en Paciente.FromDataReader(reader)
        //                lista.Add(Paciente.FromDataReader(reader));
        //            }
        //        }
        //        return ResponseFromService<IEnumerable<Paciente>>.Success(lista, spNotificacion);
        //    }
        //    catch (Exception ex)
        //    {
        //        var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
        //        return ResponseFromService<IEnumerable<Paciente>>.Exeption(ex, notificacion);
        //    }
        //}

        public async Task<ResponseFromService<IEnumerable<PacienteConsultaRequest>>> GetPacienteByNameAsync(string nombreBusqueda, Guid idGemp)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Paciente_GetPacienteByName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@NombreBusqueda", nombreBusqueda);
                command.Parameters.AddWithValue("@IdGemp", idGemp);

                using var reader = await command.ExecuteReaderAsync();

                // Leer el primer conjunto para obtener el código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Failure(spNotificacion);

                // Leer el segundo conjunto: Lista de pacientes
                var lista = new List<PacienteConsultaRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Se obtiene el objeto UsuarioPaciente a partir del lector
                        var usuarioPaciente = UsuarioPaciente.FromDataReader(reader);
                        // Mapear el objeto UsuarioPaciente a PacienteConsultaRequest usando el helper de mapeo
                        var paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                        lista.Add(paciente);
                    }
                }

                return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(lista, spNotificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Exeption(ex, errorNotificacion);
            }
        }

        public async Task<ResponseFromService<bool>> CreatePacienteAsync(PacienteCreateConListas pacienteRequest, Guid idUsuarioSolicitante)
        {
            try
            {
                int result = await EjecutarStoredProcedureAsync("Pacientes_CrearPaciente", pacienteRequest, idUsuarioSolicitante);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(result);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<bool>.Failure(spNotificacion);

                return ResponseFromService<bool>.Success(true, spNotificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }


        public async Task<ResponseFromService<bool>> UpdatePacienteAsync(PacienteConListas pacienteRequest, Guid idUsuarioSolicitante)
        {
            try
            {
                int codigoNotificacion = await EjecutarStoredProcedureAsync("Pacientes_UpdatePaciente", pacienteRequest, idUsuarioSolicitante);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                
                if (spNotificacion.ToastType.ToUpperInvariant() == "SUCCESS" ||
                    spNotificacion.ToastType.ToUpperInvariant() == "INFO")
                {
                    return ResponseFromService<bool>.Success(true, spNotificacion);
                }
                else
                {
                    return ResponseFromService<bool>.Failure(spNotificacion);
                }
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<bool>> EliminarPacienteAsync(Guid idPaciente, Guid idUsuarioSolicitante)
        {
            try
            {
                // Ejecuta el SP y obtiene el código de notificación devuelto.
                int codigo = await EjecutarStoredProcedureAsync("Pacientes_EliminarPaciente", idPaciente, idUsuarioSolicitante);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigo);

                if (notificacion.ToastType.ToUpperInvariant() == "SUCCESS" ||
                    notificacion.ToastType.ToUpperInvariant() == "INFO")
                {
                    return ResponseFromService<bool>.Success(true, notificacion);
                }
                else
                {
                    return ResponseFromService<bool>.Failure(notificacion);
                }
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, errorNotificacion);
            }
        }

        public async Task<ResponseFromService<IEnumerable<EntidadNacimiento>>> GetEntidadesFederativasAsync()
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand("Paciente_GetEntidadesFederativas", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<EntidadNacimiento>>.Failure(spNotificacion);

                var lista = new List<EntidadNacimiento>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Se asume que existe EntidadNacimiento.FromDataReader(reader)
                        lista.Add(EntidadNacimiento.FromDataReader(reader));
                    }
                }
                return ResponseFromService<IEnumerable<EntidadNacimiento>>.Success(lista, spNotificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<EntidadNacimiento>>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<IEnumerable<PacienteConsultaRequest>>> GetPacientesBySucursalAsync(Guid idSucursal)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Paciente_GetPacientesBySucursal", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdSucursal", idSucursal);

                using var reader = await command.ExecuteReaderAsync();

                // Primer conjunto: Código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Failure(spNotificacion);

                // Segundo conjunto: Lista de pacientes
                var lista = new List<PacienteConsultaRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var usuarioPaciente = UsuarioPaciente.FromDataReader(reader);
                        // Mapear el objeto UsuarioPaciente a PacienteConsultaRequest utilizando el helper de mapeo
                        var paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                        lista.Add(paciente);
                    }
                }

                return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(lista, spNotificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Exeption(ex, errorNotificacion);
            }
        }

        public async Task<ResponseFromService<IEnumerable<PacienteConsultaRequest>>> GetPacientesByGEMPAsync(Guid idGemp)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Paciente_GetPacientesByGEMP", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdGemp", idGemp);

                using var reader = await command.ExecuteReaderAsync();

                // Leer el primer conjunto: Código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Failure(spNotificacion);

                // Leer el segundo conjunto: Lista de pacientes
                var lista = new List<PacienteConsultaRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Mapear el registro a un objeto UsuarioPaciente
                        var usuarioPaciente = UsuarioPaciente.FromDataReader(reader);
                        // Convertir el usuarioPaciente en un PacienteConsultaRequest usando el helper de mapeo
                        var paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                        lista.Add(paciente);
                    }
                }

                return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(lista, spNotificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Exeption(ex, errorNotificacion);
            }
        }

        public async Task<ResponseFromService<IEnumerable<PacienteConsultaRequest>>> GetPacientesByMedicoAsync(Guid idMedico)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Paciente_GetPacientesByMedico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdMedico", idMedico);

                using var reader = await command.ExecuteReaderAsync();

                // Primer conjunto: leer el código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Failure(spNotificacion);

                // Segundo conjunto: leer la lista de pacientes
                var lista = new List<PacienteConsultaRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var usuarioPaciente = UsuarioPaciente.FromDataReader(reader);
                        // Mapear el usuarioPaciente a un objeto de tipo PacienteConsultaRequest usando el helper de mapeo
                        var paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                        lista.Add(paciente);
                    }
                }

                return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(lista, spNotificacion);
            }
            catch (Exception ex)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Exeption(ex, errNotificacion);
            }
        }


        public async Task<ResponseFromService<IEnumerable<EventosSaludConsulta>>> GetAllEventosPacienteAsync(Guid idPaciente)
        {
            try
            {
                var eventos = new List<EventosSaludConsulta>();
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("EventosSalud_GetEventosByPaciente", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@IdPaciente", idPaciente));

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<EventosSaludConsulta>>.Failure(spNotificacion);

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        eventos.Add(new EventosSaludConsulta
                        {
                            IdEventoSalud = reader.GetGuid(reader.GetOrdinal("IdEventoSalud")),
                            IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                            Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                            EventoDeSalud = reader.GetInt32(reader.GetOrdinal("EventoDeSalud")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? string.Empty : reader.GetString(reader.GetOrdinal("Descripcion")),
                            NombreEvento = reader.IsDBNull(reader.GetOrdinal("NombreEvento")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreEvento"))
                        });
                    }
                }
                return ResponseFromService<IEnumerable<EventosSaludConsulta>>.Success(eventos, spNotificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<EventosSaludConsulta>>.Exeption(ex, notificacion);
            }
        }

        // Devuelve el evento de salud correspondiente al idEventoSalud.
        public async Task<ResponseFromService<EventosSaludConsulta>> GetEventoPacienteByIdAsync(Guid idEventoSalud)
        {
            try
            {
                var evento = await (from ev in _context.EventosSalud
                                    join cat in _context.CatEventosDeSalud on ev.EventoDeSalud equals cat.IdEvento
                                    where ev.IdEventoSalud == idEventoSalud
                                    select new EventosSaludConsulta
                                    {
                                        IdEventoSalud = ev.IdEventoSalud,
                                        IdPaciente = ev.IdPaciente,
                                        Fecha = ev.Fecha,
                                        EventoDeSalud = ev.EventoDeSalud,
                                        Descripcion = ev.Descripcion,
                                        NombreEvento = cat.NombreEvento
                                    }).FirstOrDefaultAsync();

                if (evento == null)
                {
                    var notificacion = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "NO_EVENTOS_ENCONTRADOS");
                    return ResponseFromService<EventosSaludConsulta>.Failure(notificacion);
                }

                var success = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "EVENTOS_ENCONTRADOS");
                return ResponseFromService<EventosSaludConsulta>.Success(evento, success);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<EventosSaludConsulta>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<EventosSalud>> CreateEventoPacienteAsync(EventosSalud evento)
        {
            try
            {
                _context.EventosSalud.Add(evento);
                var affectedRows = await _context.SaveChangesAsync();
                if (affectedRows > 0)
                {
                    var success = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "CREADO");
                    return ResponseFromService<EventosSalud>.Success(evento, success);
                }
                else
                {
                    var failure = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "NO_CREADO");
                    return ResponseFromService<EventosSalud>.Failure(failure);
                }
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<EventosSalud>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<bool>> UpdateEventoPacienteAsync(EventosSalud evento)
        {
            try
            {
                _context.Entry(evento).State = EntityState.Modified;
                var affectedRows = await _context.SaveChangesAsync();
                if (affectedRows > 0)
                {
                    var success = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "EVENTO_ACTUALIZADO");
                    return ResponseFromService<bool>.Success(true, success);
                }
                else
                {
                    var failure = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "EVENTO_NO_ACTUALIZADO");
                    return ResponseFromService<bool>.Failure(failure);
                }
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<bool>> DeleteEventoPacienteAsync(Guid idEventoSalud)
        {
            try
            {
                var evento = await _context.EventosSalud.FindAsync(idEventoSalud);
                if (evento == null)
                {
                    var notificacion = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "NO_EVENTOS_ENCONTRADOS");
                    return ResponseFromService<bool>.Failure(notificacion);
                }

                _context.EventosSalud.Remove(evento);
                var affectedRows = await _context.SaveChangesAsync();
                if (affectedRows > 0)
                {
                    var success = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "EVENTO_ELIMINADO");
                    return ResponseFromService<bool>.Success(true, success);
                }
                else
                {
                    var failure = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "EVENTO_NO_ELIMINADO");
                    return ResponseFromService<bool>.Failure(failure);
                }
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }


        // Obtiene el IdPaciente asociado a un usuario, mediante un SP estandarizado.
        public async Task<ResponseFromService<Guid?>> GetIdPacienteByUsuarioAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Recetas_GetPacienteByIdUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();

                // Leer el código de notificación del primer conjunto
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<Guid?>.Failure(notificacion);
                }

                // Leer el segundo conjunto, que contiene el IdPaciente
                Guid? idPaciente = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    idPaciente = reader.IsDBNull(0) ? (Guid?)null : reader.GetGuid(0);
                }

                return ResponseFromService<Guid?>.Success(idPaciente, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<Guid?>.Exeption(ex, error);
            }
        }


        private async Task<PacienteConsultaRequest?> ObtenerUsuarioPacienteAsync(string procedimiento, string paramName, Guid paramValue)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(procedimiento, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter(paramName, paramValue));

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return null;

                PacienteConsultaRequest paciente = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    var usuarioPaciente = UsuarioPaciente.FromDataReader(reader);
                    paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                }
                return paciente;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<PacienteConsultaRequest>> ObtenerListaPacientesAsync(string procedimiento, params SqlParameter[] parameters)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                string sqlQuery = $"EXEC {procedimiento} " + string.Join(", ", parameters.Select(p => p.ParameterName));
                using var command = new SqlCommand(sqlQuery, connection)
                {
                    CommandType = CommandType.Text
                };
                command.Parameters.AddRange(parameters);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return new List<PacienteConsultaRequest>();

                var lista = new List<PacienteConsultaRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var usuarioPaciente = UsuarioPaciente.FromDataReader(reader);
                        lista.Add(await MapToPacienteConsultaRequest(usuarioPaciente));
                    }
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> EjecutarStoredProcedureAsync(string procedimiento, object pacienteRequest, Guid idUsuarioSolicitante)
        {
            try
            {
                var pacienteJson = JsonConvert.SerializeObject(pacienteRequest);
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(procedimiento, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@PacienteJson", SqlDbType.NVarChar) { Value = pacienteJson });
                command.Parameters.Add(new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante));
                var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputMessageParam);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                return codigoNotificacion;
            }
            catch (Exception)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return notificacion.CodigoNotificacion;
            }
        }

        private async Task<PacienteConsultaRequest> MapToPacienteConsultaRequest(UsuarioPaciente paciente)
        {
            try
            {
                var alergias = await ObtenerAlergiasPorIdsAsync(paciente.Alergias);
                var molecules = await ObtenerMoleculesPorIdsAsync(paciente.Molecules);
                var patologias = await ObtenerCIM10PorIdsAsync(paciente.Patologias);

                return new PacienteConsultaRequest
                {
                    IdUsuario = paciente.IdUsuario,
                    IdTipoUsuario = paciente.IdTipoUsuario,
                    IdPaciente = paciente.IdPaciente,
                    IdGEMP = paciente.IdGEMP,
                    GrupoEmpresarial = paciente.GrupoEmpresarial,
                    IdSucursal = paciente.IdSucursal,
                    Sucursal = paciente.Sucursal,
                    Usr = paciente.Usr,
                    Nombres = paciente.Nombres,
                    PrimerApellido = paciente.PrimerApellido,
                    SegundoApellido = paciente.SegundoApellido,
                    IdTipoIdentificacion = paciente.IdTipoIdentificacion,
                    TipoIdentificacion = paciente.TipoIdentificacion,
                    NumeroIdentificacion = paciente.NumeroIdentificacion,
                    FechaNacimiento = paciente.FechaNacimiento,
                    Edad = paciente.Edad,
                    IdEntidadNacimiento = paciente.IdEntidadNacimiento,
                    EntidadNacimiento = paciente.EntidadNacimiento,
                    Genero = paciente.Genero,
                    Movil = paciente.Movil,
                    Email = paciente.Email,
                    Domicilio = paciente.Domicilio,
                    IdAsentamiento = paciente.IdAsentamiento,
                    Asentamiento = paciente.Asentamiento,
                    IdTipoAsentamiento = paciente.IdTipoAsentamiento,
                    TipoAsentamiento = paciente.TipoAsentamiento,
                    IdCP = paciente.IdCP,
                    CodigoPostal = paciente.CodigoPostal,
                    IdMunicipio = paciente.IdMunicipio,
                    NoMunicipio = paciente.NoMunicipio,
                    Municipio = paciente.Municipio,
                    IdCiudad = paciente.IdCiudad,
                    Ciudad = paciente.Ciudad,
                    IdEntidad = paciente.IdEntidad,
                    Estado = paciente.Estado,
                    Abreviatura = paciente.Abreviatura,
                    Firma = paciente.Firma,
                    Imagen = paciente.Imagen,
                    Alergias = alergias,
                    Molecules = molecules,
                    Patologias = patologias
                };
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }

        private async Task<List<AllergyModel>> ObtenerAlergiasPorIdsAsync(string alergiasIds)
        {
            try
            {
                return string.IsNullOrEmpty(alergiasIds)
                    ? new List<AllergyModel>()
                    : await _context.AllergyModels
                         .FromSqlRaw("EXEC Vidal_GetAllergiesByIds @Ids", new SqlParameter("@Ids", alergiasIds))
                         .ToListAsync();
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }

        private async Task<List<MoleculeModel>> ObtenerMoleculesPorIdsAsync(string moleculesIds)
        {
            try
            {
                return string.IsNullOrEmpty(moleculesIds)
                    ? new List<MoleculeModel>()
                    : await _context.MoleculeModels
                         .FromSqlRaw("EXEC Vidal_GetMoleculesByIds @Ids", new SqlParameter("@Ids", moleculesIds))
                         .ToListAsync();
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }

        private async Task<List<CIM10Model>> ObtenerCIM10PorIdsAsync(string cim10Ids)
        {
            try
            {
                return string.IsNullOrEmpty(cim10Ids)
                    ? new List<CIM10Model>()
                    : await _context.CIM10Models
                         .FromSqlRaw("EXEC Vidal_GetCIM10ByIds @Ids", new SqlParameter("@Ids", cim10Ids))
                         .ToListAsync();
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }

    }
}
