using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
{
    public class CatTipoAsentamientoService: ICatTipoAsentamientoService
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatTipoAsentamientoService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        #region CRUD para Tipo de Asentamiento

        private async Task<bool> AsentamientoExistsAsync(string name, int? id = null) =>
            await _context.CatTipoAsentamiento.AnyAsync(a => a.TipoAsentamiento == name && (!id.HasValue || a.IdTipoAsentamiento != id));

        public async Task<ResponseFromService<IEnumerable<CatTipoAsentamiento>>> GetAllTipoAsentamientosAsync()
        {
            try
            {
                var tipos = await _context.CatTipoAsentamiento.ToListAsync();
                if (!tipos.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_TIPOS_ASENTAMIENTO");
                    return ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPOS_ASENTAMIENTO_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Success(tipos, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatTipoAsentamiento>> GetTipoAsentamientoByIdAsync(int id)
        {
            try
            {
                var tipo = await _context.CatTipoAsentamiento.FindAsync(id);
                if (tipo == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPO_ASENTAMIENTO_NO_ENCONTRADO");
                    return ResponseFromService<CatTipoAsentamiento>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPO_ASENTAMIENTO_OBTENIDO");
                return ResponseFromService<CatTipoAsentamiento>.Success(tipo, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatTipoAsentamiento>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<CatTipoAsentamiento>>> GetTipoAsentamientosByNameAsync(string name)
        {
            try
            {
                var tipos = await _context.CatTipoAsentamiento.Where(a => EF.Functions.Like(a.TipoAsentamiento, $"%{name}%")).ToListAsync();
                if (!tipos.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_TIPOS_ASENTAMIENTO_NOMBRE");
                    return ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPOS_ASENTAMIENTO_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Success(tipos, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatTipoAsentamiento>> CreateTipoAsentamientoAsync(CatTipoAsentamiento asentamiento)
        {
            try
            {
                if (await AsentamientoExistsAsync(asentamiento.TipoAsentamiento))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPO_ASENTAMIENTO_EXISTE");
                    return ResponseFromService<CatTipoAsentamiento>.Failure(notif);
                }

                _context.CatTipoAsentamiento.Add(asentamiento);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPO_ASENTAMIENTO_CREADO");
                return ResponseFromService<CatTipoAsentamiento>.Success(asentamiento, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatTipoAsentamiento>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateTipoAsentamientoAsync(int id, CatTipoAsentamiento asentamiento)
        {
            try
            {
                if (await AsentamientoExistsAsync(asentamiento.TipoAsentamiento, id))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPO_ASENTAMIENTO_EXISTE");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.Entry(asentamiento).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPO_ASENTAMIENTO_ACTUALIZADO");
                return ResponseFromService<string>.Success("Tipo de asentamiento actualizado correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> DeleteTipoAsentamientoAsync(int id)
        {
            try
            {
                var tipo = await _context.CatTipoAsentamiento.FindAsync(id);
                if (tipo == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPO_ASENTAMIENTO_NO_ENCONTRADO");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.CatTipoAsentamiento.Remove(tipo);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "TIPO_ASENTAMIENTO_ELIMINADO");
                return ResponseFromService<string>.Success("Tipo de asentamiento eliminado correctamente.", success);
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
