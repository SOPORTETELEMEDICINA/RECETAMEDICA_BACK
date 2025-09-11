using RMD.Interface.Pacientes;

namespace RMD.Controllers.Pacientes
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class PacienteQRController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IConfiguration _configuration;
        public PacienteQRController(
            IPacienteService pacienteService,
            ICatalogoNotificacionService catalogoNotificacionService,
            IConfiguration configuration)
        {
            _pacienteService = pacienteService;
            _catalogoNotificacionService = catalogoNotificacionService;
            _configuration = configuration;
        }

        [HttpPost("GenerarQR")]
        public async Task<IActionResult> GenerarQRParaPaciente()
        {

            var country = _configuration["Country"];
            if (!string.Equals(country, "ES", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }
            if (!HasPermission("GenerarQRParaPaciente"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var pacienteResult = await _pacienteService.GetPacienteByIdUsuarioAsync(idUsuario);
            if (pacienteResult.Data.IdPaciente == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PACIENTES", "NO_SE_ENCONTRO_PACIENTE");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _pacienteService.GenerarQRParaPacienteAsync(pacienteResult.Data.IdPaciente);
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
