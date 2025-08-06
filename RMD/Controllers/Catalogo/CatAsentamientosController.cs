using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatAsentamientosController : ControllerBase
    {
        private readonly ICatAsentamientosService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatAsentamientosController(ICatAsentamientosService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para CatAsentamientos

        [HttpGet("asentamientos")]
        public async Task<IActionResult> GetAllAsentamientos()
        {
            if (!HasPermission("GetAllAsentamientos"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllAsentamientosAsync();
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : NotFound(response);
        }

        [HttpGet("asentamientos/{id}")]
        public async Task<IActionResult> GetAsentamientoById(int id)
        {
            if (!HasPermission("GetAsentamientoById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAsentamientoByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : NotFound(response);
        }

        //[HttpPost("asentamientos")]
        //public async Task<IActionResult> CreateAsentamiento([FromBody] CatAsentamientos asentamiento)
        //{
        //    if (!HasPermission("CreateAsentamiento"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    asentamiento.IdAsentamiento = 0;
        //    var response = await _service.CreateAsentamientoAsync(asentamiento);
        //    return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        //}

        [HttpPut("asentamientos/{id}")]
        public async Task<IActionResult> UpdateAsentamiento(int id, [FromBody] CatAsentamientos asentamiento)
        {
            if (!HasPermission("UpdateAsentamiento"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.UpdateAsentamientoAsync(id, asentamiento);
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        //[HttpDelete("asentamientos/{id}")]
        //public async Task<IActionResult> DeleteAsentamiento(int id)
        //{
        //    if (!HasPermission("DeleteAsentamiento"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    var response = await _service.DeleteAsentamientoAsync(id);
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
