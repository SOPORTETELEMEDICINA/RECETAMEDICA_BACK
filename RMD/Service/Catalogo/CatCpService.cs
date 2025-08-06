using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
{
    public class CatCpService : ICatCpService        
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatCpService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        #region CRUD para CatCP

        private async Task<bool> CPExistsAsync(string codigoPostal, int idEntidad, int? id = null) =>
            await _context.CatCP.AnyAsync(c => c.CodigoPostal == codigoPostal && c.IdEntidad == idEntidad &&
                                               (!id.HasValue || c.IdCP != id));
        public async Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPAsync()
        {
            try
            {
                var cps = await _context.CatCP.ToListAsync();
                if (!cps.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_CP");
                    return ResponseFromService<IEnumerable<CatCP>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatCP>>.Success(cps, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatCP>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPByMunicipioAsync(int idMunicipio)
        {
            try
            {
                var cps = await _context.CatCP.Where(c => c.IdMunicipio == idMunicipio).ToListAsync();
                if (!cps.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_CP_MUNICIPIO");
                    return ResponseFromService<IEnumerable<CatCP>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_MUNICIPIO_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatCP>>.Success(cps, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatCP>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPByEntidadAsync(int idEntidad)
        {
            try
            {
                var cps = await _context.CatCP.Where(c => c.IdEntidad == idEntidad).ToListAsync();
                if (!cps.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_CP_ENTIDAD");
                    return ResponseFromService<IEnumerable<CatCP>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_ENTIDAD_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatCP>>.Success(cps, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatCP>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatCP>> GetCPByIdAsync(int id)
        {
            try
            {
                var cp = await _context.CatCP.FindAsync(id);
                if (cp == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_NO_ENCONTRADO");
                    return ResponseFromService<CatCP>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_OBTENIDO");
                return ResponseFromService<CatCP>.Success(cp, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatCP>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatCP>> CreateCPAsync(CatCP cp)
        {
            try
            {
                if (await CPExistsAsync(cp.CodigoPostal, cp.IdEntidad))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_EXISTE");
                    return ResponseFromService<CatCP>.Failure(notif);
                }

                _context.CatCP.Add(cp);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_CREADO");
                return ResponseFromService<CatCP>.Success(cp, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatCP>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateCPAsync(int id, CatCP cp)
        {
            try
            {
                if (await CPExistsAsync(cp.CodigoPostal, cp.IdEntidad, id))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_EXISTE");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.Entry(cp).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_ACTUALIZADO");
                return ResponseFromService<string>.Success("CP actualizado correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> DeleteCPAsync(int id)
        {
            try
            {
                var cp = await _context.CatCP.FindAsync(id);
                if (cp == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_NO_ENCONTRADO");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.CatCP.Remove(cp);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "CP_ELIMINADO");
                return ResponseFromService<string>.Success("CP eliminado correctamente.", success);
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
