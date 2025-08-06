using RMD.Interface.Vidal;
using RMD.Shared.Models.Vidal.CIM10;

namespace RMD.Controllers.Vidal.CIM10
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class VIDAL_CIM10Controller : ControllerBase
    {
        private readonly ICIM10Service _cim10Service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public VIDAL_CIM10Controller(
        ICIM10Service cim10Service,
        ICatalogoNotificacionService catalogoNotificacionService)
        {
            _cim10Service = cim10Service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("CIM10ByName")]
        public async Task<IActionResult> GetCIM10ByName([FromBody] PatologiaRequest request)
        {
            // 1) Permiso
            if (!HasPermission("GetCIM10ByName"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación del modelo
            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }


            // 3) Llamada al servicio
            var response = await _cim10Service.GetCIM10sByNameAsync(request);

            // 4) Evaluar toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        [HttpPost("AskPSumistroByIdProduct")]
        public async Task<IActionResult> AskPSumistroByIdProduct([FromBody] int idProduct)
        {
            // 1) Permiso
            if (!HasPermission("AskPSumistroByIdProduct"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación básica
            if (idProduct <= 0)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var result = await _cim10Service.AskPSumistroByIdProduct(idProduct);

            // 4) Notificación
            var notifSuccess = await _catalogoNotificacionService
                .GetNotificationByTipoAndFuncionAsync("VIDALCIM10", "ASK_PSUMINISTRO");

            return Ok(ResponseFromService<bool>.Success(result, notifSuccess));
        }

        [HttpPost("GetPSumistroByIdProduct")]
        public async Task<IActionResult> GetPSumistroByIdProduct([FromBody] int idProduct)
        {
            // 1) Permiso
            if (!HasPermission("GetPSumistroByIdProduct"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación básica
            if (idProduct <= 0)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var result = await _cim10Service.GetPSumistroByIdProduct(idProduct);

            // 4) Notificación
            var notifSuccess = await _catalogoNotificacionService
                .GetNotificationByTipoAndFuncionAsync("VIDALCIM10", "GET_PSUMINISTRO");

            return Ok(ResponseFromService<string>.Success(result ?? string.Empty, notifSuccess));
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.VidalCim10Controller.EndpointRolesVidalCim10Controller[endpointName].Contains(rol);
        }
    }
}
