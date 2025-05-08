using RMD.Interface.Catalogo;
using RMD.Interface.Consulta;
using RMD.Interface.Notificaciones;
using RMD.Interface.Pacientes;
using RMD.Models.Consulta;
using RMD.Models.Pacientes;
using RMD.Models.Responses;

namespace RMD.Controllers.Consulta
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class ConsultaController : ControllerBase
    {
        private readonly IConsultaService _consultaService;
        private readonly ICatalogoService _catalogoService;
        private readonly IPacienteService _pacienteService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public ConsultaController(
            IConsultaService consultaService,
            ICatalogoService catalogoService,
            IPacienteService pacienteService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _consultaService = consultaService;
            _catalogoService = catalogoService;
            _pacienteService = pacienteService;
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
            var response = await _consultaService.GetAllergiesByNameAsync(name);

            // 4) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
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
            var response = await _consultaService.GetMoleculeByNameAsync(name);

            // 4) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("CIM10ByName")]
        public async Task<IActionResult> GetCIM10ByName([FromBody] string name)
        {
            // 1) Permiso
            if (!HasPermission("GetCIM10ByName"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación de datos
            if (string.IsNullOrWhiteSpace(name))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Llamada al servicio
            var response = await _consultaService.GetCIM10sByNameAsync(name);

            // 4) Evaluar toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
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
            var response = await _consultaService.GetIdsFromLink(id, idType, relacionType);

            // 4) Evaluar respuesta
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("Analisis")]
        public async Task<IActionResult> AnalyzePrescription([FromBody] PrescriptionModel request)
        {
            // 1) Permiso
            if (!HasPermission("AnalyzePrescription"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validación de datos
            if (request == null)
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
            if (request == null)
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
            var response = await _catalogoService.BuscarAsentamientosAsync(searchModel);

            // 3) Evaluar Toast
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

        [HttpPost("RegistrarReceta")]
        public async Task<IActionResult> RegistrarReceta([FromBody] RecetaRequestModel request)
        {
            // 1) Permiso
            if (!HasPermission("RegistrarReceta"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validar input
            if (!ModelState.IsValid || request == null)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                notif.Mensaje = errores;
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Obtener token
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            // 4) Llamada al servicio
            var response = await _consultaService.RegistrarRecetaAsync(request, token);

            // 5) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("EliminarReceta/{idReceta}")]
        public async Task<IActionResult> EliminarReceta(Guid idReceta)
        {
            // 1) Permiso
            if (!HasPermission("EliminarReceta"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validar input
            if (idReceta == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Extraer claims
            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            var idRolClaim = User.FindFirstValue("IdRol");

            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario) || string.IsNullOrEmpty(idRolClaim))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 4) Llamada al servicio
            var response = await _consultaService.EliminarRecetaAsync(idReceta);

            // 5) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("ConsultarReceta/{idReceta}")]
        public async Task<IActionResult> ConsultarReceta(Guid idReceta)
        {
            // 1) Permiso
            if (!HasPermission("ConsultarReceta"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validar input
            if (idReceta == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 3) Extraer claims
            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            var idRolClaim = User.FindFirstValue("IdRol");

            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario) || string.IsNullOrEmpty(idRolClaim))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 4) Llamada al servicio
            var response = await _consultaService.ConsultarRecetaAsync(idReceta);

            // 5) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("RecetasPorMedico")]
        public async Task<IActionResult> ObtenerRecetasPorMedico([FromQuery] Guid idSucursal)
        {
            if (!HasPermission("ObtenerRecetasPorMedico"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idSucursal == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _consultaService.ObtenerRecetasPorMedicoAsync(idUsuario, idSucursal);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet("RecetasByIdMedico")]
        public async Task<IActionResult> ObtenerRecetasByIdMedico([FromQuery] Guid idSucursal, Guid idMedico)
        {
            if (!HasPermission("ObtenerRecetasByIdMedico"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (idSucursal == Guid.Empty || idMedico == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _consultaService.ObtenerRecetasPorMedicoAsync(idMedico, idSucursal);

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

        [HttpGet("GetReaccionMedicamentoPrevio/{idPaciente}")]
        public async Task<IActionResult> GetReaccionMedicamentoPrevio(Guid idPaciente)
        {
            if (!HasPermission("GetReaccionMedicamentoPrevio"))
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

            var response = await _consultaService.GetReaccionMedicamentoPrevioAsync(idPaciente);

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

            var response = await _pacienteService.GetAllEventosPacienteAsync(idPaciente);

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
