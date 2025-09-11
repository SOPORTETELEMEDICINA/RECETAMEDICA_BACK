using RMD.Interface.Consulta;
using RMD.Interface.Pacientes;
using RMD.Shared.Models.Consulta;

namespace RMD.Controllers.Consulta
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class ConsultaController : ControllerBase
    {
        private readonly IConsultaService _consultaService;
        private readonly IPacienteService _pacienteService;
        private readonly IEventosSaludService _eventosSaludService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public ConsultaController(
            IConsultaService consultaService,
            IPacienteService pacienteService,
            IEventosSaludService eventosSaludService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _consultaService = consultaService;
            _pacienteService = pacienteService;
            _eventosSaludService = eventosSaludService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }      
               
        [HttpPost("GetRelaciones")]
        public async Task<IActionResult> GetIdsFromLink([FromQuery] int id, [FromQuery] string idType, [FromQuery] string relacionType)
        {
            // 1) Permiso
            if (!HasPermission("GetIdsFromLink"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación de datos
            if (id <= 0 || string.IsNullOrWhiteSpace(idType) || string.IsNullOrWhiteSpace(relacionType))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            try
            {
                var response = await _consultaService.GetIdsFromLink(id, idType, relacionType);

                return (response.Toast == "success" || response.Toast == "info")
                    ? Ok(response)
                    : BadRequest(response);
            }
            catch (Exception ex)
            {
                // loguealo, debugguealo, o hazle burla
                return StatusCode(500, $"Excepción en Controller: {ex.Message}");
            }
        }

        [HttpPost("Analisis")]
        public async Task<IActionResult> AnalyzePrescription([FromBody] PrescriptionModel request)
        {
            try
            {
                // 1) Permiso
                if (!HasPermission("AnalyzePrescription"))
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }

                // 2) Validación de datos
                if (!ModelState.IsValid)
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }

                // 3) Llamada al servicio
                var response = await _consultaService.ProcessPrescriptionRequest(request);

                // 4) Evaluar Toast
                return (response.Toast == "success" || response.Toast == "info")
                    ? Ok(response)
                    : BadRequest(response);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");

                return BadRequest(ResponseFromService<string>.Exeption(ex, notif));
            }
            
        }
        [HttpPost("AnalisisXml")]
        public async Task<IActionResult> AnalyzePrescriptionXML([FromBody] PrescriptionModel request)
        {
            // 1) Permiso
            if (!HasPermission("AnalyzePrescriptionXML"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación de datos
            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var response = await _consultaService.ProcessPrescriptionXMLRequest(request);

            // 4) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
       
        [HttpPost("PacienteByName")]
        public async Task<IActionResult> SearchPacienteByName(string pacientName)
        {
            // 1) Permiso
            if (!HasPermission("SearchPacienteByName"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación de token
            var gempClaim = User.FindFirstValue("GEMP");
            if (!Guid.TryParse(gempClaim, out var idGemp))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var response = await _pacienteService.GetPacienteByNameAsync(pacientName, idGemp);

            // 4) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("MedicamentoByName")]
        public async Task<IActionResult> GetMedicamentoByName(string name)
        {
            // 1) Permiso
            if (!HasPermission("GetMedicamentoByName"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Llamada al servicio
            var response = await _consultaService.GetMedicamentoByNameAsync(name);

            // 3) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }       

        [HttpGet("SucursalesPorUsuario")]
        public async Task<IActionResult> ObtenerSucursalesPorUsuario()
        {
            if (!HasPermission("ObtenerSucursalesPorUsuario"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _consultaService.ObtenerSucursalesPorUsuarioAsync(idUsuario);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("GetTieneEventoSalud")]
        public async Task<IActionResult> GetTieneEventoSalud([FromBody] Guid idPaciente)
        {
            if (!HasPermission("GetTieneEventoSalud"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idPaciente == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _consultaService.GetTieneEventoSaludAsync(idPaciente);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("GetTieneEventoMedicamentoso")]
        public async Task<IActionResult> GetTieneEventoMedicamentoso([FromBody] Guid idPaciente)
        {
            if (!HasPermission("GetTieneEventoMedicamentoso"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idPaciente == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _consultaService.GetTieneEventoMedicamentosoAsync(idPaciente);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("eventosSalud")]
        public async Task<IActionResult> GetEventosSaludByPaciente([FromBody] Guid idPaciente)
        {
            if (!HasPermission("GetEventosSaludByPaciente"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idPaciente == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _eventosSaludService.GetAllEventosPacienteAsync(idPaciente);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.ConsultaController.EndpointRolesConsultaController[endpointName].Contains(rol);
        }
    }
}
