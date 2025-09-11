using RMD.Interface.Receta;
using RMD.Shared.Models.Receta.AlertaToma.Request;

namespace RMD.Controllers.Receta
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class AlertasController : ControllerBase
    {
        private readonly IAlertaTomaService _alertaTomaService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public AlertasController(
            IAlertaTomaService alertaTomaService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _alertaTomaService = alertaTomaService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("ActivarAlertaToma")]
        public async Task<IActionResult> ActivarAlertaToma([FromBody] ActivarAlertaTomaRequest request)
        {
            if (!HasPermission("ActivarAlertaToma"))
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

            var response = await _alertaTomaService.ActivarAlertaTomaAsync(request, idUsuario);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("DesactivarAlertaToma")]
        public async Task<IActionResult> DesactivarAlertaToma([FromBody] DesactivarAlertaTomaRequest request)
        {
            if (!HasPermission("DesactivarAlertaToma"))
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

            var response = await _alertaTomaService.DesactivarAlertaTomaAsync(request, idUsuario);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.AlertasController.EndpointRolesAlertasController[endpointName].Contains(rol);
        }
    }
}
