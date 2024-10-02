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

        [HttpGet("ByIdMedico/{idMedico}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetMedicoByIdMedico(Guid idMedico)
        {
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
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetMedicosBySucursal(Guid idSucursal)
        {
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
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetMedicosByGEMP(Guid idGEMP)
        {
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

                var response = ResponseFromService<MedicoConsultaRequest>.Success(medico, "Médico obtenido con éxito.");
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

                var response = ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(medico, "Médico obtenido con éxito.");
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
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> UpdateMedico([FromBody] Medico medico)
        {
            try
            {
                var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");

                if (!Guid.TryParse(idUsuarioSolicitante, out Guid idUsuarioSolicitanteGuid))
                {
                    return BadRequest("IdUsuario no es válido.");
                }

                var result = await _medicoService.UpdateMedicoAsync(medico, idUsuarioSolicitanteGuid);
                if (!result)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Error al actualizar el médico."));
                }

                return Ok(ResponseFromService<string>.Success(null, "Médico actualizado con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpDelete("{idMedico}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> DeleteMedico(Guid idMedico)
        {
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
