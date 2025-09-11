using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatEntidadesFederativasController : ControllerBase
    {
        private readonly ICatEntidadesFederativasService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatEntidadesFederativasController(ICatEntidadesFederativasService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        #region CRUD para Entidades Federativas

        [HttpGet("entidades")]
        public async Task<IActionResult> GetAllEntidades()
        {
            if (!HasPermission("GetAllEntidades"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllEntidadesAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEntidadById(int id)
        {
            if (!HasPermission("GetEntidadById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            if (id <= 0)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _service.GetEntidadByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntidad(int id, [FromBody] CatEntidadesFederativas entidad)
        {
            if (!HasPermission("UpdateEntidad"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                notif.Mensaje = errores;
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _service.UpdateEntidadAsync(id, entidad);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        #endregion
        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.CatalogoController.EndpointRolesCatalogoController[endpointName].Contains(rol);
        }

    }
}
