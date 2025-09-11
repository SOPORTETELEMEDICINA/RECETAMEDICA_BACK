using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Pacientes;
using RMD.Interface.Security;
using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;
using System.Data;

namespace RMD.Service.Pacientes
{
    public class EventosSaludService(PacientesDbContext context,
        ICatalogoNotificacionService catalogoNotificacionService,
        IDapperService dapperService) : IEventosSaludService
    {
        private readonly PacientesDbContext _context = context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService = catalogoNotificacionService;
        private readonly IDapperService _dapperService = dapperService;

        public async Task<ResponseFromService<IEnumerable<EventosSaludResponse>>> GetAllEventosPacienteAsync(Guid idPaciente)
        {
            try
            {
                var parameters = new { IdPaciente = idPaciente };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "EventosSalud_GetEventosByPaciente",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<EventosSaludResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<EventosSaludResponse>>.Success(new List<EventosSaludResponse>(), notificacion);

                var eventos = multi.Read<EventosSaludResponse>().ToList();
                return ResponseFromService<IEnumerable<EventosSaludResponse>>.Success(eventos, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<EventosSaludResponse>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<EventosSaludResponse>> GetEventoPacienteByIdAsync(Guid idEventoSalud)
        {
            try
            {
                var evento = await (from ev in _context.EventosSalud
                                    join cat in _context.CatEventosDeSalud on ev.EventoDeSalud equals cat.IdEvento
                                    where ev.IdEventoSalud == idEventoSalud
                                    select new EventosSaludResponse
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
                    var notFound = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "EVENTO_NO_ENCONTRADO");

                    return ResponseFromService<EventosSaludResponse>.Failure(notFound);
                }

                var success = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "EVENTO_ENCONTRADO");

                return ResponseFromService<EventosSaludResponse>.Success(evento, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");

                return ResponseFromService<EventosSaludResponse>.Exeption(ex, error);
            }
        }

        //public async Task<ResponseFromService<EventosSaludRequest>> CreateEventoPacienteAsync(EventosSaludRequest evento)
        //{
        //    try
        //    {
        //        _context.EventosSalud.Add(evento);
        //        var affectedRows = await _context.SaveChangesAsync();
        //        if (affectedRows > 0)
        //        {
        //            var success = await _catalogoNotificacionService
        //                .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "CREADO");
        //            return ResponseFromService<EventosSaludRequest>.Success(evento, success);
        //        }
        //        else
        //        {
        //            var failure = await _catalogoNotificacionService
        //                .GetNotificationByTipoAndFuncionAsync("PACIENTESP", "NO_CREADO");
        //            return ResponseFromService<EventosSaludRequest>.Failure(failure);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var notificacion = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
        //        return ResponseFromService<EventosSaludRequest>.Exeption(ex, notificacion);
        //    }
        //}
        public async Task<ResponseFromService<bool>> CreateEventoPacienteAsync(EventosSaludRequest evento)
        {
            try
            {
                _context.EventosSalud.Add(evento);
                var affectedRows = await _context.SaveChangesAsync();

                if (affectedRows > 0)
                {
                    var success = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("EVENTOSC", "CREADO");

                    return ResponseFromService<bool>.Success(true, success);
                }
                else
                {
                    var failure = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("EVENTOSC", "NO_CREADO");

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


        public async Task<ResponseFromService<bool>> UpdateEventoPacienteAsync(EventosSaludRequest evento)
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
    }
}
