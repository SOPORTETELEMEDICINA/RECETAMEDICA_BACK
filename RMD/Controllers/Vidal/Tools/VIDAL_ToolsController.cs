using RMD.Interface.Vidal;
using RMD.Shared.Models.Vidal.Tools;

namespace RMD.Controllers.Vidal.Tools
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class VIDAL_ToolsController : ControllerBase
    {
        private readonly IToolsService _toolsService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly string country;

        public VIDAL_ToolsController(
            IToolsService toolsService,
            ICatalogoNotificacionService catalogoNotificacionService, 
            IConfiguration _configuration)
        {
            _toolsService = toolsService;
            _catalogoNotificacionService = catalogoNotificacionService;
            country = _configuration["Country"] ?? "MX";
        }

        [HttpPost("documentos")]
        public async Task<IActionResult> GetDocumentosFicha([FromBody] FichaHtmlRequest request)
        {
            //if (!HasPermission("GetDocumentosFicha"))
            //{
            //    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
            //    return BadRequest(ResponseFromService<string>.Failure(notif));
            //}

            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "MODELO_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _toolsService.GetDocumentListAsync(request);
            return (response.Toast.ToUpper() == "SUCCESS" || response.Toast.ToUpper() == "INFO")
                ? Ok(response) : BadRequest(response);
        }

        [HttpPost("fichaHtml")]
        public async Task<IActionResult> GetFichaHtml([FromBody] string path)
        {
            //if (!HasPermission("GetFichaHtml"))
            //{
            //    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
            //    return BadRequest(ResponseFromService<string>.Failure(notif));
            //}

            if (string.IsNullOrWhiteSpace(path))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "MODELO_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _toolsService.GetFichaHtmlAsync(path);
            return (response.Toast.ToUpper() == "SUCCESS" || response.Toast.ToUpper() == "INFO")
                ? Ok(response)
                : BadRequest(response);
        }
        [HttpPost("getTiposDeAlertas")]
        public async Task<IActionResult> GetTiposDeAlertas()
        {
            if (!string.Equals(country, "ES", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }
            //if (!HasPermission("GetTiposDeAlertas"))
            //{
            //    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
            //    return BadRequest(ResponseFromService<string>.Failure(notif));
            //}

            var response = await _toolsService.GetTiposDeAlertasAsync();
            return (response.Toast.ToUpper() == "SUCCESS" || response.Toast.ToUpper() == "INFO")
                ? Ok(response) : BadRequest(response);
        }
    
        [HttpPost("getAlertaByTipo")]
        public async Task<IActionResult> GetAlertaByTipo([FromBody] int type)
        {
            if (!string.Equals(country, "ES", StringComparison.OrdinalIgnoreCase))
                return NotFound();

            var response = await _toolsService.GetTipoAlertaJsonAsync(type);

            if (response.Toast.ToUpper() == "SUCCESS" || response.Toast.ToUpper() == "INFO")
                return Ok(response); // ← JSON nativo

            return BadRequest(response);
        }
        [HttpPost("getContenidoDesdeRuta")]
        public async Task<IActionResult> GetContenidoDesdeRuta([FromBody] string ruta)
        {
            if (!string.Equals(country, "ES", StringComparison.OrdinalIgnoreCase))
                return NotFound();

            if (string.IsNullOrWhiteSpace(ruta))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "MODELO_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _toolsService.GetContenidoDesdeRutaAsync(ruta);

            if (response.Toast.ToUpper() == "SUCCESS" || response.Toast.ToUpper() == "INFO")
            {
                // Intentamos parsear el string como JSON, si falla asumimos que es HTML
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("getNoticiaRelatedById")]
        public async Task<IActionResult> GetNoticiaRelatedById([FromBody]int idNoticia)
        {
            if (!string.Equals(country, "ES", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }
            if (!HasPermission("GetNoticiaRelatedById"))
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }


            var response = await _toolsService.GetNoticiaRelatedHtmlById(idNoticia);
            return (response.Toast.ToUpper() == "SUCCESS" || response.Toast.ToUpper() == "INFO")
                ? Ok(response) : BadRequest(response);
        }

        //[HttpPost("cargar-alertas-vidal")]
        //public async Task<IActionResult> CargarAlertasDesdeVidal()
        //{
        //    try
        //    {
        //        await _toolsService.GuardarTodasLasAlertasDesdeVidalAsync();
        //        return Ok("Alertas cargadas correctamente desde Vidal.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error al cargar alertas: {ex.Message}");
        //    }
        //}
        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.VIDAL_ToolsController.EndpointRolesVIDAL_ToolsController[endpointName].Contains(rol);
        }
        //[HttpPost("getAlertaByTipo")]
        //public async Task<IActionResult> GetAlertaByTipo([FromBody] int type)
        //{
        //    if (!string.Equals(country, "ES", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return NotFound();
        //    }
        //    //if (!HasPermission("GetAlertaByTipo"))
        //    //{
        //    //    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //    //    return BadRequest(ResponseFromService<string>.Failure(notif));
        //    //}

        //    var response = await _toolsService.GetTipoAlertaHtmlAsync(type);
        //    return (response.Toast.ToUpper() == "SUCCESS" || response.Toast.ToUpper() == "INFO")
        //        ? Ok(response) : BadRequest(response);
        //}
        //[HttpPost("getNoticiaYAlertasById")]
        //public async Task<IActionResult> GetNoticiaById([FromBody] string type, int idNoticia)
        //{
        //    if (!string.Equals(country, "ES", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return NotFound();
        //    }
        //    if (!HasPermission("GetNoticiaById"))
        //    {
        //        var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<string>.Failure(notif));
        //    }
        //    var response = await _toolsService.GetNoticiaHtmlById(type, idNoticia);
        //    return (response.Toast.ToUpper() == "SUCCESS" || response.Toast.ToUpper() == "INFO")
        //        ? Ok(response) : BadRequest(response);
        //}
    }

}
