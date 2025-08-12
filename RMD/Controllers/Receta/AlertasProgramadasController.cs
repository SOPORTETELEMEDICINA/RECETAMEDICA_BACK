using RMD.Interface.Receta;
using RMD.Shared.Models.Receta.AlertaToma.Request;

namespace RMD.Controllers.Receta
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class AlertasProgramadasController : ControllerBase
    {
        private readonly IAlertasProgramadasService _alertasProgramadasService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public AlertasProgramadasController(
            IAlertasProgramadasService alertasProgramadasService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _alertasProgramadasService = alertasProgramadasService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("GetAlertasProgramadas")]
        public async Task<IActionResult> GetAlertasProgramadas([FromBody] GetAlertasProgramadasRequest request)
        {
            if (!HasPermission("GetAlertasProgramadas"))
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

            var response = await _alertasProgramadasService.GetAlertasProgramadasAsync(request);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.AlertasProgramadasController.EndpointRolesAlertasProgramadasController[endpointName].Contains(rol);
        }
    }
}
