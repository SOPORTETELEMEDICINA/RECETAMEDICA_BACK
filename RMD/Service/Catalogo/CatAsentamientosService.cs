using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
{
    public class CatAsentamientosService : ICatAsentamientosService
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatAsentamientosService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para CatAsentamientos

        private async Task<bool> AsentamientoExistsAsync(string nombre, int idCP, int? id = null) =>
            await _context.CatAsentamientos.AnyAsync(a => a.Nombre == nombre && a.IdCP == idCP && (!id.HasValue || a.IdAsentamiento != id));

        public async Task<ResponseFromService<IEnumerable<CatAsentamientos>>> GetAllAsentamientosAsync()
        {
            try
            {
                var asentamientos = await _context.CatAsentamientos.ToListAsync();
                if (!asentamientos.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_ASENTAMIENTOS");
                    return ResponseFromService<IEnumerable<CatAsentamientos>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTOS_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatAsentamientos>>.Success(asentamientos, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatAsentamientos>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatAsentamientos>> GetAsentamientoByIdAsync(int id)
        {
            try
            {
                var asentamiento = await _context.CatAsentamientos.FindAsync(id);
                if (asentamiento == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_NO_ENCONTRADO");
                    return ResponseFromService<CatAsentamientos>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_OBTENIDO");
                return ResponseFromService<CatAsentamientos>.Success(asentamiento, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatAsentamientos>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatAsentamientos>> CreateAsentamientoAsync(CatAsentamientos asentamiento)
        {
            try
            {
                if (await AsentamientoExistsAsync(asentamiento.Nombre, asentamiento.IdCP))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_EXISTE");
                    return ResponseFromService<CatAsentamientos>.Failure(notif);
                }

                _context.CatAsentamientos.Add(asentamiento);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_CREADO");
                return ResponseFromService<CatAsentamientos>.Success(asentamiento, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatAsentamientos>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateAsentamientoAsync(int id, CatAsentamientos asentamiento)
        {
            try
            {
                if (await AsentamientoExistsAsync(asentamiento.Nombre, asentamiento.IdCP, id))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_EXISTE");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.Entry(asentamiento).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_ACTUALIZADO");
                return ResponseFromService<string>.Success("Asentamiento actualizado correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> DeleteAsentamientoAsync(int id)
        {
            try
            {
                var asentamiento = await _context.CatAsentamientos.FindAsync(id);
                if (asentamiento == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_NO_ENCONTRADO");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.CatAsentamientos.Remove(asentamiento);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_ELIMINADO");
                return ResponseFromService<string>.Success("Asentamiento eliminado correctamente.", success);
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
