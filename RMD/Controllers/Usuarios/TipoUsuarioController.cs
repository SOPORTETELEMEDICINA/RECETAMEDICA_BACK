using Microsoft.AspNetCore.Mvc;
using RMD.Interface.Usuarios;
using RMD.Models.Usuarios;
using RMD.Models.Responses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RMD.Extensions;

namespace RMD.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class TipoUsuarioController(ITipoUsuarioService tipoUsuarioService) : ControllerBase
    {
        private readonly ITipoUsuarioService _tipoUsuarioService = tipoUsuarioService;

        /// <summary>
        /// Gets all user types.
        /// </summary>
        /// <returns>A list of user types.</returns>
        [HttpGet("GetAllTipoUsuario")]
        public async Task<IActionResult> GetAllTipoUsuario()
        {
            try
            {
                var result = await _tipoUsuarioService.GetAllTipoUsuario();
                var response = ResponseFromService<IEnumerable<TipoUsuario>>.Success(result, "Lista de tipos de usuario obtenida con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets a user type by its ID.
        /// </summary>
        /// <param name="id">The ID of the user type.</param>
        /// <returns>The requested user type.</returns>
        [HttpGet("GetTipoUsuarioById/{id}")]
        public async Task<IActionResult> GetTipoUsuarioById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El Id proporcionado no es válido."));
            }

            try
            {
                var result = await _tipoUsuarioService.GetTipoUsuarioById(id);
                if (result == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "No se encontró el tipo de usuario especificado."));
                }

                var response = ResponseFromService<TipoUsuario>.Success(result, "Tipo de usuario obtenido con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        /// <summary>
        /// Creates a new user type.
        /// </summary>
        /// <param name="tipoUsuario">The user type model.</param>
        /// <returns>A message indicating the result of the operation.</returns>
        [HttpPost("CreateTipoUsuario")]
        public async Task<IActionResult> CreateTipoUsuario([FromBody] TipoUsuario tipoUsuario)
        {
            if (tipoUsuario == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El modelo TipoUsuario proporcionado no es válido."));
            }

            try
            {
                var result = await _tipoUsuarioService.CreateTipoUsuario(tipoUsuario);
                var response = ResponseFromService<string>.Success(result, "Tipo de usuario creado con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        /// <summary>
        /// Updates an existing user type.
        /// </summary>
        /// <param name="tipoUsuario">The updated user type model.</param>
        /// <returns>A message indicating the result of the operation.</returns>
        [HttpPut("UpdateTipoUsuario")]
        public async Task<IActionResult> UpdateTipoUsuario([FromBody] TipoUsuario tipoUsuario)
        {
            if (tipoUsuario == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El modelo TipoUsuario proporcionado no es válido."));
            }

            try
            {
                var result = await _tipoUsuarioService.UpdateTipoUsuario(tipoUsuario);
                var response = ResponseFromService<string>.Success(result, "Tipo de usuario actualizado con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }
    }
}
