using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
{
    public class CatAsentamientoCiudadService : ICatAsentamientoCiudadService
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatAsentamientoCiudadService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para CatAsentamientoCiudad

        public async Task<ResponseFromService<IEnumerable<CatAsentamientoCiudad>>> GetAllAsentamientoCiudadAsync()
        {
            try
            {
                var registros = await _context.CatAsentamientoCiudad.ToListAsync();
                if (!registros.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_ASENTAMIENTO_CIUDAD");
                    return ResponseFromService<IEnumerable<CatAsentamientoCiudad>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_CIUDAD_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatAsentamientoCiudad>>.Success(registros, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatAsentamientoCiudad>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatAsentamientoCiudad>> GetAsentamientoCiudadByIdAsync(int id)
        {
            try
            {
                var registro = await _context.CatAsentamientoCiudad.FindAsync(id);
                if (registro == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_CIUDAD_NO_ENCONTRADO");
                    return ResponseFromService<CatAsentamientoCiudad>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_CIUDAD_OBTENIDO");
                return ResponseFromService<CatAsentamientoCiudad>.Success(registro, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatAsentamientoCiudad>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatAsentamientoCiudad>> CreateAsentamientoCiudadAsync(CatAsentamientoCiudad entidad)
        {
            try
            {
                _context.CatAsentamientoCiudad.Add(entidad);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_CIUDAD_CREADO");
                return ResponseFromService<CatAsentamientoCiudad>.Success(entidad, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatAsentamientoCiudad>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateAsentamientoCiudadAsync(int id, CatAsentamientoCiudad entidad)
        {
            try
            {
                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_CIUDAD_ACTUALIZADO");
                return ResponseFromService<string>.Success("Asentamiento-Ciudad actualizado correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> DeleteAsentamientoCiudadAsync(int id)
        {
            try
            {
                var registro = await _context.CatAsentamientoCiudad.FindAsync(id);
                if (registro == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_CIUDAD_NO_ENCONTRADO");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.CatAsentamientoCiudad.Remove(registro);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_CIUDAD_ELIMINADO");
                return ResponseFromService<string>.Success("Registro eliminado correctamente.", success);
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
