using RMD.Interface.Catalogo;
using RMD.Interface.Notificaciones;
using RMD.Models.Catalogo;
using RMD.Models.Responses;

namespace RMD.Controllers.Catalogo
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatalogoController(ICatalogoService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para Entidades Federativas
        [Route("api/[controller]")]
        [ApiController]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public class EntidadesController : ControllerBase
        {
            private readonly ICatalogoService _service;
            private readonly ICatalogoNotificacionService _catalogoNotificacionService;

            public EntidadesController(ICatalogoService service, ICatalogoNotificacionService catalogoNotificacionService)
            {
                _service = service;
                _catalogoNotificacionService = catalogoNotificacionService;
            }

            [HttpGet]
            public async Task<IActionResult> GetAllEntidades()
            {
                var response = await _service.GetAllEntidadesAsync();
                return (response.Toast == "success" || response.Toast == "info")
                    ? Ok(response)
                    : BadRequest(response);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetEntidadById(int id)
            {
                if (id <= 0)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }

                var response = await _service.GetEntidadByIdAsync(id);
                return (response.Toast == "success" || response.Toast == "info")
                    ? Ok(response)
                    : BadRequest(response);
            }

            [HttpPost]
            public async Task<IActionResult> CreateEntidad([FromBody] CatEntidadesFederativas entidad)
            {
                if (!ModelState.IsValid || entidad == null)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                    notif.Mensaje = errores;
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }

                var response = await _service.CreateEntidadAsync(entidad);
                return (response.Toast == "success" || response.Toast == "info")
                    ? Ok(response)
                    : BadRequest(response);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateEntidad(int id, [FromBody] CatEntidadesFederativas entidad)
            {
                if (!ModelState.IsValid || entidad == null)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                    notif.Mensaje = errores;
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }

                var response = await _service.UpdateEntidadAsync(id, entidad);
                return (response.Toast == "success" || response.Toast == "info")
                    ? Ok(response)
                    : BadRequest(response);
            }
        }

        [HttpDelete("entidades/{id}")]
        public async Task<IActionResult> DeleteEntidad(int id)
        {
            if (id <= 0)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _service.DeleteEntidadAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }


        #endregion

        #region CRUD para Municipios
        [HttpGet("municipiosbyEntidad/{idEntidad}")]
        public async Task<IActionResult> GetAllMunicipiosByIdEntidad(int idEntidad)
        {
            var response = await _service.GetAllMunicipiosByIdEntidadAsync(idEntidad);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("municipios/{id}")]
        public async Task<IActionResult> GetMunicipioById(int id)
        {
            var response = await _service.GetMunicipioByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpPost("municipios")]
        public async Task<IActionResult> CreateMunicipio([FromBody] CatMunicipios municipio)
        {
            municipio.IdMunicipio = 0;
            var response = await _service.CreateMunicipioAsync(municipio);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut("municipios/{id}")]
        public async Task<IActionResult> UpdateMunicipio(int id, [FromBody] CatMunicipios municipio)
        {
            var response = await _service.UpdateMunicipioAsync(id, municipio);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("municipios/{id}")]
        public async Task<IActionResult> DeleteMunicipio(int id)
        {
            var response = await _service.DeleteMunicipioAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        #endregion

        #region CRUD para Tipo de Asentamiento

        [HttpGet("tipoAsentamientos")]
        public async Task<IActionResult> GetAllTipoAsentamientos()
        {
            var response = await _service.GetAllTipoAsentamientosAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("tipoAsentamientos/{id}")]
        public async Task<IActionResult> GetTipoAsentamientoById(int id)
        {
            var response = await _service.GetTipoAsentamientoByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("tipoAsentamientos/byName/{name}")]
        public async Task<IActionResult> GetTipoAsentamientosByName(string name)
        {
            var response = await _service.GetTipoAsentamientosByNameAsync(name);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpPost("tipoAsentamientos")]
        public async Task<IActionResult> CreateTipoAsentamiento([FromBody] CatTipoAsentamiento asentamiento)
        {
            asentamiento.IdTipoAsentamiento = 0;
            var response = await _service.CreateTipoAsentamientoAsync(asentamiento);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut("tipoAsentamientos/{id}")]
        public async Task<IActionResult> UpdateTipoAsentamiento(int id, [FromBody] CatTipoAsentamiento asentamiento)
        {
            var response = await _service.UpdateTipoAsentamientoAsync(id, asentamiento);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("tipoAsentamientos/{id}")]
        public async Task<IActionResult> DeleteAsentamiento(int id)
        {
            var response = await _service.DeleteTipoAsentamientoAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        #endregion

        #region CRUD para CatCP

        [HttpGet("cp")]
        public async Task<IActionResult> GetAllCP()
        {
            var response = await _service.GetAllCPAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("cp/byMunicipio/{idMunicipio}")]
        public async Task<IActionResult> GetAllCPByMunicipio(int idMunicipio)
        {
            var response = await _service.GetAllCPByMunicipioAsync(idMunicipio);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("cp/byEntidad/{idEntidad}")]
        public async Task<IActionResult> GetAllCPByEntidad(int idEntidad)
        {
            var response = await _service.GetAllCPByEntidadAsync(idEntidad);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("cp/{id}")]
        public async Task<IActionResult> GetCPById(int id)
        {
            var response = await _service.GetCPByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpPost("cp")]
        public async Task<IActionResult> CreateCP([FromBody] CatCP cp)
        {
            cp.IdCP = 0;
            var response = await _service.CreateCPAsync(cp);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut("cp/{id}")]
        public async Task<IActionResult> UpdateCP(int id, [FromBody] CatCP cp)
        {
            var response = await _service.UpdateCPAsync(id, cp);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("cp/{id}")]
        public async Task<IActionResult> DeleteCP(int id)
        {
            var response = await _service.DeleteCPAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        #endregion

        #region CRUD para CatCiudades

        [HttpGet("ciudades")]
        public async Task<IActionResult> GetAllCiudades()
        {
            var response = await _service.GetAllCiudadesAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("ciudades/{id}")]
        public async Task<IActionResult> GetCiudadById(int id)
        {
            var response = await _service.GetCiudadByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("ciudades/byName/{name}")]
        public async Task<IActionResult> GetCiudadesByName(string name)
        {
            var response = await _service.GetCiudadesByNameAsync(name);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpPost("ciudades")]
        public async Task<IActionResult> CreateCiudad([FromBody] CatCiudades ciudad)
        {
            ciudad.IdCiudad = 0;
            var response = await _service.CreateCiudadAsync(ciudad);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut("ciudades/{id}")]
        public async Task<IActionResult> UpdateCiudad(int id, [FromBody] CatCiudades ciudad)
        {
            var response = await _service.UpdateCiudadAsync(id, ciudad);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("ciudades/{id}")]
        public async Task<IActionResult> DeleteCiudad(int id)
        {
            var response = await _service.DeleteCiudadAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }


        #endregion

        #region CRUD para CatEventosDeSalud

        [HttpGet("eventosSalud")]
        public async Task<IActionResult> GetAllEventosSalud()
        {
            var response = await _service.GetAllEventosSaludAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("eventosSalud/{id}")]
        public async Task<IActionResult> GetEventoSaludById(int id)
        {
            var response = await _service.GetEventoSaludByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpPost("eventosSalud")]
        public async Task<IActionResult> CreateEventoSalud([FromBody] CatEventosDeSalud evento)
        {
            evento.IdEvento = 0;
            var response = await _service.CreateEventoSaludAsync(evento);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut("eventosSalud/{id}")]
        public async Task<IActionResult> UpdateEventoSalud(int id, [FromBody] CatEventosDeSalud evento)
        {
            var response = await _service.UpdateEventoSaludAsync(id, evento);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("eventosSalud/{id}")]
        public async Task<IActionResult> DeleteEventoSalud(int id)
        {
            var response = await _service.DeleteEventoSaludAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }


        #endregion

    }
}
