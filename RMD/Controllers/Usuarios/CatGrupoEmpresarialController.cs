using RMD.Interface.Usuarios;
using RMD.Shared.Models.Usuarios;

namespace RMD.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatGrupoEmpresarialController : ControllerBase
    {
        private readonly ICatGrupoEmpresarialService _grupoEmpresarialService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatGrupoEmpresarialController(
            ICatGrupoEmpresarialService grupoEmpresarialService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _grupoEmpresarialService = grupoEmpresarialService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        /// <summary>
        /// Obtiene todos los grupos empresariales.
        /// </summary>
        [HttpGet("GetAllGrupoEmpresarial")]
        public async Task<IActionResult> GetAllGrupoEmpresarial()
        {
            if (!HasPermission("GetAllGrupoEmpresarial"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var result = await _grupoEmpresarialService.GetAllGrupoEmpresarialAsync();
            return (result.Toast == "success" || result.Toast == "info") ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Obtiene un grupo empresarial por su ID.
        /// </summary>
        [HttpGet("GetGrupoEmpresarialById/{id}")]
        public async Task<IActionResult> GetGrupoEmpresarialById(Guid id)
        {
            if (!HasPermission("GetGrupoEmpresarialById"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (id == Guid.Empty)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var result = await _grupoEmpresarialService.GetGrupoEmpresarialByIdAsync(id);
            return (result.Toast == "success" || result.Toast == "info") ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Crea un nuevo grupo empresarial.
        /// </summary>
        [HttpPost("CreateGrupoEmpresarial")]
        public async Task<IActionResult> CreateGrupoEmpresarial([FromBody] CatGrupoEmpresarial grupoEmpresarial)
        {
            if (!HasPermission("CreateGrupoEmpresarial"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (!ModelState.IsValid)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var result = await _grupoEmpresarialService.CreateGrupoEmpresarialAsync(grupoEmpresarial);
            return (result.Toast == "success" || result.Toast == "info") ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Actualiza un grupo empresarial existente.
        /// </summary>
        [HttpPut("UpdateGrupoEmpresarial")]
        public async Task<IActionResult> UpdateGrupoEmpresarial([FromBody] CatGrupoEmpresarial grupoEmpresarial)
        {
            if (!HasPermission("UpdateGrupoEmpresarial"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (!ModelState.IsValid)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var result = await _grupoEmpresarialService.UpdateGrupoEmpresarialAsync(grupoEmpresarial);
            return (result.Toast == "success" || result.Toast == "info") ? Ok(result) : BadRequest(result);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.CatGrupoEmpresarialController.EndpointRolesCatGrupoEmpresarialController[endpointName].Contains(rol);
        }
    }
}
