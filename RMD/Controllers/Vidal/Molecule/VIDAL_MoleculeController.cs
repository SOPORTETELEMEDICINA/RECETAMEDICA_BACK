using RMD.Interface.Vidal;

namespace RMD.Controllers.Vidal.Molecule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class VIDAL_MoleculeController : ControllerBase
    {
        private readonly IMoleculeService _moleculeService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public VIDAL_MoleculeController(
        IMoleculeService moleculeService,
        ICatalogoNotificacionService catalogoNotificacionService)
        {
            _moleculeService = moleculeService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("MoleculeByName")]
        public async Task<IActionResult> GetMoleculeByName([FromBody] string name)
        {
            // 1) Permiso
            if (!HasPermission("GetMoleculeByName"))
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
            var response = await _moleculeService.GetMoleculeByNameAsync(name);

            // 4) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.VIDAL_MoleculeController.EndpointRolesVIDAL_MoleculeController[endpointName].Contains(rol);
        }
    }
}
