using RMD.Extensions;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;
using RMD.Models.Responses;
using RMD.Models.Sucursales;

namespace RMD.Controllers.Recetas
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class RecetasController(IRecetaService recetaService, IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        private readonly IRecetaService _recetaService = recetaService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        //[HttpGet("GetByIdRecetaByMedico/{idReceta}")]
        //[Authorize]
        //[ServiceFilter(typeof(ValidateTokenFilter))]
        //public async Task<IActionResult> GetRecetaByIdRecetaByMedico(Guid idReceta)
        //{
        //    // Obtener el IdUsuario del token
        //    var idUsuario = User.FindFirstValue("IdUsuario");
        //    if (!Guid.TryParse(idUsuario, out var idUsuarioGuid))
        //    {
        //        return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El IdUsuario no es válido."));
        //    }

        //    try
        //    {
        //        // Llamar al servicio para obtener la receta y detalles
        //        var recetaConDetalles = await _recetaService.GetRecetaByIdRecetaByMedicoAsync(idReceta, idUsuarioGuid);

        //        if (recetaConDetalles == null)
        //        {
        //            // Retornar modelo vacío con mensaje informativo
        //            return Ok(ResponseFromService<object>.Success(null, "No se encontró la receta o el médico no está autorizado."));
        //        }

        //        // Retornar la receta encontrada con mensaje de éxito
        //        return Ok(ResponseFromService<object>.Success(recetaConDetalles, "Receta obtenida exitosamente."));
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejo de errores con ResponseFromService
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
        //    }
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecetaById(Guid id)
        {
            try
            {
                // Llamar al servicio para obtener la receta por ID
                var receta = await _recetaService.GetRecetaByIdAsync(id);

                if (receta == null)
                {
                    // Retornar un modelo vacío con un mensaje informativo
                    return Ok(ResponseFromService<Receta>.Success(null, "No se encontró la receta especificada."));
                }

                return Ok(ResponseFromService<Receta>.Success(receta, "Receta obtenida exitosamente."));
            }
            catch (Exception ex)
            {
                // Manejo de errores con ResponseFromService
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }


        /// <summary>
        /// Gets prescriptions by doctor ID.
        /// </summary>
        /// <param name="idMedico">The doctor ID.</param>
        /// <returns>A list of prescriptions.</returns>
        [HttpGet("medico/{idMedico}")]
        public async Task<IActionResult> GetRecetasByMedico(Guid idMedico)
        {
            try
            {
                // Llamar al servicio para obtener las recetas por médico
                var recetas = await _recetaService.GetRecetasByMedicoAsync(idMedico);

                if (recetas == null || !recetas.Any())
                {
                    // Retornar un modelo vacío y un mensaje informativo
                    return Ok(ResponseFromService<List<Receta>>.Success(new List<Receta>(), "No se encontraron recetas para el médico especificado."));
                }

                return Ok(ResponseFromService<IEnumerable<Receta>>.Success(recetas, "Recetas obtenidas exitosamente."));
            }
            catch (Exception ex)
            {
                // Manejo de errores con ResponseFromService
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }


        /// <summary>
        /// Gets prescriptions by patient ID.
        /// </summary>
        /// <param name="idPaciente">The patient ID.</param>
        /// <returns>A list of prescriptions.</returns>
        [HttpGet("paciente/{idPaciente}")]
        public async Task<IActionResult> GetRecetasByPaciente(Guid idPaciente)
        {
            try
            {
                // Llamar al servicio para obtener las recetas por paciente
                var recetas = await _recetaService.GetRecetasByPacienteAsync(idPaciente);

                if (recetas == null || !recetas.Any())
                {
                    // Retornar un modelo vacío y un mensaje informativo
                    return Ok(ResponseFromService<List<Receta>>.Success(new List<Receta>(), "No se encontraron recetas para el paciente especificado."));
                }

                return Ok(ResponseFromService<IEnumerable<Receta>>.Success(recetas, "Recetas obtenidas exitosamente."));
            }
            catch (Exception ex)
            {
                // Manejo de errores con ResponseFromService
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }


        [HttpPost("GetFilteredRecetas")]
        public async Task<IActionResult> GetFilteredRecetas([FromBody] RecetaFilterRequest filterRequest)
        {
            // Obtener los valores del token
            var rol = User.FindFirstValue(ClaimTypes.Role); // TipoUsuario del token
            var idGEMPFromToken = User.FindFirstValue("GEMP");
            var idSucursalFromToken = User.FindFirstValue("IdSucursal");

            if (string.IsNullOrEmpty(rol))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "No se encontró el rol en el token."));
            }

            try
            {
                Guid idGEMP;
                Guid? idSucursal = null;

                // Validar y asignar los valores según el rol
                if (rol == "Empleado de Farmacia" || rol == "Encargado de Farmacia")
                {
                    // Empleado o encargado: datos estrictamente del token
                    if (string.IsNullOrEmpty(idGEMPFromToken) || string.IsNullOrEmpty(idSucursalFromToken))
                        return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El token no contiene IdGEMP o IdSucursal."));

                    idGEMP = Guid.Parse(idGEMPFromToken);
                    idSucursal = Guid.Parse(idSucursalFromToken);
                }
                else if (rol == "Supervisor Sucursales" || rol == "Medico" || rol == "Particular")
                {
                    // Supervisor, médico o particular: IdGEMP del token, pero requieren IdSucursal
                    if (string.IsNullOrEmpty(idGEMPFromToken))
                        return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "El token no contiene IdGEMP."));

                    idGEMP = Guid.Parse(idGEMPFromToken);
                    idSucursal = filterRequest.IdSucursal ?? throw new ArgumentException("El IdSucursal es obligatorio para este rol.");
                }
                else if (rol == "Super Admin")
                {
                    // Super Admin: todo es pasado en la solicitud
                    idGEMP = filterRequest.IdGEMP ?? throw new ArgumentException("El IdGEMP es obligatorio para Super Admin.");
                    idSucursal = filterRequest.IdSucursal ?? throw new ArgumentException("El IdSucursal es obligatorio para Super Admin.");
                }
                else
                {
                    // Si no es un rol válido
                    return Forbid(ResponseFromService<string>.Failure(HttpStatusCode.Forbidden, "No tiene permisos para acceder a este recurso.").Message);
                }

                // Llamar al servicio para obtener las recetas filtradas
                var recetas = await _recetaService.GetFilteredRecetasAsync(
                    rol,
                    idGEMP,
                    idSucursal,
                    filterRequest.StartDate,
                    filterRequest.EndDate,
                    filterRequest.DateFilter
                );

                if (!recetas.Any())
                {
                    // Retornar un modelo vacío y un mensaje
                    return Ok(ResponseFromService<List<RecetaList>>.Success(new List<RecetaList>(), "No se encontraron recetas con los filtros especificados."));
                }

                return Ok(ResponseFromService<IEnumerable<RecetaList>>.Success(recetas, "Recetas obtenidas exitosamente."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        [HttpPost("GetRecetaByIdReceta")]
        public async Task<IActionResult> GetRecetaByIdReceta([FromBody] RecetaRequest request)
        {
            try
            {
                if (request == null || request.IdReceta == Guid.Empty || request.IdPaciente == Guid.Empty)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Parámetros inválidos."));
                }

                // Llamar al servicio para obtener el PDF
                var htmlContent = await _recetaService.GetRecetaByIdRecetaAsync(request.IdReceta, request.IdPaciente);

                if (htmlContent == null || htmlContent.Length == 0)
                {
                    return Ok(ResponseFromService<string>.Success(null, "No se encontró la receta especificada."));
                }

                // Retornar el PDF como archivo descargable
                //return File(htmlContent, "application/pdf", "receta_medica.pdf");
                // Retornar el HTML
                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }



        [HttpPost("GetRecetasByIdPaciente")]
        public async Task<IActionResult> GetRecetasByIdPaciente([FromBody] RecetaFilterByPacienteRequest filterRequest)
        {
            try
            {
                var recetas = await _recetaService.GetRecetasByIdPacienteAsync(
                    filterRequest.IdPaciente, 
                    filterRequest.StartDate,
                    filterRequest.EndDate,
                    filterRequest.DateFilter
                );

                if (recetas.Count == 0)
                {
                    // Retornar un modelo vacío y un mensaje
                    return Ok(ResponseFromService<List<RecetaList>>.Success(new List<RecetaList>(), "No se encontraron recetas con los filtros especificados."));
                }

                return Ok(ResponseFromService<IEnumerable<RecetaList>>.Success(recetas, "Recetas obtenidas exitosamente."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }

        //private bool IsUserAuthorized()
        //{
        //    var user = _httpContextAccessor.HttpContext?.User;
        //    var roleIdClaim = user?.FindFirst("IdRol")?.Value;

        //    // Validar si el IdRol del token es uno de los permitidos
        //    return roleIdClaim == "7905213C-B0CB-4D42-A997-20094EF41F9C" ||
        //           roleIdClaim == "DE5DFDDC-F6CC-4B7F-B805-286732501E57";
        //}
    }
}
