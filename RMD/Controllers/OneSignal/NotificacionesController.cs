using RMD.Interface.OneSignal;
using RMD.Shared.Models.OneSignal.Request;

namespace RMD.Controllers.OneSignal
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public sealed class NotificacionesController : ControllerBase
    {
        private readonly IOneSignalNotificationService _svc;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public NotificacionesController(
            IOneSignalNotificationService svc,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _svc = svc;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        public NotificacionesController(IOneSignalNotificationService svc)
        {
            _svc = svc;
        }

        [HttpPost("ProgramarToma")]
        public async Task<IActionResult> ProgramarToma(
            [FromQuery] Guid idPaciente,
            [FromBody] List<ProgramarTomaRequest> req,
            CancellationToken ct)
        {
            if (!HasPermission("ProgramarToma"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<bool>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                notif.Mensaje = errores;
                return BadRequest(ResponseFromService<bool>.Failure(notif));
            }

            if (idPaciente == Guid.Empty || req is null || req.Count == 0)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<bool>.Failure(notif));
            }

            var result = await _svc.ProgramarRecordatorioTomaAsync(req, ct);

            if (result.Toast == "success" || result.Toast == "info")
                return Ok(result);

            return BadRequest(result);
        }

        // NUEVO: Posponer (snooze) una notificación programada
        [HttpPost("PosponerNotificacion")]
        public async Task<IActionResult> PosponerNotificacion([FromBody] PosponerNotificacionRequest req)
        {
            if (!HasPermission("PosponerNotificacion"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<bool>.Failure(notif));
            }

            if (!ModelState.IsValid || req is null)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");

                if (!string.IsNullOrWhiteSpace(errores))
                    notif.Mensaje = errores;

                return BadRequest(ResponseFromService<bool>.Failure(notif));
            }

            var result = await _svc.PosponerNotificacionAsync(req);

            // estándar: éxito si SUCCESS o INFO
            if (result.Toast == "success" || result.Toast == "info")
                return Ok(result);

            return BadRequest(result);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.NotificacionesController
                .EndpointRolesNotificacionesController[endpointName]
                .Contains(rol);
        }
    }
}
