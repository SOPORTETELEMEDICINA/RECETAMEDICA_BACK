using RMD.Interface.Medicos;
using RMD.Shared.Models.Medicos;

namespace RMD.Controllers.Medicos
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class MedicosController : ControllerBase
    {
        private readonly IMedicoService _medicoService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public MedicosController(
            IMedicoService medicoService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _medicoService = medicoService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpGet("ByIdMedico/{idMedico}")]
        public async Task<IActionResult> GetMedicoByIdMedico(Guid idMedico)
        {
            if (!HasPermission("GetMedicoByIdMedico"))
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

            var response = await _medicoService.GetMedicoByIdMedicoAsync(idMedico);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("BySucursal/{idSucursal}")]
        public async Task<IActionResult> GetMedicosBySucursal(Guid idSucursal)
        {
            if (!HasPermission("GetMedicosBySucursal"))
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

            var response = await _medicoService.GetMedicosBySucursalAsync(idSucursal);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("ByGEMP/{idGEMP}")]
        public async Task<IActionResult> GetMedicosByGEMP(Guid idGEMP)
        {
            if (!HasPermission("GetMedicosByGEMP"))
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

            var response = await _medicoService.GetMedicosByGEMPAsync(idGEMP);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("ByIdUsuario/{idUsuario}")]
        public async Task<IActionResult> GetMedicoByIdUsuario(Guid idUsuario)
        {
            if (!HasPermission("GetMedicoByIdUsuario"))
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

            var response = await _medicoService.GetMedicoByIdUsuarioAsync(idUsuario);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMedico([FromBody] MedicoCreate model)
        {
            if (!HasPermission("CreateMedico"))
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

            var idRolClaim = User.FindFirstValue("IdRol");
            if (!Guid.TryParse(idRolClaim, out var idRol))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _medicoService.CreateMedicoAsync(model, idRol);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMedico([FromBody] Medico model)
        {
            if (!HasPermission("UpdateMedico"))
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

            var response = await _medicoService.UpdateMedicoAsync(model, idUsuario);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("{idMedico}")]
        public async Task<IActionResult> DeleteMedico(Guid idMedico)
        {
            if (!HasPermission("DeleteMedico"))
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

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _medicoService.DeleteMedicoAsync(idMedico, idUsuario);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.MedicosController
                       .EndpointRolesMedicosController[endpointName]
                   .Contains(rol);
        }
    }
}
