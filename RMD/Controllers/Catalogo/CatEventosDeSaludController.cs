using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatEventosDeSaludController : ControllerBase
    {
        private readonly ICatEventosDeSaludService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatEventosDeSaludController(ICatEventosDeSaludService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }


        #region CRUD para CatEventosDeSalud

        [HttpGet("CateventosSalud")]
        public async Task<IActionResult> GetAllEventosSalud()
        {
            if (!HasPermission("GetAllEventosSalud"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllEventosSaludAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("CateventosSalud/{id}")]
        public async Task<IActionResult> GetEventoSaludById(int id)
        {
            if (!HasPermission("GetEventoSaludById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetEventoSaludByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpPut("CateventosSalud/{id}")]
        public async Task<IActionResult> UpdateEventoSalud(int id, [FromBody] CatEventosDeSalud evento)
        {
            if (!HasPermission("UpdateEventoSalud"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.UpdateEventoSaludAsync(id, evento);
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
