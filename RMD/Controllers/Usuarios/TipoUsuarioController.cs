using RMD.Interface.Usuarios;
using RMD.Shared.Models.Usuarios;

namespace RMD.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class TipoUsuarioController : ControllerBase
    {
        private readonly ITipoUsuarioService _tipoUsuarioService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public TipoUsuarioController(ITipoUsuarioService tipoUsuarioService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _tipoUsuarioService = tipoUsuarioService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpGet("GetAllTipoUsuario")]
        public async Task<IActionResult> GetAllTipoUsuario()
        {
            if (!HasPermission("GetAllTipoUsuario"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var response = await _tipoUsuarioService.GetAllTipoUsuarioAsync();
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        [HttpGet("GetTipoUsuarioById/{id}")]
        public async Task<IActionResult> GetTipoUsuarioById(Guid id)
        {
            if (!HasPermission("GetTipoUsuarioById"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (id == Guid.Empty)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var response = await _tipoUsuarioService.GetTipoUsuarioByIdAsync(id);
            // Si el código en la respuesta indica que el tipo de usuario no fue encontrado, se retorna NotFound
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        [HttpPost("CreateTipoUsuario")]
        public async Task<IActionResult> CreateTipoUsuario([FromBody] TipoUsuario tipoUsuario)
        {
            if (!HasPermission("CreateTipoUsuario"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (!ModelState.IsValid)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var response = await _tipoUsuarioService.CreateTipoUsuarioAsync(tipoUsuario);
            // Se espera el código de éxito correspondiente (por ejemplo, 50450)
            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        [HttpPut("UpdateTipoUsuario")]
        public async Task<IActionResult> UpdateTipoUsuario([FromBody] TipoUsuario tipoUsuario)
        {
            if (!HasPermission("UpdateTipoUsuario"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (!ModelState.IsValid)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var response = await _tipoUsuarioService.UpdateTipoUsuarioAsync(tipoUsuario);
            // Se espera el código de éxito correspondiente para actualización (por ejemplo, 50451)

            return (response.Toast == "success" || response.Toast == "info") ? Ok(response) : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.TipoUsuarioController.EndpointRolesTipoUsuarioController[endpointName].Contains(rol);
        }
    }
}
