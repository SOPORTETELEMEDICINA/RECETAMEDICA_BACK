using RMD.Extensions;
using RMD.Interface.Medicos;
using RMD.Models.Medicos;
using RMD.Models.Responses;

namespace RMD.Controllers.Medicos
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class MedicosController(IMedicoService medicoService) : ControllerBase
    {
        private readonly IMedicoService _medicoService = medicoService;

        [HttpGet("ByIdMedico/{idMedico}")]
        public async Task<IActionResult> GetMedicoByIdMedico(Guid idMedico)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.MedicosController.EndpointRolesMedicosController["GetMedicoByIdMedico"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            try
            {
                var medico = await _medicoService.GetMedicoByIdMedicoAsync(idMedico);
                if (medico == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "Médico no encontrado."));
                }

                return Ok(ResponseFromService<MedicoConsultaRequest>.Success(medico, "Médico obtenido con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("BySucursal/{idSucursal}")]
        public async Task<IActionResult> GetMedicosBySucursal(Guid idSucursal)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.MedicosController.EndpointRolesMedicosController["GetMedicosBySucursal"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            try
            {
                var medicos = await _medicoService.GetMedicosBySucursalAsync(idSucursal);
                if (!medicos.Any())
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "No se encontraron médicos para la sucursal especificada."));
                }

                return Ok(ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(medicos, "Médicos obtenidos con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("ByGEMP/{idGEMP}")]
        public async Task<IActionResult> GetMedicosByGEMP(Guid idGEMP)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.MedicosController.EndpointRolesMedicosController["GetMedicosByGEMP"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            try
            {
                var medicos = await _medicoService.GetMedicosByGEMPAsync(idGEMP);
                if (!medicos.Any())
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "No se encontraron médicos para el GEMP especificado."));
                }

                return Ok(ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(medicos, "Médicos obtenidos con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("ByIdUsuario/{id}")]
        public async Task<IActionResult> GetMedicoByIdUsuario(Guid id)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.MedicosController.EndpointRolesMedicosController["GetMedicoByIdUsuario"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            try
            {
                var medico = await _medicoService.GetMedicoByIdUsuarioAsync(id);
                if (medico == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "Médico no encontrado."));
                }

                return Ok(ResponseFromService<MedicoConsultaRequest>.Success(medico, "Médico obtenido con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMedico([FromBody] MedicoCreate medico)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.MedicosController.EndpointRolesMedicosController["CreateMedico"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            try
            {
                var idRol = User.FindFirstValue("IdRol");

                if (!Guid.TryParse(idRol, out Guid idRolGuid))
                {
                    return BadRequest("IdRol no es válido.");
                }

                var result = await _medicoService.CreateMedicoAsync(medico, idRolGuid);
                if (!result)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Error al crear el médico."));
                }

                return Ok(ResponseFromService<string>.Success(null, "Médico creado con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMedico([FromBody] Medico medico)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.MedicosController.EndpointRolesMedicosController["UpdateMedico"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            try
            {
                var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");

                if (!Guid.TryParse(idUsuarioSolicitante, out Guid idUsuarioSolicitanteGuid))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no es válido."));
                }

                var resultMessage = await _medicoService.UpdateMedicoAsync(medico, idUsuarioSolicitanteGuid);

                if (resultMessage == "Médico actualizado con éxito.")
                {
                    return Ok(ResponseFromService<string>.Success(null, resultMessage));
                }

                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, resultMessage));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpDelete("{idMedico}")]
        public async Task<IActionResult> DeleteMedico(Guid idMedico)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.MedicosController.EndpointRolesMedicosController["DeleteMedico"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            try
            {
                var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");

                if (!Guid.TryParse(idUsuarioSolicitante, out Guid idUsuarioSolicitanteGuid))
                {
                    return BadRequest("IdUsuario no es válido.");
                }

                var result = await _medicoService.DeleteMedicoAsync(idMedico, idUsuarioSolicitanteGuid);
                if (!result)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Error al eliminar el médico."));
                }

                return Ok(ResponseFromService<string>.Success(null, "Médico eliminado con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }
    }
}
