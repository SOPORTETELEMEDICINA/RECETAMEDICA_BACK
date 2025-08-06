using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Service.Catalogo
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
        public async Task<ResponseFromService<IEnumerable<AsentamientoResultModel>>> BuscarAsentamientosAsync(AsentamientoSearchModel searchModel)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CodigoPostal", searchModel.CodigoPostal ?? (object)DBNull.Value),
                    new SqlParameter("@NombreAsentamiento", searchModel.NombreAsentamiento ?? (object)DBNull.Value),
                    new SqlParameter("@TipoAsentamiento", searchModel.TipoAsentamiento ?? (object)DBNull.Value),
                    new SqlParameter("@NombreMunicipio", searchModel.NombreMunicipio ?? (object)DBNull.Value),
                    new SqlParameter("@NombreCiudad", searchModel.NombreCiudad ?? (object)DBNull.Value),
                    new SqlParameter("@Abreviatura", searchModel.Abreviatura ?? (object)DBNull.Value)
                };

                var results = await _context.AsentamientoResultModel
                    .FromSqlRaw("EXEC Catalogo_GetAsentamientosByNames @CodigoPostal, @NombreAsentamiento, @TipoAsentamiento, @NombreMunicipio, @NombreCiudad, @Abreviatura", parameters)
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
        
        public async Task<ResponseFromService<int>> InsertarDomicilioCompletoAsync(DomicilioRequest model)
        {
            try
            {
                // 1. Validar y/o insertar país
                int idPais = model.IdPais;
                if (idPais == 0)
                {
                    var pais = new CatPaises { NombrePais = model.NombrePais, Abreviatura = model.AbreviaturaPais };
                    _context.CatPaises.Add(pais);
                    await _context.SaveChangesAsync();
                    idPais = pais.Id;
                }

                // 2. Validar y/o insertar entidad federativa
                int idEntidad = model.IdEntidad;
                if (idEntidad == 0)
                {
                    var entidad = new CatEntidadesFederativas { Nombre = model.NombreEntidad, Abreviatura = model.AbreviaturaEntidad, IdPais = idPais };
                    _context.CatEntidadesFederativas.Add(entidad);
                    await _context.SaveChangesAsync();
                    idEntidad = entidad.IdEntidad;
                }

                // 3. Validar y/o insertar municipio
                int idMunicipio = model.IdMunicipio;
                if (idMunicipio == 0)
                {
                    var municipio = new CatMunicipios { Nombre = model.NombreMunicipio, IdEntidad = idEntidad };
                    _context.CatMunicipios.Add(municipio);
                    await _context.SaveChangesAsync();
                    idMunicipio = municipio.IdMunicipio;
                }

                // 4. Validar y/o insertar ciudad
                int idCiudad = model.IdCiudad;
                if (idCiudad == 0)
                {
                    var ciudad = new CatCiudades { Nombre = model.NombreCiudad };
                    _context.CatCiudades.Add(ciudad);
                    await _context.SaveChangesAsync();
                    idCiudad = ciudad.IdCiudad;
                }

                // 5. Validar y/o insertar CP
                int idCP = model.IdCP;
                if (idCP == 0)
                {
                    var cp = new CatCP { CodigoPostal = model.CodigoPostal, IdEntidad = idEntidad, IdMunicipio = idMunicipio };
                    _context.CatCP.Add(cp);
                    await _context.SaveChangesAsync();
                    idCP = cp.IdCP;
                }

                // 6. Validar y/o insertar tipo de asentamiento
                int idTipoAsentamiento = model.IdTipoAsentamiento;
                if (idTipoAsentamiento == 0)
                {
                    var tipo = new CatTipoAsentamiento { TipoAsentamiento = model.TipoAsentamiento };
                    _context.CatTipoAsentamiento.Add(tipo);
                    await _context.SaveChangesAsync();
                    idTipoAsentamiento = tipo.IdTipoAsentamiento;
                }

                // 7. Insertar asentamiento
                var asentamiento = new CatAsentamientos
                {
                    Nombre = model.NombreAsentamiento,
                    IdTipoAsentamiento = idTipoAsentamiento,
                    IdCP = idCP,
                    IdMunicipio = idMunicipio,
                    IdCiudad = idCiudad
                };
                _context.CatAsentamientos.Add(asentamiento);
                await _context.SaveChangesAsync();

                // 8. Insertar relación asentamiento-ciudad
                var relacion = new CatAsentamientoCiudad
                {
                    IdAsentamiento = asentamiento.IdAsentamiento,
                    IdCiudad = idCiudad
                };
                _context.CatAsentamientoCiudad.Add(relacion);
                await _context.SaveChangesAsync();

                // Notificación de éxito
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CATALOGOS", "DOMICILIO_CREADO");
                return ResponseFromService<int>.Success(asentamiento.IdAsentamiento, notif);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<int>.Exeption(ex, error);
            }
        }
    }
}
