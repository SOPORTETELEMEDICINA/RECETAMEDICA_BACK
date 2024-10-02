using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;
using RMD.Models.Vidal;
using System.Security.Claims;

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

        [HttpPost("create")]
        public async Task<IActionResult> CreateReceta([FromBody] RecetaRecibidaModel recetaRecibida)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            try
            {
                // Generar un nuevo Guid para IdReceta si no está presente
                var idReceta = recetaRecibida.IdReceta == Guid.Empty ? Guid.NewGuid() : recetaRecibida.IdReceta;
                
                // Convertir listas en strings delimitados por ";"
                var alergiasString = string.Join(";", recetaRecibida.Alergias.Select(a => a.IdAllergy));
                var moleculesString = string.Join(";", recetaRecibida.Molecules.Select(m => m.IdMolecule));
                var patologiasString = string.Join(";", recetaRecibida.Patologias.Select(p => p.IdCIM10));

                // Crear el RecetaModel a partir del RecetaRecibidaModel
                var recetaModel = new RecetaModel
                {
                    IdReceta = idReceta, // Usar el nuevo Guid generado si no estaba presente
                    IdMedico = recetaRecibida.IdMedico,
                    IdPaciente = recetaRecibida.IdPaciente,
                    PacPeso = recetaRecibida.PacPeso,
                    PacTalla = recetaRecibida.PacTalla,
                    PacEmbarazo = recetaRecibida.PacEmbarazo,
                    PacSemAmenorrea = recetaRecibida.PacSemAmenorrea,
                    PacLactancia = recetaRecibida.PacLactancia,
                    PacCreatinina = recetaRecibida.PacCreatinina,
                    Alergias = alergiasString, // Convertido a string delimitado por ";"
                    Molecules = moleculesString, // Convertido a string delimitado por ";"
                    Patologias = patologiasString, // Convertido a string delimitado por ";"
                    IdSucursal = recetaRecibida.IdSucursal,
                    IdGEMP = recetaRecibida.IdGEMP
                };

                // Convertir detalles de receta recibidos a RecetaDetalleModel
                var detallesRecetaModel = recetaRecibida.DetallesReceta.Select(detalle => new RecetaDetalleModel
                {
                    IdDetalleReceta = detalle.IdDetalleReceta == Guid.Empty ? Guid.NewGuid() : detalle.IdDetalleReceta, // Generar nuevo IdDetalleReceta si no está presente
                    IdReceta = idReceta, // Usar el mismo IdReceta generado arriba
                    Medicamento = detalle.Medicamento,
                    CantidadDiaria = detalle.CantidadDiaria,
                    UnidadDispensacion = detalle.UnidadDispensacion,
                    RutaAdministracion = detalle.RutaAdministracion,
                    Indicacion = detalle.Indicacion,
                    Duracion = detalle.Duracion,
                    UnidadDuracion = detalle.UnidadDuracion,
                    PeriodoInicio = detalle.PeriodoInicio,
                    PeriodoTerminacion = detalle.PeriodoTerminacion
                }).ToList();

                // Llamar al servicio para crear la receta y los detalles
                var result = await _recetaService.CreateRecetaAsync(recetaModel, detallesRecetaModel);

                if (!result)
                {
                    return BadRequest("No se pudo crear la receta.");
                }

                return Ok("Receta creada con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }

        [HttpGet("GetByIdRecetaByMedico/{idReceta}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetRecetaByIdRecetaByMedico(Guid idReceta)
        {
            // Obtener el IdUsuario del token
            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var idUsuarioGuid))
            {
                return BadRequest("El IdUsuario no es válido.");
            }

            try
            {
                // Llamar al servicio para obtener la receta y detalles
                var recetaConDetalles = await _recetaService.GetRecetaByIdRecetaByMedicoAsync(idReceta, idUsuarioGuid);

                if (recetaConDetalles == null)
                {
                    return NotFound("No se encontró la receta o el médico no está autorizado.");
                }

                return Ok(recetaConDetalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }

        [HttpPut("UpdateReceta")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> UpdateReceta([FromBody] RecetaRecibidaModel recetaRecibida)
        {
            //if (!IsUserAuthorized())
            //{
            //    return Forbid("No tiene permisos para realizar esta acción.");
            //}

            //// Obtener el IdUsuario desde el token
            //var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == "IdUsuario");
            //if (idUsuarioClaim == null)
            //{
            //    return Forbid("No se pudo identificar al usuario.");
            //}

            //var idUsuario = Guid.Parse(idUsuarioClaim.Value);

            try
            {
                //    // Convertir las listas en strings delimitados por ";"
                //    var alergiasString = string.Join(";", recetaRecibida.Alergias.Select(a => a.IdAllergy));
                //    var moleculesString = string.Join(";", recetaRecibida.Molecules.Select(m => m.IdMolecule));
                //    var patologiasString = string.Join(";", recetaRecibida.Patologias.Select(p => p.IdCIM10));

                //    // Crear el RecetaModel para la actualización
                //    var recetaModel = new RecetaModel
                //    {
                //        IdReceta = recetaRecibida.IdReceta,
                //        IdMedico = recetaRecibida.IdMedico,
                //        IdPaciente = recetaRecibida.IdPaciente,
                //        PacPeso = recetaRecibida.PacPeso,
                //        PacTalla = recetaRecibida.PacTalla,
                //        PacEmbarazo = recetaRecibida.PacEmbarazo,
                //        PacSemAmenorrea = recetaRecibida.PacSemAmenorrea,
                //        PacLactancia = recetaRecibida.PacLactancia,
                //        PacCreatinina = recetaRecibida.PacCreatinina,
                //        Alergias = alergiasString,  // Convertido a string delimitado por ";"
                //        Molecules = moleculesString,  // Convertido a string delimitado por ";"
                //        Patologias = patologiasString,  // Convertido a string delimitado por ";"
                //        IdSucursal = recetaRecibida.IdSucursal,
                //        IdGEMP = recetaRecibida.IdGEMP
                //    };

                //    // Llamar al servicio para actualizar la receta
                //    var result = await _recetaService.UpdateRecetaAsync(recetaModel, recetaRecibida.DetallesReceta, idUsuario);

                //    if (!result)
                //    {
                //        return BadRequest("No se pudo actualizar la receta o no tiene permisos.");
                //    }

                return Ok("Receta actualizada con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }










        [HttpGet("{id}")]
        public async Task<ActionResult<Receta>> GetRecetaById(Guid id)
        {
            var receta = await _recetaService.GetRecetaByIdAsync(id);
            if (receta == null)
            {
                return NotFound();
            }
            return Ok(receta);
        }

        /// <summary>
        /// Gets prescriptions by doctor ID.
        /// </summary>
        /// <param name="idMedico">The doctor ID.</param>
        /// <returns>A list of prescriptions.</returns>
        [HttpGet("medico/{idMedico}")]
        public async Task<ActionResult<IEnumerable<Receta>>> GetRecetasByMedico(Guid idMedico)
        {
            var recetas = await _recetaService.GetRecetasByMedicoAsync(idMedico);
            if (recetas == null || !recetas.Any())
            {
                return NotFound();
            }
            return Ok(recetas);
        }

        /// <summary>
        /// Gets prescriptions by patient ID.
        /// </summary>
        /// <param name="idPaciente">The patient ID.</param>
        /// <returns>A list of prescriptions.</returns>
        [HttpGet("paciente/{idPaciente}")]
        public async Task<ActionResult<IEnumerable<Receta>>> GetRecetasByPaciente(Guid idPaciente)
        {
            var recetas = await _recetaService.GetRecetasByPacienteAsync(idPaciente);
            if (recetas == null || !recetas.Any())
            {
                return NotFound();
            }
            return Ok(recetas);
        }

        private bool IsUserAuthorized()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var roleIdClaim = user?.FindFirst("IdRol")?.Value;

            // Validar si el IdRol del token es uno de los permitidos
            return roleIdClaim == "7905213C-B0CB-4D42-A997-20094EF41F9C" ||
                   roleIdClaim == "DE5DFDDC-F6CC-4B7F-B805-286732501E57";
        }
    }
}
