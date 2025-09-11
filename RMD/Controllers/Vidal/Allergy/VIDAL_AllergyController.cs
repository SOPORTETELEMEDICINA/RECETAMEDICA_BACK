using RMD.Interface.Vidal;

namespace RMD.Controllers.Vidal.Allergy
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class VIDAL_AllergyController : ControllerBase
    {
        private readonly IAllergyService _allergyService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        public VIDAL_AllergyController(
        IAllergyService allergyService,
        ICatalogoNotificacionService catalogoNotificacionService)
        {
            _allergyService = allergyService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        [HttpPost("AllergyByName")]
        public async Task<IActionResult> GetAllergiesByName([FromBody] string name)
        {
            // 1) Permiso
            if (!HasPermission("GetAllergiesByName"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación de input
            if (string.IsNullOrWhiteSpace(name))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al service
            var response = await _allergyService.GetAllergiesByNameAsync(name);

            // 4) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.VIDAL_AllergyController.EndpointRolesVIDAL_AllergyController[endpointName].Contains(rol);
        }
    }
}
