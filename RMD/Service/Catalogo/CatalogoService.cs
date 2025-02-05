using RMD.Data;
using RMD.Interface.Catalogo;
using RMD.Models.Catalogo;
using RMD.Models.Consulta;

namespace RMD.Services.Catalogo
{
    public class CatalogoService : ICatalogoService
    {
        private readonly CatalogoDbContext _context;

        public CatalogoService(CatalogoDbContext context)
        {
            _context = context;
        }
    #region CRUD para EntidadesFederativas
        // Obtener todas las entidades federativas
        public async Task<IEnumerable<CatEntidadesFederativas>> GetAllEntidadesAsync() =>
            await _context.CatEntidadesFederativas.ToListAsync();

        // Obtener entidad federativa por ID
        public async Task<CatEntidadesFederativas> GetEntidadByIdAsync(int id) =>
            await _context.CatEntidadesFederativas.FindAsync(id);

        // Obtener entidades federativas por nombre
        public async Task<IEnumerable<CatEntidadesFederativas>> GetEntidadesByNameAsync(string name) =>
            await _context.CatEntidadesFederativas
                .Where(e => EF.Functions.Like(e.Nombre, $"%{name}%"))
                .ToListAsync();
        // Validar si ya existe una entidad con el mismo nombre o abreviatura dentro del mismo IdPais
        private async Task<bool> EntidadExistsAsync(string nombre, string abreviatura, int idPais, int? id = null)
        {
            return await _context.CatEntidadesFederativas
                .AnyAsync(e =>
                    e.IdPais == idPais &&
                    (e.Nombre == nombre || e.Abreviatura == abreviatura) &&
                    (!id.HasValue || e.IdEntidad != id));
        }

