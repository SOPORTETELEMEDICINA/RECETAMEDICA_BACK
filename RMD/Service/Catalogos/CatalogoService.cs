using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Catalogos;
using RMD.Models.Catalogos;
using RMD.Models.Consulta;
using System.Data;

namespace RMD.Service.Catalogos
{
    public class CatalogoService : ICatalogoService
    {
        private readonly ApplicationDbContext _context;

        public CatalogoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Catalogo>> GetCatalogoByIdCP(int idCP)
        {
            try
            {
                var results = await _context.Catalogos
                    .FromSqlRaw("EXEC Catalogos_GetCatalogoByIdCP @IdCP", new SqlParameter("@IdCP", idCP))
                    .ToListAsync();

                if (results.Any())
                {
                    return results;
                }
                else
                {
                    throw new KeyNotFoundException("No existe el Código Postal especificado.");
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejo de excepciones específicas de SQL
                throw new Exception("Ocurrió un error al ejecutar la consulta SQL.", sqlEx);
            }
            catch (Exception ex)
            {
                // Manejo de cualquier otra excepción
                throw new Exception("Ocurrió un error al obtener el catálogo por ID de CP.", ex);
            }
        }


        public async Task<IEnumerable<Catalogo>> GetCatalogoByIdMunicipio(int idMunicipio)
        {
            var results = await _context.Catalogos
                .FromSqlRaw("EXEC Catalogos_GetCatalogoByIdMunicipio @IdMunicipio", new SqlParameter("@IdMunicipio", idMunicipio))
                .ToListAsync();

            if (results.Any())
            {
                return results;
            }
            else
            {
                throw new KeyNotFoundException("No existe el Municipio especificado.");
            }
        }

        public async Task<IEnumerable<Catalogo>> GetCatalogoByIdAsentamiento(int idAsentamiento)
        {
            var results = await _context.Catalogos
                .FromSqlRaw("EXEC Catalogos_GetCatalogoByIdAsentamiento @IdAsentamiento", new SqlParameter("@IdAsentamiento", idAsentamiento))
                .ToListAsync();

            if (results.Any())
            {
                return results;
            }
            else
            {
                throw new KeyNotFoundException("No existe el Asentamiento especificado.");
            }
        }

        //public async Task<IEnumerable<Catalogo>> GetCatalogoByIdEntidad(int idEntidad)
        //{
        //    var results = await _context.Catalogos
        //        .FromSqlRaw("EXEC sp_Cat_GetByIdEntidad @IdEntidad", new SqlParameter("@IdEntidad", idEntidad))
        //        .ToListAsync();

        //    if (results.Any())
        //    {
        //        return results;
        //    }
        //    else
        //    {
        //        throw new KeyNotFoundException("No existe la Entidad especificada.");
        //    }
        //}

        public async Task<IEnumerable<Catalogo>> GetCatalogoByIdCiudad(int idCiudad)
        {
            var results = await _context.Catalogos
                .FromSqlRaw("EXEC Catalogos_GetCatalogoByIdCiudad @IdCiudad", new SqlParameter("@IdCiudad", idCiudad))
                .ToListAsync();

            if (results.Any())
            {
                return results;
            }
            else
            {
                throw new KeyNotFoundException("No existe la Ciudad especificada.");
            }
        }

        public async Task<string> CreateOrUpdateCatalogo(Catalogo catalogo)
        {
            var parameter = new SqlParameter("@CatalogoData", SqlDbType.Structured)
            {
                TypeName = "dbo.CatalogoType",
                Value = new List<Catalogo> { catalogo }.ToDataTable()
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC Catalogos_CreateOrUpdateCatalogo @CatalogoData", parameter);
            return "Operación realizada con éxito.";
        }

        public async Task<CatalogoDetail> GetDetailsByIdCP(int idCP)
        {
            var detail = new CatalogoDetail
            {
                Asentamientos = await _context.Asentamiento
                    .FromSqlRaw("EXEC Catalogos_GetAsentamientosByIdCP @IdCP", new SqlParameter("@IdCP", idCP))
                    .ToListAsync(),

                Municipios = await _context.Municipio
                    .FromSqlRaw("EXEC Catalogos_GetMunicipiosByIdCP @IdCP", new SqlParameter("@IdCP", idCP))
                    .ToListAsync(),

                Ciudades = await _context.Ciudad
                    .FromSqlRaw("EXEC Catalogos_GetCiudadesByIdCP @IdCP", new SqlParameter("@IdCP", idCP))
                    .ToListAsync(),

                Entidades = await _context.Entidad
                    .FromSqlRaw("EXEC Catalogos_GetEntidadesByIdCP @IdCP", new SqlParameter("@IdCP", idCP))
                    .ToListAsync()
            };

            return detail;
        }

        public async Task<CatalogoDetail> GetDetailsByIdAsentamiento(int idAsentamiento)
        {
            var detail = new CatalogoDetail
            {
                CPs = await _context.CP
                    .FromSqlRaw("EXEC Catalogos_GetCPsByIdAsentamiento @IdAsentamiento", new SqlParameter("@IdAsentamiento", idAsentamiento))
                    .ToListAsync(),

                Municipios = await _context.Municipio
                    .FromSqlRaw("EXEC Catalogos_GetMunicipiosByIdAsentamiento @IdAsentamiento", new SqlParameter("@IdAsentamiento", idAsentamiento))
                    .ToListAsync(),

                Ciudades = await _context.Ciudad
                    .FromSqlRaw("EXEC Catalogos_GetCiudadesByIdAsentamiento @IdAsentamiento", new SqlParameter("@IdAsentamiento", idAsentamiento))
                    .ToListAsync(),

                Entidades = await _context.Entidad
                    .FromSqlRaw("EXEC Catalogos_GetEntidadesByIdAsentamiento @IdAsentamiento", new SqlParameter("@IdAsentamiento", idAsentamiento))
                    .ToListAsync()
            };

            return detail;
        }

        public async Task<CatalogoDetail> GetDetailsByIdMunicipio(int idMunicipio)
        {
            var detail = new CatalogoDetail
            {
                CPs = await _context.CP
                    .FromSqlRaw("EXEC Catalogos_GetCPsByIdMunicipio @IdMunicipio", new SqlParameter("@IdMunicipio", idMunicipio))
                    .ToListAsync(),

                Asentamientos = await _context.Asentamiento
                    .FromSqlRaw("EXEC Catalogos_GetAsentamientosByIdMunicipio @IdMunicipio", new SqlParameter("@IdMunicipio", idMunicipio))
                    .ToListAsync(),

                Ciudades = await _context.Ciudad
                    .FromSqlRaw("EXEC Catalogos_GetCiudadesByIdMunicipio @IdMunicipio", new SqlParameter("@IdMunicipio", idMunicipio))
                    .ToListAsync(),

                Entidades = await _context.Entidad
                    .FromSqlRaw("EXEC Catalogos_GetEntidadesByIdMunicipio @IdMunicipio", new SqlParameter("@IdMunicipio", idMunicipio))
                    .ToListAsync()
            };

            return detail;
        }

        //public async Task<CatalogoDetail> GetDetailsByIdEntidad(int idEntidad)
        //{
        //    var detail = new CatalogoDetail
        //    {
        //        Municipios = await _context.Municipio
        //            .FromSqlRaw("EXEC Catalogos_GetMunicipiosByIdEntidad @IdEntidad", new SqlParameter("@IdEntidad", idEntidad))
        //            .ToListAsync(),

        //        CPs = await _context.CP
        //            .FromSqlRaw("EXEC Catalogos_GetCPsByIdEntidad @IdEntidad", new SqlParameter("@IdEntidad", idEntidad))
        //            .ToListAsync(),

        //        Asentamientos = await _context.Asentamiento
        //            .FromSqlRaw("EXEC Catalogos_GetAsentamientosByIdEntidad @IdEntidad", new SqlParameter("@IdEntidad", idEntidad))
        //            .ToListAsync(),

        //        Ciudades = await _context.Ciudad
        //            .FromSqlRaw("EXEC Catalogos_GetCiudadesByIdEntidad @IdEntidad", new SqlParameter("@IdEntidad", idEntidad))
        //            .ToListAsync()
        //    };

        //    return detail;
        //}

        public async Task<CatalogoDetail> GetDetailsByIdCiudad(int idCiudad)
        {
            var detail = new CatalogoDetail
            {
                Asentamientos = await _context.Asentamiento
                    .FromSqlRaw("EXEC Catalogos_GetAsentamientosByIdCiudad @IdCiudad", new SqlParameter("@IdCiudad", idCiudad))
                    .ToListAsync(),

                CPs = await _context.CP
                    .FromSqlRaw("EXEC Catalogos_GetCPsByIdCiudad @IdCiudad", new SqlParameter("@IdCiudad", idCiudad))
                    .ToListAsync(),

                Municipios = await _context.Municipio
                    .FromSqlRaw("EXEC Catalogos_GetMunicipiosByIdCiudad @IdCiudad", new SqlParameter("@IdCiudad", idCiudad))
                    .ToListAsync(),

                Entidades = await _context.Entidad
                    .FromSqlRaw("EXEC Catalogos_GetEntidadesByIdCiudad @IdCiudad", new SqlParameter("@IdCiudad", idCiudad))
                    .ToListAsync()
            };

            return detail;
        }


        public async Task<IEnumerable<AsentamientoResultModel>> BuscarAsentamientosAsync(AsentamientoSearchModel searchModel)
        {
            // Configuramos los parámetros para el procedimiento almacenado
            var parameters = new[]
            {
                new SqlParameter("@CodigoPostalParam", searchModel.CodigoPostal ?? (object)DBNull.Value),
                new SqlParameter("@NombreAsentamientoParam", searchModel.NombreAsentamiento ?? (object)DBNull.Value),
                new SqlParameter("@TipoAsentamientoParam", searchModel.TipoAsentamiento ?? (object)DBNull.Value),
                new SqlParameter("@NombreMunicipioParam", searchModel.NombreMunicipio ?? (object)DBNull.Value),
                new SqlParameter("@NombreCiudadParam", searchModel.NombreCiudad ?? (object)DBNull.Value),
                new SqlParameter("@AbreviaturaParam", searchModel.Abreviatura ?? (object)DBNull.Value)
            };

            // Ejecutamos la consulta y obtenemos los resultados usando Entity Framework Core
            var results = await _context.AsentamientoResultModel
                .FromSqlRaw("EXEC Catalogos_GetAsentamientosByNames @CodigoPostalParam, @NombreAsentamientoParam, @TipoAsentamientoParam, @NombreMunicipioParam, @NombreCiudadParam, @AbreviaturaParam", parameters)
                .ToListAsync();

            if (results.Count > 0)
            {
                return results;
            }
            else
            {
                throw new KeyNotFoundException("No se encontraron resultados para los criterios especificados.");
            }
        }
    }
}
