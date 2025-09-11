using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
{
    public class CatCiudadesService : ICatCiudadesService
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatCiudadesService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para CatCiudades

        private async Task<bool> CiudadExistsAsync(string nombre, int? id = null) =>
            await _context.CatCiudades.AnyAsync(c => c.Nombre == nombre && (!id.HasValue || c.IdCiudad != id));

        public async Task<ResponseFromService<IEnumerable<CatCiudades>>> GetAllCiudadesAsync()
        {
            try
            {
                var ciudades = await _context.CatCiudades.ToListAsync();
                if (!ciudades.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_CIUDADES");
                    return ResponseFromService<IEnumerable<CatCiudades>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDADES_OBTENIDAS");
                return ResponseFromService<IEnumerable<CatCiudades>>.Success(ciudades, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatCiudades>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatCiudades>> GetCiudadByIdAsync(int id)
        {
            try
            {
                var ciudad = await _context.CatCiudades.FindAsync(id);
                if (ciudad == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDAD_NO_ENCONTRADA");
                    return ResponseFromService<CatCiudades>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDAD_OBTENIDA");
                return ResponseFromService<CatCiudades>.Success(ciudad, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatCiudades>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<CatCiudades>>> GetCiudadesByNameAsync(string name)
        {
            try
            {
                var ciudades = await _context.CatCiudades
                    .Where(c => EF.Functions.Like(c.Nombre, $"%{name}%"))
                    .ToListAsync();

                if (!ciudades.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_CIUDADES_NOMBRE");
                    return ResponseFromService<IEnumerable<CatCiudades>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDADES_OBTENIDAS_NOMBRE");
                return ResponseFromService<IEnumerable<CatCiudades>>.Success(ciudades, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatCiudades>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatCiudades>> CreateCiudadAsync(CatCiudades ciudad)
        {
            try
            {
                if (await CiudadExistsAsync(ciudad.Nombre))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDAD_EXISTE");
                    return ResponseFromService<CatCiudades>.Failure(notif);
                }

                _context.CatCiudades.Add(ciudad);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDAD_CREADA");
                return ResponseFromService<CatCiudades>.Success(ciudad, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatCiudades>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateCiudadAsync(int id, CatCiudades ciudad)
        {
            try
            {
                if (await CiudadExistsAsync(ciudad.Nombre, id))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDAD_EXISTE");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.Entry(ciudad).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDAD_ACTUALIZADA");
                return ResponseFromService<string>.Success("Ciudad actualizada correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> DeleteCiudadAsync(int id)
        {
            try
            {
                var ciudad = await _context.CatCiudades.FindAsync(id);
                if (ciudad == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDAD_NO_ENCONTRADA");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.CatCiudades.Remove(ciudad);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CIUDAD_ELIMINADA");
                return ResponseFromService<string>.Success("Ciudad eliminada correctamente.", success);
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
