using RMD.Interface.Pacientes;
using RMD.Shared.Models.Pacientes.Request;

namespace RMD.Controllers.Pacientes
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public PacientesController(
            IPacienteService pacienteService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _pacienteService = pacienteService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpGet("ByIdUsuario/{idUsuario}")]
        public async Task<IActionResult> GetPacienteByIdUsuario(Guid idUsuario)
        {
            if (!HasPermission("GetPacienteByIdUsuario"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idUsuario == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _pacienteService.GetPacienteByIdUsuarioAsync(idUsuario);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("ByIdPaciente/{idPaciente}")]
        public async Task<IActionResult> GetPacienteByIdPaciente(Guid idPaciente)
        {
            if (!HasPermission("GetPacienteByIdPaciente"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idPaciente == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _pacienteService.GetPacienteByIdPacienteAsync(idPaciente);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateConListasRequest model)
        {
            if (!HasPermission("CreatePaciente"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                notif.Mensaje = errores;
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuarioSolicitante))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _pacienteService.CreatePacienteAsync(model, idUsuarioSolicitante);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePaciente([FromBody] PacienteConListasRequest model)
        {
            if (!HasPermission("UpdatePaciente"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                notif.Mensaje = errores;
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuarioSolicitante))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _pacienteService.UpdatePacienteAsync(model, idUsuarioSolicitante);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("{idPaciente}")]
        public async Task<IActionResult> EliminarPaciente(Guid idPaciente)
        {
            if (!HasPermission("EliminarPaciente"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idPaciente == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuarioSolicitante))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _pacienteService.EliminarPacienteAsync(idPaciente, idUsuarioSolicitante);
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
