using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatTipoAsentamientoController : ControllerBase
    {
        private readonly ICatTipoAsentamientoService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatTipoAsentamientoController(ICatTipoAsentamientoService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        #region CRUD para Tipo de Asentamiento

        [HttpGet("tipoAsentamientos")]
        public async Task<IActionResult> GetAllTipoAsentamientos()
        {
            if (!HasPermission("GetAllTipoAsentamientos"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllTipoAsentamientosAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("tipoAsentamientos/{id}")]
        public async Task<IActionResult> GetTipoAsentamientoById(int id)
        {
            if (!HasPermission("GetTipoAsentamientoById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetTipoAsentamientoByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("tipoAsentamientos/byName/{name}")]
        public async Task<IActionResult> GetTipoAsentamientosByName(string name)
        {
            if (!HasPermission("GetTipoAsentamientosByName"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetTipoAsentamientosByNameAsync(name);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        //[HttpPost("tipoAsentamientos")]
        //public async Task<IActionResult> CreateTipoAsentamiento([FromBody] CatTipoAsentamiento asentamiento)
        //{
        //    if (!HasPermission("CreateTipoAsentamiento"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    asentamiento.IdTipoAsentamiento = 0;
        //    var response = await _service.CreateTipoAsentamientoAsync(asentamiento);
        //    return (response.Toast == "success" || response.Toast == "info")
        //        ? Ok(response)
        //        : BadRequest(response);
        //}

        [HttpPut("tipoAsentamientos/{id}")]
        public async Task<IActionResult> UpdateTipoAsentamiento(int id, [FromBody] CatTipoAsentamiento asentamiento)
        {
            if (!HasPermission("UpdateTipoAsentamiento"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.UpdateTipoAsentamientoAsync(id, asentamiento);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        //[HttpDelete("tipoAsentamientos/{id}")]
        //public async Task<IActionResult> DeleteTipoAsentamiento(int id)
        //{
        //    if (!HasPermission("DeleteTipoAsentamiento"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    var response = await _service.DeleteTipoAsentamientoAsync(id);
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
