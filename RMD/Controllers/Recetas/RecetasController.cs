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
    public class RecetasController : ControllerBase
    {
        private readonly IRecetaService _recetaService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public RecetasController(IRecetaService recetaService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _recetaService = recetaService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        [HttpPost("GetFilteredRecetas")]
        public async Task<IActionResult> GetFilteredRecetas([FromBody] RecetaFilterRequest filterRequest)
        {
            if (!HasPermission("GetFilteredRecetas"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (filterRequest == null || !ModelState.IsValid)
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
        public async Task<IActionResult> GetFilteredRecetasByIdMedico([FromBody] RecetaFilterByMedicoRequest filterRequest)
        {
            if (!HasPermission("GetFilteredRecetasByIdMedico"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (filterRequest == null || !ModelState.IsValid)
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

        [HttpPost("GetRecetaByIdReceta")]
        public async Task<IActionResult> GetRecetaByIdReceta([FromBody] RecetaRequest request)
        {
            if (!HasPermission("GetRecetaByIdReceta"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (request == null
                || request.IdReceta == Guid.Empty
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

        [HttpPost("GetRecetasByIdPaciente")]
        public async Task<IActionResult> GetRecetasByIdPaciente([FromBody] RecetaFilterByPacienteRequest filterRequest)
        {
            if (!HasPermission("GetRecetasByIdPaciente"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            if (filterRequest == null || !ModelState.IsValid)
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
                filterRequest.IdPaciente = pacienteResp.Data.Value;
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
            if(pacienteResp.Toast == "error" || pacienteResp.Toast == "warning")
            {
                return NotFound(pacienteResp);
            }
           

            var response = await _recetaService.GetQRAsync(IdReceta, pacienteResp.Data.Value);

            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.RecetaController.EndpointRolesRecetaController[endpointName].Contains(rol);
        }
    }
}
