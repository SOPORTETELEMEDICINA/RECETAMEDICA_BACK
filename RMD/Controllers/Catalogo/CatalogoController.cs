using RMD.Interface.Catalogo;
using RMD.Models.Catalogo;
using RMD.Models.Responses;

namespace RMD.Controllers.Catalogo
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController(ICatalogoService service) : ControllerBase
    {
        private readonly ICatalogoService _service = service;

        #region CRUD para Entidades Federativas

        [HttpGet("entidades")]
        public async Task<IActionResult> GetAllEntidades()
        {
            var entidades = await _service.GetAllEntidadesAsync();
            if (entidades == null || !entidades.Any())
                return NotFound(ResponseFromService<IEnumerable<CatEntidadesFederativas>>.Failure(HttpStatusCode.NotFound, "No se encontraron entidades."));

            return Ok(ResponseFromService<IEnumerable<CatEntidadesFederativas>>.Success(entidades));
        }

        [HttpGet("entidades/{id}")]
        public async Task<IActionResult> GetEntidadById(int id)
        {
            var entidad = await _service.GetEntidadByIdAsync(id);
            if (entidad == null)
                return NotFound(ResponseFromService<CatEntidadesFederativas>.Failure(HttpStatusCode.NotFound, "Entidad no encontrada."));

            return Ok(ResponseFromService<CatEntidadesFederativas>.Success(entidad));
        }

        [HttpPost("entidades")]
        public async Task<IActionResult> CreateEntidad([FromBody] CatEntidadesFederativas entidad)
        {
            entidad.IdEntidad = 0;  // Aseguramos que no se reciba ni se utilice un ID preexistente

            try
            {
                var createdEntidad = await _service.CreateEntidadAsync(entidad);
                return Ok(ResponseFromService<CatEntidadesFederativas>.Success(createdEntidad, "Entidad creada exitosamente."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseFromService<CatEntidadesFederativas>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }


        [HttpPut("entidades/{id}")]
        public async Task<IActionResult> UpdateEntidad(int id, [FromBody] CatEntidadesFederativas entidad)
        {
            if (id != entidad.IdEntidad)
                return BadRequest(ResponseFromService<CatEntidadesFederativas>.Failure(HttpStatusCode.BadRequest, "ID no coincide."));

            try
            {
                await _service.UpdateEntidadAsync(id, entidad);
                return Ok(ResponseFromService<string>.Success("Entidad actualizada exitosamente."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [HttpDelete("entidades/{id}")]
        public async Task<IActionResult> DeleteEntidad(int id)
        {
            try
            {
                await _service.DeleteEntidadAsync(id);
                return Ok(ResponseFromService<string>.Success("Entidad eliminada exitosamente."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        #endregion

        #region CRUD para Municipios

        [HttpGet("municipiosbyEntidad/{idEntidad}")]
        public async Task<IActionResult> GetAllMunicipiosByIdEntidad(int idEntidad)
        {
            var municipios = await _service.GetAllMunicipiosByIdEntidadAsync(idEntidad);
            if (municipios == null || !municipios.Any())
                return NotFound(ResponseFromService<IEnumerable<CatMunicipios>>.Failure(HttpStatusCode.NotFound, "No se encontraron municipios para la entidad especificada."));

            return Ok(ResponseFromService<IEnumerable<CatMunicipios>>.Success(municipios));
        }

        [HttpGet("municipios/{id}")]
        public async Task<IActionResult> GetMunicipioById(int id)
        {
            var municipio = await _service.GetMunicipioByIdAsync(id);
            if (municipio == null)
                return NotFound(ResponseFromService<CatMunicipios>.Failure(HttpStatusCode.NotFound, "Municipio no encontrado."));

            return Ok(ResponseFromService<CatMunicipios>.Success(municipio));
        }

        [HttpPost("municipios")]
        public async Task<IActionResult> CreateMunicipio([FromBody] CatMunicipios municipio)
        {
            municipio.IdMunicipio = 0;  // Aseguramos que el ID no se reciba ni se utilice

            try
            {
                var createdMunicipio = await _service.CreateMunicipioAsync(municipio);
                return Ok(ResponseFromService<CatMunicipios>.Success(createdMunicipio, "Municipio creado exitosamente."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<CatMunicipios>.Failure(HttpStatusCode.InternalServerError, ex.Message));
            }
        }


        [HttpPut("municipios/{id}")]
        public async Task<IActionResult> UpdateMunicipio(int id, [FromBody] CatMunicipios municipio)
        {
            if (id != municipio.IdMunicipio)
                return BadRequest(ResponseFromService<CatMunicipios>.Failure(HttpStatusCode.BadRequest, "ID no coincide."));

            try
            {
                await _service.UpdateMunicipioAsync(id, municipio);
                return Ok(ResponseFromService<string>.Success("Municipio actualizado exitosamente."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpDelete("municipios/{id}")]
        public async Task<IActionResult> DeleteMunicipio(int id)
        {
            try
            {
                await _service.DeleteMunicipioAsync(id);
                return Ok(ResponseFromService<string>.Success("Municipio eliminado exitosamente."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        #endregion

        #region CRUD para Tipo de Asentamiento

        [HttpGet("tipoAsentamientos")]
        public async Task<IActionResult> GetAllTipoAsentamientos()
        {
            var asentamientos = await _service.GetAllTipoAsentamientosAsync();
            if (asentamientos == null || !asentamientos.Any())
                return NotFound(ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Failure(HttpStatusCode.NotFound, "No se encontraron tipos de asentamientos."));

            return Ok(ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Success(asentamientos));
        }

        [HttpGet("tipoAsentamientos/{id}")]
        public async Task<IActionResult> GetTipoAsentamientoById(int id)
        {
            var asentamiento = await _service.GetTipoAsentamientoByIdAsync(id);
            if (asentamiento == null)
                return NotFound(ResponseFromService<CatTipoAsentamiento>.Failure(HttpStatusCode.NotFound, "Tipo de asentamiento no encontrado."));

            return Ok(ResponseFromService<CatTipoAsentamiento>.Success(asentamiento));
        }

        [HttpGet("tipoAsentamientos/byName/{name}")]
        public async Task<IActionResult> GetTipoAsentamientosByName(string name)
        {
            var asentamientos = await _service.GetTipoAsentamientosByNameAsync(name);
            if (asentamientos == null || !asentamientos.Any())
                return NotFound(ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Failure(HttpStatusCode.NotFound, "No se encontraron tipos de asentamientos con ese nombre."));

            return Ok(ResponseFromService<IEnumerable<CatTipoAsentamiento>>.Success(asentamientos));
        }

        [HttpPost("tipoAsentamientos")]
        public async Task<IActionResult> CreateTipoAsentamiento([FromBody] CatTipoAsentamiento asentamiento)
        {
            asentamiento.IdTipoAsentamiento = 0;  // Aseguramos que no se reciba ni utilice un ID preexistente

            try
            {
                var createdAsentamiento = await _service.CreateTipoAsentamientoAsync(asentamiento);
                return Ok(ResponseFromService<CatTipoAsentamiento>.Success(createdAsentamiento, "Tipo de asentamiento creado exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<CatTipoAsentamiento>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }


        [HttpPut("tipoAsentamientos/{id}")]
        public async Task<IActionResult> UpdateTipoAsentamiento(int id, [FromBody] CatTipoAsentamiento asentamiento)
        {
            if (id != asentamiento.IdTipoAsentamiento)
                return BadRequest(ResponseFromService<CatTipoAsentamiento>.Failure(HttpStatusCode.BadRequest, "ID no coincide."));

            try
            {
                await _service.UpdateTipoAsentamientoAsync(id, asentamiento);
                return Ok(ResponseFromService<string>.Success("Tipo de asentamiento actualizado exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [HttpDelete("tipoAsentamientos/{id}")]
        public async Task<IActionResult> DeleteAsentamiento(int id)
        {
            try
            {
                await _service.DeleteTipoAsentamientoAsync(id);
                return Ok(ResponseFromService<string>.Success("Tipo de asentamiento eliminado exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        #endregion

        #region CRUD para CatCP

        [HttpGet("cp")]
        public async Task<IActionResult> GetAllCP()
        {
            var cps = await _service.GetAllCPAsync();
            if (cps == null || !cps.Any())
                return NotFound(ResponseFromService<IEnumerable<CatCP>>.Failure(HttpStatusCode.NotFound, "No se encontraron códigos postales."));

            return Ok(ResponseFromService<IEnumerable<CatCP>>.Success(cps));
        }

        [HttpGet("cp/byMunicipio/{idMunicipio}")]
        public async Task<IActionResult> GetAllCPByMunicipio(int idMunicipio)
        {
            var cps = await _service.GetAllCPByMunicipioAsync(idMunicipio);
            if (cps == null || !cps.Any())
                return NotFound(ResponseFromService<IEnumerable<CatCP>>.Failure(HttpStatusCode.NotFound, "No se encontraron CPs para el municipio especificado."));

            return Ok(ResponseFromService<IEnumerable<CatCP>>.Success(cps));
        }

        [HttpGet("cp/byEntidad/{idEntidad}")]
        public async Task<IActionResult> GetAllCPByEntidad(int idEntidad)
        {
            var cps = await _service.GetAllCPByEntidadAsync(idEntidad);
            if (cps == null || !cps.Any())
                return NotFound(ResponseFromService<IEnumerable<CatCP>>.Failure(HttpStatusCode.NotFound, "No se encontraron CPs para la entidad especificada."));

            return Ok(ResponseFromService<IEnumerable<CatCP>>.Success(cps));
        }

        [HttpGet("cp/{id}")]
        public async Task<IActionResult> GetCPById(int id)
        {
            var cp = await _service.GetCPByIdAsync(id);
            if (cp == null)
                return NotFound(ResponseFromService<CatCP>.Failure(HttpStatusCode.NotFound, "CP no encontrado."));

            return Ok(ResponseFromService<CatCP>.Success(cp));
        }

        [HttpPost("cp")]
        public async Task<IActionResult> CreateCP([FromBody] CatCP cp)
        {
            cp.IdCP = 0;  // Aseguramos que no se reciba ni utilice un ID preexistente

            try
            {
                var createdCP = await _service.CreateCPAsync(cp);
                return Ok(ResponseFromService<CatCP>.Success(createdCP, "CP creado exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<CatCP>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }


        [HttpPut("cp/{id}")]
        public async Task<IActionResult> UpdateCP(int id, [FromBody] CatCP cp)
        {
            if (id != cp.IdCP)
                return BadRequest(ResponseFromService<CatCP>.Failure(HttpStatusCode.BadRequest, "ID no coincide."));

            try
            {
                await _service.UpdateCPAsync(id, cp);
                return Ok(ResponseFromService<string>.Success("CP actualizado exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [HttpDelete("cp/{id}")]
        public async Task<IActionResult> DeleteCP(int id)
        {
            try
            {
                await _service.DeleteCPAsync(id);
                return Ok(ResponseFromService<string>.Success("CP eliminado exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        #endregion

        #region CRUD para CatCiudades

        [HttpGet("ciudades")]
        public async Task<IActionResult> GetAllCiudades()
        {
            var ciudades = await _service.GetAllCiudadesAsync();
            if (ciudades == null || !ciudades.Any())
                return NotFound(ResponseFromService<IEnumerable<CatCiudades>>.Failure(HttpStatusCode.NotFound, "No se encontraron ciudades."));

            return Ok(ResponseFromService<IEnumerable<CatCiudades>>.Success(ciudades));
        }

        [HttpGet("ciudades/{id}")]
        public async Task<IActionResult> GetCiudadById(int id)
        {
            var ciudad = await _service.GetCiudadByIdAsync(id);
            if (ciudad == null)
                return NotFound(ResponseFromService<CatCiudades>.Failure(HttpStatusCode.NotFound, "Ciudad no encontrada."));

            return Ok(ResponseFromService<CatCiudades>.Success(ciudad));
        }

        [HttpGet("ciudades/byName/{name}")]
        public async Task<IActionResult> GetCiudadesByName(string name)
        {
            var ciudades = await _service.GetCiudadesByNameAsync(name);
            if (ciudades == null || !ciudades.Any())
                return NotFound(ResponseFromService<IEnumerable<CatCiudades>>.Failure(HttpStatusCode.NotFound, "No se encontraron ciudades con ese nombre."));

            return Ok(ResponseFromService<IEnumerable<CatCiudades>>.Success(ciudades));
        }

        [HttpPost("ciudades")]
        public async Task<IActionResult> CreateCiudad([FromBody] CatCiudades ciudad)
        {
            ciudad.IdCiudad = 0;  // Aseguramos que no se utilice un ID preexistente

            try
            {
                var createdCiudad = await _service.CreateCiudadAsync(ciudad);
                return Ok(ResponseFromService<CatCiudades>.Success(createdCiudad, "Ciudad creada exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<CatCiudades>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }


        [HttpPut("ciudades/{id}")]
        public async Task<IActionResult> UpdateCiudad(int id, [FromBody] CatCiudades ciudad)
        {
            if (id != ciudad.IdCiudad)
                return BadRequest(ResponseFromService<CatCiudades>.Failure(HttpStatusCode.BadRequest, "ID no coincide."));

            try
            {
                await _service.UpdateCiudadAsync(id, ciudad);
                return Ok(ResponseFromService<string>.Success("Ciudad actualizada exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        [HttpDelete("ciudades/{id}")]
        public async Task<IActionResult> DeleteCiudad(int id)
        {
            try
            {
                await _service.DeleteCiudadAsync(id);
                return Ok(ResponseFromService<string>.Success("Ciudad eliminada exitosamente."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
        }

        #endregion
    }
}
