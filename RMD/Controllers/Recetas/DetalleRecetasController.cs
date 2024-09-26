using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;
using RMD.Models.Responses; // Asegúrate de que este using está incluido

namespace RMD.Controllers.Recetas
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class DetalleRecetasController : ControllerBase
    {
        private readonly IDetalleRecetaService _detalleRecetaService;

        public DetalleRecetasController(IDetalleRecetaService detalleRecetaService)
        {
            _detalleRecetaService = detalleRecetaService;
        }

        /// <summary>
        /// Obtiene el detalle de una receta por su ID.
        /// </summary>
        /// <param name="id">El ID del detalle de la receta.</param>
        /// <returns>El detalle de la receta solicitada.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetalleRecetaById(Guid id)
        {
            var detalle = await _detalleRecetaService.GetDetalleRecetaByIdAsync(id);

            // Si no se encuentra, retorna un modelo vacío con un mensaje adecuado
            if (detalle == null)
            {
                return Ok(ResponseFromService<DetalleReceta>.Success(new DetalleReceta(), "No se encontró el detalle de la receta."));
            }

            return Ok(ResponseFromService<DetalleReceta>.Success(detalle));
        }

        /// <summary>
        /// Obtiene los detalles de una receta por el ID de la receta.
        /// </summary>
        /// <param name="idReceta">El ID de la receta.</param>
        /// <returns>Una lista de detalles de la receta.</returns>
        [HttpGet("receta/{idReceta}")]
        public async Task<IActionResult> GetDetalleRecetasByReceta(Guid idReceta)
        {
            var detalles = (await _detalleRecetaService.GetDetalleRecetasByRecetaAsync(idReceta)).ToList();

            // Si no se encuentran detalles, retornamos una lista vacía con un mensaje
            if (detalles == null || detalles.Count == 0)
            {
                return Ok(ResponseFromService<List<DetalleReceta>>.Success(new List<DetalleReceta>(), "No se encontraron detalles para la receta proporcionada."));
            }

            return Ok(ResponseFromService<List<DetalleReceta>>.Success(detalles));
        }

    }
}
