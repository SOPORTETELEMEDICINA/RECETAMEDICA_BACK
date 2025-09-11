using RMD.Interface.Receta;

namespace RMD.Controllers.Receta
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class RecetaCatalogosController : ControllerBase
    {
        private readonly IRecetaCatalogosService _recetaCatalogosService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public RecetaCatalogosController(
            IRecetaCatalogosService recetaCatalogosService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _recetaCatalogosService = recetaCatalogosService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpGet("frecuency-types")]
        public async Task<IActionResult> ObtenerFrecuencyTypes()
        {
            // Validación de permisos
            if (!HasPermission("ObtenerFrecuencyTypes"))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // Llamar servicio
            var response = await _recetaCatalogosService.ObtenerFrecuencyTypesAsync();

            // Validar resultado
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("frecuency-types/{id}")]
        public async Task<IActionResult> ObtenerFrecuencyTypePorId(int id)
        {
            // Validación de permisos
            if (!HasPermission("ObtenerFrecuencyTypePorId"))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // Llamar servicio
            var response = await _recetaCatalogosService.ObtenerFrecuencyTypePorIdAsync(id);

            // Validar resultado
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.RecetaCatalogosController.EndpointRolesRecetaCatalogosController[endpointName].Contains(rol);
        }
    }
}
