using RMD.Interface.Receta;
using RMD.Shared.Models.Receta.Header.Request;

namespace RMD.Controllers.Receta
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class RecetasController : ControllerBase
    {
        private readonly IRecetaService _recetaService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public RecetasController(IRecetaService recetaService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _recetaService = recetaService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("RegistrarReceta")]
        public async Task<IActionResult> RegistrarReceta([FromBody] HeaderRequest request)
        {
            try
            {
                // 1) Permiso
                if (!HasPermission("RegistrarReceta"))
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }

                // 2) Validar input
                if (!ModelState.IsValid)
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
                var response = await _recetaService.RegistrarRecetaAsync(request, token);

                // 5) Evaluar Toast
                return (response.Toast == "success" || response.Toast == "info")
                    ? Ok(response)
                    : BadRequest(response);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = ex.Message;
                return BadRequest(ResponseFromService<string>.Exeption(ex, notif));
            }
        }

        [HttpPut("ActualizarReceta")]
        public async Task<IActionResult> ActualizarReceta([FromBody] HeaderUpdateRequest request)
        {
            if (!HasPermission("ActualizarReceta"))
            {
                var n = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(n));
            }

            if (!ModelState.IsValid || request.IdReceta == Guid.Empty)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var n = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                n.Mensaje = errores;
                return BadRequest(ResponseFromService<string>.Failure(n));
            }

            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var n = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(n));
            }

            var resp = await _recetaService.ActualizarRecetaAsync(request, idUsuario);
            return (resp.Toast == "success" || resp.Toast == "info") ? Ok(resp) : BadRequest(resp);
        }

        [HttpPost("TimbrarReceta")]
        public async Task<IActionResult> TimbrarReceta([FromBody] QRRequest request)
        {
            // 1) Permiso
            if (!HasPermission("TimbrarReceta"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 2) Validar input
            if (request.IdReceta == Guid.Empty)
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
            // 4) Llamada al servicio
            var response = await _recetaService.TimbrarAsync(request, idUsuario);

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

            if (!Guid.TryParse(idUsuarioClaim, out _) || string.IsNullOrEmpty(idRolClaim))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 4) Llamada al servicio
            var response = await _recetaService.EliminarRecetaAsync(idReceta);

            // 5) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("GetFilteredRecetas")]
        public async Task<IActionResult> GetFilteredRecetas([FromBody] HeaderFilterRequest filterRequest)
        {
            if (!HasPermission("GetFilteredRecetas"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // Extraer rol y claims
            var rol = User.FindFirstValue(ClaimTypes.Role);
            var idGempClaim = User.FindFirstValue("GEMP");
            var idSucursalClaim = User.FindFirstValue("IdSucursal");
            if (string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(idGempClaim))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var idGemp = Guid.Parse(idGempClaim);
            Guid? idSucursal = null;

            if (rol is "EMPLEADO DE FARMACIA" or "ENCARGADO DE FARMACIA")
            {
                if (string.IsNullOrEmpty(idSucursalClaim))
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }
                idSucursal = Guid.Parse(idSucursalClaim);
            }
            else if (rol is "SUPERVISOR SUCURSALES" or "MEDICO" or "PARTICULAR")
            {
                if (!filterRequest.IdSucursal.HasValue)
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("RECETAS", "SUCURSAL_REQUERIDA");
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }
                idSucursal = filterRequest.IdSucursal;
            }
            else if (rol == "SUPER ADMIN")
            {
                if (!filterRequest.IdGEMP.HasValue)
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }
                idGemp = filterRequest.IdGEMP.Value;
                idSucursal = filterRequest.IdSucursal;
            }

            // 5) Llamada al servicio y validación de toast
            var response = await _recetaService.GetFilteredRecetasAsync(
                rol, idGemp, idSucursal,
                filterRequest.Folio,
                filterRequest.StartDate,
                filterRequest.EndDate,
                filterRequest.DateFilter
            );

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("GetFilteredRecetasByIdMedico")]
        public async Task<IActionResult> GetFilteredRecetasByIdMedico([FromBody] HeaderFilterByMedicoRequest filterRequest)
        {
            if (!HasPermission("GetFilteredRecetasByIdMedico"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!Guid.TryParse(User.FindFirstValue("IdUsuario"), out var idUsuario) ||
                !Guid.TryParse(User.FindFirstValue("GEMP"), out var idGemp))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            Guid? idSucursal = null;
            if (Guid.TryParse(User.FindFirstValue("IdSucursal"), out var tmp))
                idSucursal = tmp;

            var response = await _recetaService.GetFilteredRecetasByIdMedicoAsync(
                idUsuario, idGemp, idSucursal,
                filterRequest.Folio,
                filterRequest.StartDate,
                filterRequest.EndDate,
                filterRequest.DateFilter
            );

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("GetRecetasByIdPaciente")]
        public async Task<IActionResult> GetRecetasByIdPaciente([FromBody] HeaderFilterByPacienteRequest filterRequest)
        {
            if (!HasPermission("GetRecetasByIdPaciente"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (!Guid.TryParse(User.FindFirstValue("IdUsuario"), out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            // 1) Obtener paciente
            if (!filterRequest.IdPaciente.HasValue)
            {
                var pacienteResp = await _recetaService.GetIdPacienteByUsuarioAsync(idUsuario);
                if (pacienteResp.Toast == "error" || pacienteResp.Toast == "warning")
                {
                    return NotFound(pacienteResp);
                }
                if (pacienteResp.Data.HasValue)
                {
                    filterRequest.IdPaciente = pacienteResp.Data.Value;
                }
                else
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                    return BadRequest(ResponseFromService<string>.Failure(notif));
                }
            }

            // 2) Llamada al servicio principal
            var response = await _recetaService.GetRecetasByIdPacienteAsync(
                idUsuario,
                filterRequest.IdPaciente.Value,
                filterRequest.StartDate,
                filterRequest.EndDate,
                filterRequest.DateFilter
            );

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("GetRecetaByIdReceta")]
        public async Task<IActionResult> GetRecetaByIdReceta([FromBody] RecetaRequest request)
        {
            if (!HasPermission("GetRecetaByIdReceta"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (request.IdReceta == Guid.Empty
                || request.IdPaciente == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 1) Llamada al servicio
            var response = await _recetaService.GetRecetaByIdRecetaAsync(request.IdReceta, request.IdPaciente);

            // 2) Validación de toast + caso “no encontrada”
            if (response.Toast == "error" || response.Toast == "warning")
            {
                return BadRequest(response);
            }
            if (string.IsNullOrEmpty(response.Data))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("RECETAS", "RECETA_NO_ENCONTRADA");
                return Ok(ResponseFromService<string>.Success(null, notif));
            }

            // 3) Éxito
            return Content(response.Data, "text/html");
        }

        [HttpPost("GetRecetaUpdateByIdReceta")]
        public async Task<IActionResult> GetRecetaUpdateByIdReceta([FromBody] RecetaRequest request)
        {
            var idUsuarioClaim = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 1) Permiso
            if (!HasPermission("GetRecetaUpdateByIdReceta"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (request.IdReceta == Guid.Empty
                || request.IdPaciente == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }
            // 1) Llamada al servicio
            var response = await _recetaService.GetRecetaUpdatgeByIdRecetaAsync(request.IdReceta, request.IdPaciente, idUsuario);

            // 2) Validación de toast + caso “no encontrada”
            //if (response.Toast == "error" || response.Toast == "warning")
            //{
            //    return BadRequest(response);
            //}

            // 3) Éxito
            return Ok(response);
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

            var response = await _recetaService.ObtenerRecetasPorMedicoAsync(idUsuario, idSucursal);

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
            if (!Guid.TryParse(idUsuarioClaim, out _))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var response = await _recetaService.ObtenerRecetasPorMedicoAsync(idMedico, idSucursal);

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
            var response = await _recetaService.ConsultarRecetaAsync(idReceta, idUsuario);

            // 5) Evaluar Toast
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("GetQRByIdReceta")]
        public async Task<IActionResult> GetQRByIdReceta([FromBody] Guid IdReceta)
        {
            if (!HasPermission("GetQRByIdReceta"))
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

            var pacienteResp = await _recetaService.GetIdPacienteByUsuarioAsync(idUsuario);
            if (pacienteResp.Toast == "error" || pacienteResp.Toast == "warning")
            {
                return NotFound(pacienteResp);
            }
            if (pacienteResp.Data.HasValue)
            {
                var response = await _recetaService.GetQRAsync(IdReceta, pacienteResp.Data.Value);
                return (response.Toast == "success" || response.Toast == "info")
                    ? Ok(response)
                    : BadRequest(response);
            }
            else
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }               
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.RecetaController.EndpointRolesRecetaController[endpointName].Contains(rol);
        }
    }
}
