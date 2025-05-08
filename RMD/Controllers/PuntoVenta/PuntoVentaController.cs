using RMD.Interface.Notificaciones;
using RMD.Interface.PuntoVenta;
using RMD.Models.PuntoVenta;
using RMD.Models.Responses;

namespace RMD.Controllers.PuntoVenta
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class PuntoVentaController : ControllerBase
    {
        private readonly IPuntoVentaService _puntoVentaService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public PuntoVentaController(IPuntoVentaService puntoVentaService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _puntoVentaService = puntoVentaService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("buscar-receta")]
        public async Task<IActionResult> BuscarReceta([FromBody] string qrEncriptado)
        {
            // 1) Permiso
            if (!HasPermission("BuscarReceta"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 2) Validar input
            if (string.IsNullOrEmpty(qrEncriptado))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 3) Desencriptar y parsear
            var partes = EncryptionHelper.Decrypt(qrEncriptado).Split('|');
            if (partes.Length != 3
                || !Guid.TryParse(partes[0], out var idReceta)
                || !Guid.TryParse(partes[1], out var idMedico)
                || !DateTime.TryParse(partes[2].Trim(),
                     CultureInfo.InvariantCulture,
                     DateTimeStyles.None,
                     out var fechaUltimaModificacion))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 4) Llamada al servicio
            var response = await _puntoVentaService
                .ObtenerRecetaAsync(idReceta, idMedico, fechaUltimaModificacion);
            // 5) Evaluar toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpPost("surtir-medicamentos")]
        public async Task<IActionResult> SurtirMedicamentos([FromBody] SurtirRecetaRequest request)
        {
            // 1) Permiso
            if (!HasPermission("SurtirMedicamentos"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 2) Validar input
            if (request?.DetallesReceta == null || !request.DetallesReceta.Any())
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 3) Llamada al servicio
            var response = await _puntoVentaService
                .SurtirMedicamentosAsync(request.IdReceta, request.DetallesReceta);
            // 4) Evaluar toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("consultar-receta/{folio}")]
        public async Task<IActionResult> ConsultarRecetaPorId(string folio)
        {
            // 1) Permiso
            if (!HasPermission("ConsultarRecetaPorId"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 2) Validar input
            if (string.IsNullOrEmpty(folio))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 3) Llamada al servicio
            var response = await _puntoVentaService
                .ConsultarRecetaPorIdAsync(folio);
            // 4) Evaluar toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.PuntoVentaController
                       .EndpointRolesPuntoVentaController[endpointName]
                   .Contains(rol);
        }

    }
}
