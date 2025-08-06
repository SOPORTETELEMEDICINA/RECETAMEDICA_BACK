using RMD.Interface.Tutores;
using RMD.Shared.Models.Tutores.Request;

namespace RMD.Controllers.Tutores
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class TutoresController : ControllerBase
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public TutoresController(ITutorService tutorService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _tutorService = tutorService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost]
        public async Task<IActionResult> AsignarTutor([FromBody] TutorRequest model)
        {
            if (!HasPermission("AsignarTutor"))
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
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _tutorService.AsignarTutorAsync(model, idUsuario);
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{idTutor}")]
        public async Task<IActionResult> ActualizarTutor(Guid idTutor, [FromBody] TutorRequest model)
        {
            if (!HasPermission("ActualizarTutor"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idTutor == Guid.Empty || !ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");

                if (!ModelState.IsValid)
                {
                    notif.Mensaje = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                }

                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _tutorService.ActualizarTutorAsync(idTutor, model, idUsuario);
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{idTutor}")]
        public async Task<IActionResult> EliminarTutor(Guid idTutor)
        {
            if (!HasPermission("EliminarTutor"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idTutor == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _tutorService.EliminarTutorAsync(idTutor);
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        [HttpGet("PorPaciente/{idPaciente}")]
        public async Task<IActionResult> GetTutorPorPaciente(Guid idPaciente)
        {
            if (!HasPermission("GetTutorPorPaciente"))
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

            var response = await _tutorService.GetTutorPorPacienteAsync(idPaciente);
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.TutoresController
                       .EndpointRolesTutoresController[endpointName]
                   .Contains(rol);
        }
    }
}
