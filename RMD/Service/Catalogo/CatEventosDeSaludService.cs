using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
{
    public class CatEventosDeSaludService : ICatEventosDeSaludService
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatEventosDeSaludService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para CatEventosDeSalud

        public async Task<ResponseFromService<IEnumerable<CatEventosDeSalud>>> GetAllEventosSaludAsync()
        {
            try
            {
                var eventos = await _context.CatEventosDeSalud.ToListAsync();
                if (!eventos.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_EVENTOS_SALUD");
                    return ResponseFromService<IEnumerable<CatEventosDeSalud>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "EVENTOS_SALUD_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatEventosDeSalud>>.Success(eventos, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatEventosDeSalud>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatEventosDeSalud>> GetEventoSaludByIdAsync(int id)
        {
            try
            {
                var evento = await _context.CatEventosDeSalud.FindAsync(id);
                if (evento == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "EVENTO_SALUD_NO_ENCONTRADO");
                    return ResponseFromService<CatEventosDeSalud>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "EVENTO_SALUD_OBTENIDO");
                return ResponseFromService<CatEventosDeSalud>.Success(evento, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatEventosDeSalud>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatEventosDeSalud>> CreateEventoSaludAsync(CatEventosDeSalud evento)
        {
            try
            {
                _context.CatEventosDeSalud.Add(evento);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "EVENTO_SALUD_CREADO");
                return ResponseFromService<CatEventosDeSalud>.Success(evento, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatEventosDeSalud>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateEventoSaludAsync(int id, CatEventosDeSalud evento)
        {
            try
            {
                if (id != evento.IdEvento)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ID_INVALIDO_EVENTO_SALUD");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.Entry(evento).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "EVENTO_SALUD_ACTUALIZADO");
                return ResponseFromService<string>.Success("Evento de salud actualizado correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> DeleteEventoSaludAsync(int id)
        {
            try
            {
                var evento = await _context.CatEventosDeSalud.FindAsync(id);
                if (evento == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "EVENTO_SALUD_NO_ENCONTRADO");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.CatEventosDeSalud.Remove(evento);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "EVENTO_SALUD_ELIMINADO");
                return ResponseFromService<string>.Success("Evento de salud eliminado correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }


        #endregion
    }
}
