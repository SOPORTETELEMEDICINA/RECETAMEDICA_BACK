using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatMunicipiosController : ControllerBase
    {
        private readonly ICatMunicipiosService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatMunicipiosController(ICatMunicipiosService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        #region CRUD para Municipios
        [HttpGet("municipiosbyEntidad/{idEntidad}")]
        public async Task<IActionResult> GetAllMunicipiosByIdEntidad(int idEntidad)
        {
            if (!HasPermission("GetAllMunicipiosByIdEntidad"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllMunicipiosByIdEntidadAsync(idEntidad);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("municipios/{id}")]
        public async Task<IActionResult> GetMunicipioById(int id)
        {
            if (!HasPermission("GetMunicipioById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetMunicipioByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        //[HttpPost("municipios")]
        //public async Task<IActionResult> CreateMunicipio([FromBody] CatMunicipios municipio)
        //{
        //    if (!HasPermission("CreateMunicipio"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    municipio.IdMunicipio = 0;
        //    var response = await _service.CreateMunicipioAsync(municipio);
        //    return (response.Toast == "success" || response.Toast == "info")
        //        ? Ok(response)
        //        : BadRequest(response);
        //}

        [HttpPut("municipios/{id}")]
        public async Task<IActionResult> UpdateMunicipio(int id, [FromBody] CatMunicipios municipio)
        {
            if (!HasPermission("UpdateMunicipio"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.UpdateMunicipioAsync(id, municipio);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        //[HttpDelete("municipios/{id}")]
        //public async Task<IActionResult> DeleteMunicipio(int id)
        //{
        //    if (!HasPermission("DeleteMunicipio"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    var response = await _service.DeleteMunicipioAsync(id);
        //    return (response.Toast == "success" || response.Toast == "info")
        //        ? Ok(response)
        //        : BadRequest(response);
        //}

        #endregion
        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.CatalogoController.EndpointRolesCatalogoController[endpointName].Contains(rol);
        }

    }
}
