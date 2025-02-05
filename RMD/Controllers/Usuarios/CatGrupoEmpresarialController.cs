using RMD.Extensions;
using RMD.Interface.Usuarios;
using RMD.Models.Responses;
using RMD.Models.Usuarios;

namespace RMD.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatGrupoEmpresarialController(ICatGrupoEmpresarialService grupoEmpresarialService) : ControllerBase
    {
        private readonly ICatGrupoEmpresarialService _grupoEmpresarialService = grupoEmpresarialService;

        /// <summary>
        /// Gets all business groups.
        /// </summary>
        /// <returns>A list of business groups.</returns>
        [HttpGet("GetAllGrupoEmpresarial")]
        public async Task<IActionResult> GetAllGrupoEmpresarial()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.CatGrupoEmpresarialController.EndpointRolesCatGrupoEmpresarialController["GetAllGrupoEmpresarial"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            try
            {
                var result = await _grupoEmpresarialService.GetAllGrupoEmpresarial();
                var response = ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Success(result, "Lista de grupos empresariales obtenida con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets a business group by its ID.
        /// </summary>
        /// <param name="id">The ID of the business group.</param>
        /// <returns>The requested business group.</returns>
        [HttpGet("GetGrupoEmpresarialById/{id}")]
        public async Task<IActionResult> GetGrupoEmpresarialById(Guid id)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.CatGrupoEmpresarialController.EndpointRolesCatGrupoEmpresarialController["GetGrupoEmpresarialById"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El Id proporcionado no es válido."));
            }

            try
            {
                var result = await _grupoEmpresarialService.GetGrupoEmpresarialById(id);
                if (result == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "No se encontró el grupo empresarial especificado."));
                }

                var response = ResponseFromService<CatGrupoEmpresarial>.Success(result, "Grupo empresarial obtenido con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        /// <summary>
        /// Creates a new business group.
        /// </summary>
        /// <param name="grupoEmpresarial">The business group model.</param>
        /// <returns>A message indicating the result of the operation.</returns>
        [HttpPost("CreateGrupoEmpresarial")]
        public async Task<IActionResult> CreateGrupoEmpresarial([FromBody] CatGrupoEmpresarial grupoEmpresarial)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.CatGrupoEmpresarialController.EndpointRolesCatGrupoEmpresarialController["CreateGrupoEmpresarial"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            if (grupoEmpresarial == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El modelo GrupoEmpresarial proporcionado no es válido."));
            }

            try
            {
                var result = await _grupoEmpresarialService.CreateGrupoEmpresarial(grupoEmpresarial);
                var response = ResponseFromService<string>.Success(result, "Grupo empresarial creado con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        /// <summary>
        /// Updates an existing business group.
        /// </summary>
        /// <param name="grupoEmpresarial">The updated business group model.</param>
        /// <returns>A message indicating the result of the operation.</returns>
        [HttpPut("UpdateGrupoEmpresarial")]
        public async Task<IActionResult> UpdateGrupoEmpresarial([FromBody] CatGrupoEmpresarial grupoEmpresarial)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.CatGrupoEmpresarialController.EndpointRolesCatGrupoEmpresarialController["UpdateGrupoEmpresarial"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
            if (grupoEmpresarial == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El modelo GrupoEmpresarial proporcionado no es válido."));
            }

            try
            {
                var result = await _grupoEmpresarialService.UpdateGrupoEmpresarial(grupoEmpresarial);
                var response = ResponseFromService<string>.Success(result, "Grupo empresarial actualizado con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }
    }
}
