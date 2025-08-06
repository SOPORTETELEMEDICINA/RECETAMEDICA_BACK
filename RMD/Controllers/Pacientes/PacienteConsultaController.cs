using RMD.Interface.Pacientes;


namespace RMD.Controllers.Pacientes
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class PacienteConsultaController : Controller
    {
        private readonly IPacienteService _pacienteService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public PacienteConsultaController(
            IPacienteService pacienteService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _pacienteService = pacienteService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        [HttpGet("BySucursal/{idSucursal}")]
        public async Task<IActionResult> GetPacientesBySucursal(Guid idSucursal)
        {
            if (!HasPermission("GetPacientesBySucursal"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idSucursal == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _pacienteService.GetPacientesBySucursalAsync(idSucursal);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("ByGEMP/{idGEMP}")]
        public async Task<IActionResult> GetPacientesByGEMP(Guid idGEMP)
        {
            if (!HasPermission("GetPacientesByGEMP"))
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

            var response = await _pacienteService.GetPacientesByGEMPAsync(idGEMP);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("ByMedico/{idMedico}")]
        public async Task<IActionResult> GetPacientesByMedico(Guid idMedico)
        {
            if (!HasPermission("GetPacientesByMedico"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idMedico == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _pacienteService.GetPacientesByMedicoAsync(idMedico);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.PacientesController
                       .EndpointRolesPacientesController[endpointName]
                   .Contains(rol);
        }
    }
}