        // Crear nueva entidad federativa con validación de duplicados por IdPais
        public async Task<CatEntidadesFederativas> CreateEntidadAsync(CatEntidadesFederativas entidad)
        {
            if (await EntidadExistsAsync(entidad.Nombre, entidad.Abreviatura, entidad.IdPais))
                throw new System.Exception("Ya existe una entidad con el mismo nombre o abreviatura para este país.");

            _context.CatEntidadesFederativas.Add(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        // Actualizar entidad federativa con validación de duplicados por IdPais
        public async Task UpdateEntidadAsync(int id, CatEntidadesFederativas entidad)
        {
            if (await EntidadExistsAsync(entidad.Nombre, entidad.Abreviatura, entidad.IdPais, id))
                throw new System.Exception("Ya existe una entidad con el mismo nombre o abreviatura para este país.");

            _context.Entry(entidad).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Eliminar entidad federativa
        public async Task DeleteEntidadAsync(int id)
        {
            var entidad = await _context.CatEntidadesFederativas.FindAsync(id);
            if (entidad == null) throw new KeyNotFoundException("Entidad no encontrada.");
            _context.CatEntidadesFederativas.Remove(entidad);
            await _context.SaveChangesAsync();
        }
    #endregion

    #region CRUD para Municipios

        // CRUD para Municipios
        //public async Task<IEnumerable<CatMunicipios>> GetAllMunicipiosAsync() =>
        //    await _context.CatMunicipios.ToListAsync();
        public async Task<IEnumerable<CatMunicipios>> GetAllMunicipiosByIdEntidadAsync(int idEntidad) =>
           await _context.CatMunicipios
               .Where(m => m.IdEntidad == idEntidad)
               .ToListAsync(); // Implementación del nuevo método


        public async Task<CatMunicipios> GetMunicipioByIdAsync(int id) =>
            await _context.CatMunicipios.FindAsync(id);

        public async Task<IEnumerable<CatMunicipios>> GetMunicipiosByNameAsync(string name) =>
            await _context.CatMunicipios
                .Where(m => EF.Functions.Like(m.Nombre, $"%{name}%"))
                .ToListAsync();

        public async Task<CatMunicipios> CreateMunicipioAsync(CatMunicipios municipio)
        {
            _context.CatMunicipios.Add(municipio);
            await _context.SaveChangesAsync();
            return municipio;
        }

        public async Task UpdateMunicipioAsync(int id, CatMunicipios municipio)
        {
            if (id != municipio.IdMunicipio) throw new KeyNotFoundException("Municipio no encontrado.");
            _context.Entry(municipio).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMunicipioAsync(int id)
        {
            var municipio = await _context.CatMunicipios.FindAsync(id);
            if (municipio == null) throw new KeyNotFoundException("Municipio no encontrado.");
            _context.CatMunicipios.Remove(municipio);
            await _context.SaveChangesAsync();
        }
        #endregion

    #region CRUD para Tipo de Asentamiento

        // Validar si ya existe un tipo de asentamiento con el mismo nombre
        private async Task<bool> AsentamientoExistsAsync(string name, int? id = null)
        {
            return await _context.CatTipoAsentamiento
                .AnyAsync(a => a.TipoAsentamiento == name && (!id.HasValue || a.IdTipoAsentamiento != id));
        }

        // Obtener todos los tipos de asentamientos
        public async Task<IEnumerable<CatTipoAsentamiento>> GetAllTipoAsentamientosAsync() =>
            await _context.CatTipoAsentamiento.ToListAsync();

        // Obtener un tipo de asentamiento por ID
        public async Task<CatTipoAsentamiento> GetTipoAsentamientoByIdAsync(int id) =>
            await _context.CatTipoAsentamiento.FindAsync(id);

        // Obtener tipos de asentamientos por nombre
        public async Task<IEnumerable<CatTipoAsentamiento>> GetTipoAsentamientosByNameAsync(string name) =>
            await _context.CatTipoAsentamiento
                .Where(a => EF.Functions.Like(a.TipoAsentamiento, $"%{name}%"))
                .ToListAsync();

        // Crear un nuevo tipo de asentamiento
        public async Task<CatTipoAsentamiento> CreateTipoAsentamientoAsync(CatTipoAsentamiento asentamiento)
        {
            if (await AsentamientoExistsAsync(asentamiento.TipoAsentamiento))
                throw new System.Exception("Ya existe un tipo de asentamiento con ese nombre.");

            _context.CatTipoAsentamiento.Add(asentamiento);
            await _context.SaveChangesAsync();
            return asentamiento;
        }

        // Actualizar un tipo de asentamiento
        public async Task UpdateTipoAsentamientoAsync(int id, CatTipoAsentamiento asentamiento)
        {
            if (await AsentamientoExistsAsync(asentamiento.TipoAsentamiento, id))
                throw new System.Exception("Ya existe un tipo de asentamiento con ese nombre.");

            _context.Entry(asentamiento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Eliminar un tipo de asentamiento
        public async Task DeleteTipoAsentamientoAsync(int id)
        {
            var asentamiento = await _context.CatTipoAsentamiento.FindAsync(id);
            if (asentamiento == null) throw new KeyNotFoundException("Tipo de asentamiento no encontrado.");

            _context.CatTipoAsentamiento.Remove(asentamiento);
            await _context.SaveChangesAsync();
        }
        #endregion


        #region CRUD para CatCP

        // Validar si ya existe el mismo CP con el mismo IdEntidad
        private async Task<bool> CPExistsAsync(string codigoPostal, int idEntidad, int? id = null)
        {
            return await _context.CatCP
                .AnyAsync(c => c.CodigoPostal == codigoPostal && c.IdEntidad == idEntidad &&
                               (!id.HasValue || c.IdCP != id));
        }

        // Obtener todos los CP
        public async Task<IEnumerable<CatCP>> GetAllCPAsync() =>
            await _context.CatCP.ToListAsync();

        // Obtener CP por municipio
        public async Task<IEnumerable<CatCP>> GetAllCPByMunicipioAsync(int idMunicipio) =>
            await _context.CatCP.Where(c => c.IdMunicipio == idMunicipio).ToListAsync();

        // Obtener CP por entidad
        public async Task<IEnumerable<CatCP>> GetAllCPByEntidadAsync(int idEntidad) =>
            await _context.CatCP.Where(c => c.IdEntidad == idEntidad).ToListAsync();

        // Obtener un CP por ID
        public async Task<CatCP> GetCPByIdAsync(int id) =>
            await _context.CatCP.FindAsync(id);

        // Crear un nuevo CP con validación
        public async Task<CatCP> CreateCPAsync(CatCP cp)
        {
            if (await CPExistsAsync(cp.CodigoPostal, cp.IdEntidad))
                throw new System.Exception("Ya existe un CP con el mismo código postal para esta entidad.");

            _context.CatCP.Add(cp);
            await _context.SaveChangesAsync();
            return cp;
        }

        // Actualizar un CP con validación
        public async Task UpdateCPAsync(int id, CatCP cp)
        {
            if (await CPExistsAsync(cp.CodigoPostal, cp.IdEntidad, id))
                throw new System.Exception("Ya existe un CP con el mismo código postal para esta entidad.");

            _context.Entry(cp).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Eliminar un CP
        public async Task DeleteCPAsync(int id)
        {
            var cp = await _context.CatCP.FindAsync(id);
            if (cp == null) throw new KeyNotFoundException("CP no encontrado.");

            _context.CatCP.Remove(cp);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region CRUD para CatCiudades

        // Validar si ya existe una ciudad con el mismo nombre
        private async Task<bool> CiudadExistsAsync(string nombre, int? id = null)
        {
            return await _context.CatCiudades
                .AnyAsync(c => c.Nombre == nombre && (!id.HasValue || c.IdCiudad != id));
        }

        // Obtener todas las ciudades
        public async Task<IEnumerable<CatCiudades>> GetAllCiudadesAsync() =>
            await _context.CatCiudades.ToListAsync();

        // Obtener una ciudad por ID
        public async Task<CatCiudades> GetCiudadByIdAsync(int id) =>
            await _context.CatCiudades.FindAsync(id);

        // Obtener ciudades por nombre
        public async Task<IEnumerable<CatCiudades>> GetCiudadesByNameAsync(string name) =>
            await _context.CatCiudades
                .Where(c => EF.Functions.Like(c.Nombre, $"%{name}%"))
                .ToListAsync();

        // Crear una nueva ciudad con validación
        public async Task<CatCiudades> CreateCiudadAsync(CatCiudades ciudad)
        {
            if (await CiudadExistsAsync(ciudad.Nombre))
                throw new System.Exception("Ya existe una ciudad con ese nombre.");

            _context.CatCiudades.Add(ciudad);
            await _context.SaveChangesAsync();
            return ciudad;
        }

        // Actualizar una ciudad con validación
        public async Task UpdateCiudadAsync(int id, CatCiudades ciudad)
        {
            if (await CiudadExistsAsync(ciudad.Nombre, id))
                throw new System.Exception("Ya existe una ciudad con ese nombre.");

            _context.Entry(ciudad).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Eliminar una ciudad
        public async Task DeleteCiudadAsync(int id)
        {
            var ciudad = await _context.CatCiudades.FindAsync(id);
            if (ciudad == null) throw new KeyNotFoundException("Ciudad no encontrada.");

            _context.CatCiudades.Remove(ciudad);
            await _context.SaveChangesAsync();
        }

        #endregion

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
                .FromSqlRaw("EXEC Catalogo_GetAsentamientosByNames @CodigoPostalParam, @NombreAsentamientoParam, @TipoAsentamientoParam, @NombreMunicipioParam, @NombreCiudadParam, @AbreviaturaParam", parameters)
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

