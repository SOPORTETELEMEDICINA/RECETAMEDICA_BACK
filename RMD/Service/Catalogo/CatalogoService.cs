using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Interface.Notificaciones;
using RMD.Models.Catalogo;
using RMD.Models.Consulta;
using RMD.Models.Responses;

namespace RMD.Services.Catalogo
{
    public class CatalogoService : ICatalogoService
    {
        private readonly CatalogoDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatalogoService(CatalogoDbContext context, ICatalogoNotificacionService catalogoNotificacionService)
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
                if (entidades == null || !entidades.Any())
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

        public async Task<ResponseFromService<IEnumerable<AsentamientoResultModel>>> BuscarAsentamientosAsync(AsentamientoSearchModel searchModel)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CodigoPostalParam", searchModel.CodigoPostal ?? (object)DBNull.Value),
                    new SqlParameter("@NombreAsentamientoParam", searchModel.NombreAsentamiento ?? (object)DBNull.Value),
                    new SqlParameter("@TipoAsentamientoParam", searchModel.TipoAsentamiento ?? (object)DBNull.Value),
                    new SqlParameter("@NombreMunicipioParam", searchModel.NombreMunicipio ?? (object)DBNull.Value),
                    new SqlParameter("@NombreCiudadParam", searchModel.NombreCiudad ?? (object)DBNull.Value),
                    new SqlParameter("@AbreviaturaParam", searchModel.Abreviatura ?? (object)DBNull.Value)
                };

                var results = await _context.AsentamientoResultModel
                    .FromSqlRaw("EXEC Catalogo_GetAsentamientosByNames @CodigoPostalParam, @NombreAsentamientoParam, @TipoAsentamientoParam, @NombreMunicipioParam, @NombreCiudadParam, @AbreviaturaParam", parameters)
                    .ToListAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<IEnumerable<AsentamientoResultModel>>.Success(results, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<AsentamientoResultModel>>.Exeption(ex, error);
            }
        }


        #endregion

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
