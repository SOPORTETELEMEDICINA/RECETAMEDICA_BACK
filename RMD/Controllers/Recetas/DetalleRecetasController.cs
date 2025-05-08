using RMD.Interface.Notificaciones;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;
using RMD.Models.Responses;

namespace RMD.Controllers.Recetas
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class DetalleRecetasController : ControllerBase
    {
        private readonly IDetalleRecetaService _detalleRecetaService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public DetalleRecetasController(IDetalleRecetaService detalleRecetaService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _detalleRecetaService = detalleRecetaService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("GetDetallesByIdReceta")]
        public async Task<IActionResult> GetDetallesByIdReceta([FromBody] Guid idReceta)
        {
            // 1) Permisos
            //if (!HasPermission("GetDetallesByIdReceta"))
            //{
            //    var notif = await _catalogoNotificacionService
            //        .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
            //    return BadRequest(ResponseFromService<string>.Failure(notif));
            //}

            // 2) Validar token
            var idUsuarioString = User.FindFirst("IdUsuario")?.Value;
            if (!Guid.TryParse(idUsuarioString, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var response = await _detalleRecetaService.GetDetallesByIdReceta(idUsuario, idReceta);

            // 4) Validación de toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("reaccion-crear-actualizar")]
        public async Task<IActionResult> CreateUpdateReaccion([FromBody] DetalleRecetaRequest request)
        {
            // 1) Permisos
            //if (!HasPermission("CreateUpdateReaccion"))
            //{
            //    var notif = await _catalogoNotificacionService
            //        .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
            //    return BadRequest(ResponseFromService<string>.Failure(notif));
            //}

            // 2) Validar token
            var idUsuarioString = User.FindFirst("IdUsuario")?.Value;
            if (!Guid.TryParse(idUsuarioString, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var response = await _detalleRecetaService.CreateUpdateReaccionAsync(request, idUsuario);

            // 4) Validación de toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("reaccion-delete")]
        public async Task<IActionResult> DeleteReaccion([FromBody] DetalleRecetaRequest request)
        {
            // 1) Permisos
            //if (!HasPermission("DeleteReaccion"))
            //{
            //    var notif = await _catalogoNotificacionService
            //        .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
            //    return BadRequest(ResponseFromService<string>.Failure(notif));
            //}

            // 2) Validar token
            var idUsuarioString = User.FindFirst("IdUsuario")?.Value;
            if (!Guid.TryParse(idUsuarioString, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var response = await _detalleRecetaService.DeleteReaccionAsync(request, idUsuario);

            // 4) Validación de toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("GetSoloMedicamentosActivos")]
        public async Task<IActionResult> GetSoloMedicamentosActivos()
        {
            // 1) Permisos
            //if (!HasPermission("GetSoloMedicamentosActivos"))
            //{
            //    var notif = await _catalogoNotificacionService
            //        .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
            //    return BadRequest(ResponseFromService<string>.Failure(notif));
            //}

            // 2) Validar token
            var idUsuarioString = User.FindFirst("IdUsuario")?.Value;
            if (!Guid.TryParse(idUsuarioString, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var response = await _detalleRecetaService.GetSoloMedicamentosActivos(idUsuario);

            // 4) Validación de toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

    }
}
