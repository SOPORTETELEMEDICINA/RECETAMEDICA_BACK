using RMD.Interface.Receta;
using RMD.Shared.Models.Receta.AlertaToma.Request;

namespace RMD.Controllers.Receta
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class AlertaManualController : ControllerBase
    {
        private readonly IAlertaTomaService _alertaTomaService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public AlertaManualController(
            IAlertaTomaService alertaTomaService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _alertaTomaService = alertaTomaService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("BuscarPorNombre")]
        public async Task<IActionResult> BuscarPorNombre([FromBody] BuscarPackageRequest request)
        {
            if (!HasPermission("BuscarPorNombre"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.NombrePackage))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _alertaTomaService.BuscarPackagePorNombreAsync(request.NombrePackage, idUsuario);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("ActivarAlertaTomaManual")]
        public async Task<IActionResult> ActivarAlertaTomaManual([FromBody] ActivarAlertaManualRequest request)
        {
            if (!HasPermission("ActivarAlertaTomaManual"))
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

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _alertaTomaService.ActivarAlertaManualAsync(request, idUsuario);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("DesactivarAlertaTomaManual")]
        public async Task<IActionResult> DesactivarAlertaTomaManual([FromBody] DesactivarAlertaManualRequest request)
        {
            if (!HasPermission("DesactivarAlertaTomaManual"))
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

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _alertaTomaService.DesactivarAlertaManualAsync(request, idUsuario);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }


        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.AlertaManualController.EndpointRolesAlertaManualController[endpointName].Contains(rol);
        }
    }
}
