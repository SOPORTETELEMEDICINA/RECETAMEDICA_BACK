using RMD.Interface.Pacientes;
using RMD.Shared.Models.Pacientes.Request;

namespace RMD.Controllers.Pacientes
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class EventosSaludController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;
        IEventosSaludService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public EventosSaludController(
            IPacienteService pacienteService,
            IEventosSaludService service,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _pacienteService = pacienteService;
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        [HttpGet("eventosSalud")]
        public async Task<IActionResult> GetEventosSaludByPaciente()
        {
            if (!HasPermission("GetEventosSaludByPaciente"))
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

            var response = await _service.GetAllEventosPacienteAsync(pacienteResult.Data.IdPaciente);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("eventosSalud")]
        public async Task<IActionResult> CreateEventoSalud([FromBody] EventosSaludRequest model)
        {
            if (!HasPermission("CreateEventoSalud"))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                notif.Mensaje = errores;
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _service.CreateEventoPacienteAsync(model);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        [HttpPut("eventosSalud")]
        public async Task<IActionResult> UpdateEventoSalud([FromBody] EventosSaludRequest model)
        {
            if (!HasPermission("UpdateEventoSalud"))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                notif.Mensaje = errores;
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _service.UpdateEventoPacienteAsync(model);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        [HttpGet("eventosSalud/{idEventoSalud}")]
        public async Task<IActionResult> GetEventoSaludById(Guid idEventoSalud)
        {
            if (!HasPermission("GetEventoSaludById"))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idEventoSalud == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _service.GetEventoPacienteByIdAsync(idEventoSalud);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        [HttpDelete("eventosSalud/{idEventoSalud}")]
        public async Task<IActionResult> DeleteEventoSalud(Guid idEventoSalud)
        {
            if (!HasPermission("DeleteEventoSalud"))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idEventoSalud == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _service.DeleteEventoPacienteAsync(idEventoSalud);
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
