using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatAsentamientoCiudadController : ControllerBase
    {
        private readonly ICatAsentamientoCiudadService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatAsentamientoCiudadController(ICatAsentamientoCiudadService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para CatAsentamientoCiudad

        [HttpGet("asentamientosCiudad")]
        public async Task<IActionResult> GetAllAsentamientoCiudad()
        {
            if (!HasPermission("GetAllAsentamientoCiudad"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllAsentamientoCiudadAsync();
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : NotFound(response);
        }

        [HttpGet("asentamientosCiudad/{id}")]
        public async Task<IActionResult> GetAsentamientoCiudadById(int id)
        {
            if (!HasPermission("GetAsentamientoCiudadById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAsentamientoCiudadByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : NotFound(response);
        }

        //[HttpPost("asentamientosCiudad")]
        //public async Task<IActionResult> CreateAsentamientoCiudad([FromBody] CatAsentamientoCiudad model)
        //{
        //    if (!HasPermission("CreateAsentamientoCiudad"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    var response = await _service.CreateAsentamientoCiudadAsync(model);
        //    return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        //}

        [HttpPut("asentamientosCiudad/{id}")]
        public async Task<IActionResult> UpdateAsentamientoCiudad(int id, [FromBody] CatAsentamientoCiudad model)
        {
            if (!HasPermission("UpdateAsentamientoCiudad"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.UpdateAsentamientoCiudadAsync(id, model);
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        //[HttpDelete("asentamientosCiudad/{id}")]
        //public async Task<IActionResult> DeleteAsentamientoCiudad(int id)
        //{
        //    if (!HasPermission("DeleteAsentamientoCiudad"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    var response = await _service.DeleteAsentamientoCiudadAsync(id);
        //    return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        //}

        #endregion
        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.CatalogoController.EndpointRolesCatalogoController[endpointName].Contains(rol);
        }
    }
}
