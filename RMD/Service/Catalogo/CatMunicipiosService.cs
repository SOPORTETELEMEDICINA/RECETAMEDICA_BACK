using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
{
    public class CatMunicipiosService: ICatMunicipiosService
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatMunicipiosService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        #region CRUD para Municipios

        public async Task<ResponseFromService<IEnumerable<CatMunicipios>>> GetAllMunicipiosByIdEntidadAsync(int idEntidad)
        {
            try
            {
                var municipios = await _context.CatMunicipios.Where(m => m.IdEntidad == idEntidad).ToListAsync();
                if (!municipios.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_MUNICIPIOS_ENTIDAD");
                    return ResponseFromService<IEnumerable<CatMunicipios>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "MUNICIPIOS_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatMunicipios>>.Success(municipios, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatMunicipios>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatMunicipios>> GetMunicipioByIdAsync(int id)
        {
            try
            {
                var municipio = await _context.CatMunicipios.FindAsync(id);
                if (municipio == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "MUNICIPIO_NO_ENCONTRADO");
                    return ResponseFromService<CatMunicipios>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "MUNICIPIO_OBTENIDO");
                return ResponseFromService<CatMunicipios>.Success(municipio, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatMunicipios>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<CatMunicipios>>> GetMunicipiosByNameAsync(string name)
        {
            try
            {
                var municipios = await _context.CatMunicipios.Where(m => EF.Functions.Like(m.Nombre, $"%{name}%")).ToListAsync();
                if (!municipios.Any())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "SIN_MUNICIPIOS_NOMBRE");
                    return ResponseFromService<IEnumerable<CatMunicipios>>.Failure(notif);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "MUNICIPIOS_OBTENIDOS");
                return ResponseFromService<IEnumerable<CatMunicipios>>.Success(municipios, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatMunicipios>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<CatMunicipios>> CreateMunicipioAsync(CatMunicipios municipio)
        {
            try
            {
                _context.CatMunicipios.Add(municipio);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "MUNICIPIO_CREADO");
                return ResponseFromService<CatMunicipios>.Success(municipio, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatMunicipios>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateMunicipioAsync(int id, CatMunicipios municipio)
        {
            try
            {
                if (id != municipio.IdMunicipio)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ID_NO_COINCIDE");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.Entry(municipio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "MUNICIPIO_ACTUALIZADO");
                return ResponseFromService<string>.Success("Municipio actualizado correctamente.", success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> DeleteMunicipioAsync(int id)
        {
            try
            {
                var municipio = await _context.CatMunicipios.FindAsync(id);
                if (municipio == null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "MUNICIPIO_NO_ENCONTRADO");
                    return ResponseFromService<string>.Failure(notif);
                }

                _context.CatMunicipios.Remove(municipio);
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "MUNICIPIO_ELIMINADO");
                return ResponseFromService<string>.Success("Municipio eliminado correctamente.", success);
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
