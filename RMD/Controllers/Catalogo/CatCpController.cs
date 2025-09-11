using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatCpController : ControllerBase
    {
        private readonly ICatCpService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatCpController(ICatCpService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para CatCP

        [HttpGet("cp")]
        public async Task<IActionResult> GetAllCP()
        {
            if (!HasPermission("GetAllCP"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllCPAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("cp/byMunicipio/{idMunicipio}")]
        public async Task<IActionResult> GetAllCPByMunicipio(int idMunicipio)
        {
            if (!HasPermission("GetAllCPByMunicipio"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllCPByMunicipioAsync(idMunicipio);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("cp/byEntidad/{idEntidad}")]
        public async Task<IActionResult> GetAllCPByEntidad(int idEntidad)
        {
            if (!HasPermission("GetAllCPByEntidad"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllCPByEntidadAsync(idEntidad);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("cp/{id}")]
        public async Task<IActionResult> GetCPById(int id)
        {
            if (!HasPermission("GetCPById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetCPByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        //[HttpPost("cp")]
        //public async Task<IActionResult> CreateCP([FromBody] CatCP cp)
        //{
        //    if (!HasPermission("CreateCP"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    cp.IdCP = 0;
        //    var response = await _service.CreateCPAsync(cp);
        //    return (response.Toast == "success" || response.Toast == "info")
        //        ? Ok(response)
        //        : BadRequest(response);
        //}

        [HttpPut("cp/{id}")]
        public async Task<IActionResult> UpdateCP(int id, [FromBody] CatCP cp)
        {
            if (!HasPermission("UpdateCP"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.UpdateCPAsync(id, cp);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        //[HttpDelete("cp/{id}")]
        //public async Task<IActionResult> DeleteCP(int id)
        //{
        //    if (!HasPermission("DeleteCP"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    var response = await _service.DeleteCPAsync(id);
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
