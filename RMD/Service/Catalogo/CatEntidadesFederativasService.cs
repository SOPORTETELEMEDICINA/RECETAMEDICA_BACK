using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
{
    public class CatEntidadesFederativasService: ICatEntidadesFederativasService
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        public CatEntidadesFederativasService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        #region CRUD para EntidadesFederativas

        public async Task<ResponseFromService<IEnumerable<CatEntidadesFederativas>>> GetAllEntidadesAsync()
        {
            try
            {
                var entidades = await _context.CatEntidadesFederativas.ToListAsync();
                if (!entidades.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_ENTIDADES");
                    return ResponseFromService<IEnumerable<CatEntidadesFederativas>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDADES_OBTENIDAS");
                return ResponseFromService<IEnumerable<CatEntidadesFederativas>>.Success(entidades, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatEntidadesFederativas>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatEntidadesFederativas>> GetEntidadByIdAsync(int id)
        {
            try
            {
                var entidad = await _context.CatEntidadesFederativas.FindAsync(id);
                if (entidad == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDAD_NO_ENCONTRADA");
                    return ResponseFromService<CatEntidadesFederativas>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDAD_OBTENIDA");
                return ResponseFromService<CatEntidadesFederativas>.Success(entidad, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatEntidadesFederativas>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<CatEntidadesFederativas>>> GetEntidadesByNameAsync(string name)
        {
            try
            {
                var entidades = await _context.CatEntidadesFederativas
                    .Where(e => EF.Functions.Like(e.Nombre, $"%{name}%"))
                    .ToListAsync();

                if (!entidades.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_ENTIDADES");
                    return ResponseFromService<IEnumerable<CatEntidadesFederativas>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDADES_OBTENIDAS");
                return ResponseFromService<IEnumerable<CatEntidadesFederativas>>.Success(entidades, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatEntidadesFederativas>>.Exeption(ex, error);
            }
        }

        private async Task<bool> EntidadExistsAsync(string nombre, string abreviatura, int idPais, int? id = null) =>
            await _context.CatEntidadesFederativas.AnyAsync(e =>
                e.IdPais == idPais &&
                (e.Nombre == nombre || e.Abreviatura == abreviatura) &&
                (!id.HasValue || e.IdEntidad != id));

        public async Task<ResponseFromService<CatEntidadesFederativas>> CreateEntidadAsync(CatEntidadesFederativas entidad)
        {
            try
            {
                if (await EntidadExistsAsync(entidad.Nombre, entidad.Abreviatura, entidad.IdPais))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDAD_DUPLICADA");
                    return ResponseFromService<CatEntidadesFederativas>.Failure(notif);
                }

                _context.CatEntidadesFederativas.Add(entidad);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDAD_CREADA");
                return ResponseFromService<CatEntidadesFederativas>.Success(entidad, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatEntidadesFederativas>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateEntidadAsync(int id, CatEntidadesFederativas entidad)
        {
            try
            {
                if (await EntidadExistsAsync(entidad.Nombre, entidad.Abreviatura, entidad.IdPais, id))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDAD_DUPLICADA");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDAD_ACTUALIZADA");
                return ResponseFromService<string>.Success("Entidad actualizada correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> DeleteEntidadAsync(int id)
        {
            try
            {
                var entidad = await _context.CatEntidadesFederativas.FindAsync(id);
                if (entidad == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDAD_NO_ENCONTRADA");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.CatEntidadesFederativas.Remove(entidad);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ENTIDAD_ELIMINADA");
                return ResponseFromService<string>.Success("Entidad eliminada correctamente.", success);
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
