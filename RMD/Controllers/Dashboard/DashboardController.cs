using RMD.Interface.Dashboard;

namespace RMD.Controllers.Dashboard
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public DashboardController(
            IDashboardService dashboardService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _dashboardService = dashboardService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("sucursales-pacientes")]
        public async Task<IActionResult> GetSucursalPacientes()
        {
            // 1) Permisos
            if (!HasPermission("GetSucursalPacientes"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Extraer y validar claims
            var gempClaim = User.FindFirstValue("GEMP");
            var sucursalClaim = User.FindFirstValue("IdSucursal");
            var rolClaim = User.FindFirstValue("IdRol");
            if (!Guid.TryParse(gempClaim, out var idGemp)
                || !Guid.TryParse(sucursalClaim, out var idSucursal)
                || !Guid.TryParse(rolClaim, out var idRol))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamar al service
            var response = await _dashboardService
                .GetSucursalesPacientesAsync(idGemp, idSucursal, idRol);

            // 4) Evaluar Toast y devolver
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("GetKPIPacientesRecetas")]
        public async Task<IActionResult> GetKpiPacientesRecetas()
        {
            // 1) Permisos
            if (!HasPermission("GetKpiPacientesRecetas"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Extraer y validar claims
            var usuarioClaim = User.FindFirstValue("IdUsuario");
            var rolClaim = User.FindFirstValue("IdRol");
            if (!Guid.TryParse(usuarioClaim, out var idUsuario)
                || !Guid.TryParse(rolClaim, out var idRol))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamar al service
            var response = await _dashboardService
                .GetKPIPacientesRecetasAsync(idUsuario, idRol);

            // 4) Evaluar Toast y devolver
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.DashBoardController
                       .EndpointRolesDashBoardController[endpointName]
                   .Contains(rol);
        }
    }
}
