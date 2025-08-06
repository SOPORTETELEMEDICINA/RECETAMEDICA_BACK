using RMD.Interface.PuntoVenta;
using RMD.Models.PuntoVenta;

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
       // private readonly IConfiguration _configuration;
        public PuntoVentaController(//IConfiguration configuration,
            IPuntoVentaService puntoVentaService, 
            ICatalogoNotificacionService catalogoNotificacionService)
        {
           // _configuration = configuration;
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

            // 3) Llamada directa al servicio, él se encarga de desencriptar y validar
            var response = await _puntoVentaService.ObtenerRecetaAsync(qrEncriptado);

            // 4) Evaluar toast
            return Ok(response);
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
            if (!request.DetallesReceta.Any())
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 3) Llamada al servicio
            var response = await _puntoVentaService
                .SurtirMedicamentosAsync( request );
            // 4) Evaluar toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        //[HttpPost("GetRepositoryPacienteByQR")]
        //public async Task<IActionResult> GetRepositoryPacienteByQR([FromBody] string qrEncriptado)
        //{
        //    var country = _configuration["Country"];
        //    if (!string.Equals(country, "ES", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return NotFound();
        //    }

        //    var response = await _puntoVentaService.GetRepositoryPacienteByQR(qrEncriptado);

        //    return (response.Toast == "success" || response.Toast == "info")
        //        ? Ok(response)
        //        : BadRequest(response);
        //}
        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.PuntoVentaController
                       .EndpointRolesPuntoVentaController[endpointName]
                   .Contains(rol);
        }

    }
}
