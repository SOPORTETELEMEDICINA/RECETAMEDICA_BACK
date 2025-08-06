using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatalogoController(ICatalogoService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpGet("AsentamientoByNames")]
        public async Task<IActionResult> SearchAsentamiento([FromQuery] AsentamientoSearchModel searchModel)
        {
            // 1) Permiso
            if (!HasPermission("SearchAsentamiento"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Llamada al servicio
            var response = await _service.BuscarAsentamientosAsync(searchModel);

            // 3) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("insertar-domicilio")]
        public async Task<IActionResult> InsertarDomicilioCompleto([FromBody] DomicilioRequest model)
        {
            if (!HasPermission("InsertarDomicilioCompleto"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }

            var result = await _service.InsertarDomicilioCompletoAsync(model);
            return (result.Toast == "success" || result.Toast == "info")
                ? Ok(result)
                : BadRequest(result);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.CatalogoController.EndpointRolesCatalogoController[endpointName].Contains(rol);
        }
    }
}
