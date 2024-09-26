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
    public class PacientesController(IPacienteService pacienteService) : ControllerBase
    {
        private readonly IPacienteService _pacienteService = pacienteService;

        [HttpGet("ByIdUsuario/{idUsuario}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacienteByIdUsuario(Guid idUsuario)
        {
            try
            {
                var paciente = await _pacienteService.GetPacienteByIdUsuarioAsync(idUsuario);
                if (paciente == null)
                {
                    return Ok(ResponseFromService<UsuarioPaciente>.Success(new UsuarioPaciente(), "No se encontró el paciente con el ID de usuario especificado."));
                }
                var response = ResponseFromService<UsuarioPaciente>.Success(paciente, "Paciente obtenido con éxito.");
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
            try
            {
                var paciente = await _pacienteService.GetPacienteByIdPacienteAsync(idPaciente);
                if (paciente == null)
                {
                    return Ok(ResponseFromService<UsuarioPaciente>.Success(new UsuarioPaciente(), "No se encontró el paciente con el ID de paciente especificado."));
                }
                var response = ResponseFromService<UsuarioPaciente>.Success(paciente, "Paciente obtenido con éxito.");
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
            try
            {
                var pacientes = await _pacienteService.GetPacienteByNameAsync(nombreBusqueda);
                if (pacientes == null || !pacientes.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<UsuarioPaciente>>.Success(new List<UsuarioPaciente>(), "No se encontraron pacientes con el nombre especificado."));
                }
                var response = ResponseFromService<IEnumerable<UsuarioPaciente>>.Success(pacientes, "Pacientes obtenidos con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpGet("entidad/{idEntidad}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetPacientesByEntidadNacimiento(int idEntidad)
        {
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

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateConListas pacienteRequest)
        {
            try
            {
                // Obtener el IdUsuario solicitante del token JWT
                var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");
                if (string.IsNullOrEmpty(idUsuarioSolicitante))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "No se pudo obtener el IdUsuario del token."));
                }

                // Convertir a GUID
                if (!Guid.TryParse(idUsuarioSolicitante, out var idUsuarioSolicitanteGuid))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no válido."));
                }

                // Llamar al servicio para crear el paciente
                var result = await _pacienteService.CreatePacienteAsync(pacienteRequest, idUsuarioSolicitanteGuid);
                if (!result)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Error al crear el paciente."));
                }

                var response = ResponseFromService<string>.Success(null, "Paciente creado con éxito.");
                return Ok(response);
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
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "No se pudo obtener el IdUsuario del token."));
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

    }
}
