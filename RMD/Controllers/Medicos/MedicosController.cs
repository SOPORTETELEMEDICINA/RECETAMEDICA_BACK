using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Medicos;
using RMD.Models.Medicos;
using RMD.Models.Responses;
using System.Net;
using System.Security.Claims;

namespace RMD.Controllers.Medicos
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicosController(IMedicoService medicoService) : ControllerBase
    {
        private readonly IMedicoService _medicoService = medicoService;

        [HttpGet("ByIdMedico/{id}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetMedicoByIdMedico(Guid id)
        {
            try
            {
                var medico = await _medicoService.GetMedicoByIdMedicoAsync(id);
                if (medico == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "Médico no encontrado."));
                }

                var response = ResponseFromService<UsuarioMedico>.Success(medico, "Médico obtenido con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("ByIdUsuario/{id}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetMedicoByIdUsuario(Guid id)
        {
            try
            {
                var medico = await _medicoService.GetMedicoByIdUsuarioAsync(id);
                if (medico == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "Médico no encontrado."));
                }

                var response = ResponseFromService<UsuarioMedico>.Success(medico, "Médico obtenido con éxito.");
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
        public async Task<IActionResult> GetMedicoByName(string nombreBusqueda)
        {
            try
            {
                var medico = await _medicoService.GetMedicoByNameAsync(nombreBusqueda);
                if (medico == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "Médico no encontrado."));
                }

                var response = ResponseFromService<UsuarioMedico>.Success(medico, "Médico obtenido con éxito.");
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
        public async Task<IActionResult> CreateMedico([FromBody] MedicoCreate medico)
        {
            try
            {
                // Obtener el IdRol del token JWT
                var idRol = User.FindFirstValue("IdRol");

                // Convertir a GUID
                if (!Guid.TryParse(idRol, out Guid idRolGuid))
                {
                    return BadRequest("IdRol no es válido.");
                }

                var result = await _medicoService.CreateMedicoAsync(medico, idRolGuid);
                if (!result)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Error al crear el médico."));
                }

                var response = ResponseFromService<string>.Success(null, "Médico creado con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> UpdateMedico([FromBody] Medico medico)
        {
            try
            {
                // Obtener el IdUsuario del token JWT
                var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");

                // Convertir a GUID
                if (!Guid.TryParse(idUsuarioSolicitante, out Guid idUsuarioSolicitanteGuid))
                {
                    return BadRequest("IdUsuario no es válido.");
                }

                // Llamar al servicio para actualizar el médico
                var result = await _medicoService.UpdateMedicoAsync(medico, idUsuarioSolicitanteGuid);
                if (!result)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Error al actualizar el médico."));
                }

                var response = ResponseFromService<string>.Success(null, "Médico actualizado con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpPost("pacientes-por-sucursal")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacientesBySucursal()
        {
            try
            {
                // Obtener el IdUsuario desde el token
                var idUsuario = User.FindFirstValue("IdUsuario");
                if (string.IsNullOrEmpty(idUsuario))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "No se pudo obtener el IdUsuario del token."));
                }

                // Convertir el IdUsuario a GUID
                if (!Guid.TryParse(idUsuario, out var idUsuarioGuid))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdUsuario en el token no es válido."));
                }

                // Llamar al servicio para obtener los pacientes asociados al IdUsuario
                var pacientes = await _medicoService.GetPacientesBySucursalListAsync(idUsuarioGuid);

                // Devolver un modelo vacío si no se encontraron pacientes
                var responsePacientes = pacientes ?? new List<PacientePorSucursalListModel>();

                return Ok(ResponseFromService<IEnumerable<PacientePorSucursalListModel>>.Success(responsePacientes, "Pacientes obtenidos con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }
    }
}
