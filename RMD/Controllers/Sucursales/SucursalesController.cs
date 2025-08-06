using RMD.Interface.Sucursales;
using RMD.Shared.Models.Sucursales;

namespace RMD.Controllers.Sucursales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class SucursalesController : ControllerBase
    {
        private readonly ISucursalService _sucursalService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public SucursalesController(ISucursalService sucursalService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _sucursalService = sucursalService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateSucursal([FromBody] CreateSucursalModel model)
        {
            if (!HasPermission("CreateSucursal"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var result = await _sucursalService.CreateSucursalAsync(model);
            return (result.Toast == "success" || result.Toast == "info")
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateSucursal(Guid id, [FromBody] UpdateSucursalModel model)
        {
            if (!HasPermission("UpdateSucursal"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (id == Guid.Empty || !ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var result = await _sucursalService.UpdateSucursalAsync(id, model);
            return (result.Toast == "success" || result.Toast == "info")
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSucursal(Guid id)
        {
            if (!HasPermission("DeleteSucursal"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (id == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var result = await _sucursalService.DeleteSucursalAsync(id);
            return (result.Toast == "success" || result.Toast == "info")
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSucursalById(Guid id)
        {
            if (!HasPermission("GetSucursalById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (id == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var result = await _sucursalService.GetSucursalByIdSucursalAsync(id);

            return (result.Toast == "success" || result.Toast == "info")
                ? Ok(result)
                : (result.Toast == "warning"
                    ? NotFound(result)
                    : BadRequest(result));
        }
                
        [HttpGet("gemp/{idGEMP:guid}")]
        public async Task<IActionResult> GetSucursalesByGEMP(Guid idGEMP)
        {
            if (!HasPermission("GetSucursalesByGEMP"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idGEMP == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var result = await _sucursalService.GetSucursalesByIdGEMPAsync(idGEMP);
            return (result.Toast == "success" || result.Toast == "info")
                ? Ok(result)
                : BadRequest(result);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.SucursalesController.EndpointRolesSucursalesController[endpointName].Contains(rol);
        }
    }
}
