using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Pacientes;
using RMD.Models.Pacientes;
using RMD.Models.Responses;
using System.Net;
using System.Security.Claims;

namespace RMD.Controllers.Pacientes
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController(IPacienteService pacienteService, IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        private readonly IPacienteService _pacienteService = pacienteService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        [HttpGet("ByIdUsuario/{idUsuario}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacienteByIdUsuario(Guid idUsuario)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var paciente = await _pacienteService.GetPacienteByIdUsuarioAsync(idUsuario);
                if (paciente == null)
                {
                    return Ok(ResponseFromService<PacienteConsultaRequest>.Success(new PacienteConsultaRequest(), "No se encontró el paciente con el ID de usuario especificado."));
                }
                var response = ResponseFromService<PacienteConsultaRequest>.Success(paciente, "Paciente obtenido con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("ByIdPaciente/{idPaciente}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacienteByIdPaciente(Guid idPaciente)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var paciente = await _pacienteService.GetPacienteByIdPacienteAsync(idPaciente);
                if (paciente == null)
                {
                    return Ok(ResponseFromService<PacienteConsultaRequest>.Success(new PacienteConsultaRequest(), "No se encontró el paciente con el ID de paciente especificado."));
                }
                var response = ResponseFromService<PacienteConsultaRequest>.Success(paciente, "Paciente obtenido con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }


        [HttpGet("ByName/{nombreBusqueda}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacienteByName(string nombreBusqueda)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var pacientes = await _pacienteService.GetPacienteByNameAsync(nombreBusqueda);
                if (pacientes == null || !pacientes.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(new List<PacienteConsultaRequest>(), "No se encontraron pacientes con el nombre especificado."));
                }
                var response = ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(pacientes, "Pacientes obtenidos con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("entidad/{idEntidad}")]
        [Authorize]
        [Obsolete]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacientesByEntidadNacimiento(int idEntidad)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var pacientes = await _pacienteService.GetPacientesByEntidadNacimientoAsync(idEntidad);
                if (pacientes == null || !pacientes.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<Paciente>>.Success(new List<Paciente>(), "No se encontraron pacientes para la entidad especificada."));
                }
                var response = ResponseFromService<IEnumerable<Paciente>>.Success(pacientes, "Pacientes obtenidos con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("BySucursal/{idSucursal}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacientesBySucursal(Guid idSucursal)
        {
            try
            {
                var pacientes = await _pacienteService.GetPacientesBySucursalAsync(idSucursal);
                if (pacientes == null || !pacientes.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(new List<PacienteConsultaRequest>(), "No se encontraron pacientes para la sucursal especificada."));
                }
                var response = ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(pacientes, "Pacientes obtenidos con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("ByGEMP/{idGEMP}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacientesByGEMP(Guid idGEMP)
        {
            try
            {
                var pacientes = await _pacienteService.GetPacientesByGEMPAsync(idGEMP);
                if (pacientes == null || !pacientes.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(new List<PacienteConsultaRequest>(), "No se encontraron pacientes para el grupo empresarial especificado."));
                }
                var response = ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(pacientes, "Pacientes obtenidos con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("ByMedico/{idMedico}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacientesByMedico(Guid idMedico)
        {
            try
            {
                var pacientes = await _pacienteService.GetPacientesByMedicoAsync(idMedico);
                if (pacientes == null || !pacientes.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(new List<PacienteConsultaRequest>(), "No se encontraron pacientes para el médico especificado."));
                }
                var response = ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(pacientes, "Pacientes obtenidos con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }


        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateConListas pacienteRequest)
        {
            try
            {
                if (!IsUserAuthorized())
                {
                    return Forbid("No tiene permisos para realizar esta acción.");
                }

                var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");
                if (string.IsNullOrEmpty(idUsuarioSolicitante))
                {
                    return BadRequest("No se pudo obtener el IdUsuario del token.");
                }

                if (!Guid.TryParse(idUsuarioSolicitante, out var idUsuarioSolicitanteGuid))
                {
                    return BadRequest("IdUsuario no válido.");
                }

                // Llamar al servicio para crear el paciente
                var result = await _pacienteService.CreatePacienteAsync(pacienteRequest, idUsuarioSolicitanteGuid);
                if (!result)
                {
                    return BadRequest("Error al crear el paciente.");
                }

                return Ok("Paciente creado con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> UpdatePaciente(Guid id, [FromBody] PacienteConListas pacienteRequest)
        {
            try
            {
                // Obtener el IdUsuario solicitante del token JWT
                var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");
                if (string.IsNullOrEmpty(idUsuarioSolicitante))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Token Invalido."));
                }

                // Convertir a GUID
                if (!Guid.TryParse(idUsuarioSolicitante, out var idUsuarioSolicitanteGuid))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no válido."));
                }

                // Asignar el ID del paciente que viene en la URL
                pacienteRequest.IdPaciente = id;

                // Llamar al servicio para actualizar el paciente
                var result = await _pacienteService.UpdatePacienteAsync(pacienteRequest, idUsuarioSolicitanteGuid);
                if (!result)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Error al actualizar el paciente."));
                }

                var response = ResponseFromService<string>.Success(null, "Paciente actualizado con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("entidades-federativas")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetEntidadesFederativas()
        {
            try
            {
                var entidades = await _pacienteService.GetEntidadesFederativasAsync();

                if (entidades == null || !entidades.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<EntidadNacimiento>>.Success(new List<EntidadNacimiento>(), "No se encontraron entidades federativas."));
                }

                return Ok(ResponseFromService<IEnumerable<EntidadNacimiento>>.Success(entidades, "Entidades federativas obtenidas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpDelete("{idPaciente}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> EliminarPaciente(Guid idPaciente)
        {
            try
            {
                if (!IsUserAuthorized())
                {
                    return Forbid("No tiene permisos para realizar esta acción.");
                }

                var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");
                if (string.IsNullOrEmpty(idUsuarioSolicitante))
                {
                    return BadRequest("No se pudo obtener el IdUsuario del token.");
                }

                if (!Guid.TryParse(idUsuarioSolicitante, out var idUsuarioSolicitanteGuid))
                {
                    return BadRequest("IdUsuario no válido.");
                }

                var result = await _pacienteService.EliminarPacienteAsync(idPaciente, idUsuarioSolicitanteGuid);
                if (!result)
                {
                    return BadRequest("Error al eliminar el paciente.");
                }

                return Ok("Paciente eliminado con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }


        private bool IsUserAuthorized()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var roleIdClaim = user?.FindFirst("IdRol")?.Value?.ToUpper();  // Convertir a mayúsculas

            // Validar si el IdRol del token es uno de los permitidos
            return roleIdClaim == "7905213C-B0CB-4D42-A997-20094EF41F9C" ||
                   roleIdClaim == "DE5DFDDC-F6CC-4B7F-B805-286732501E57";
        }



    }
}
