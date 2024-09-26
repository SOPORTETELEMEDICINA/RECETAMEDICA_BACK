using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;

namespace RMD.Controllers.Recetas
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class RecetasController : ControllerBase
    {
        private readonly IRecetaService _recetaService;

        public RecetasController(IRecetaService recetaService)
        {
            _recetaService = recetaService;
        }

        /// <summary>
        /// Gets a prescription by its ID.
        /// </summary>
        /// <param name="id">The ID of the prescription.</param>
        /// <returns>The requested prescription.</returns>
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
    }
}
