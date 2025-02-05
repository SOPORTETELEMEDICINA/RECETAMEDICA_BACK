using RMD.Extensions;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;
using RMD.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RMD.Controllers.Recetas
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class DetalleRecetasController(IDetalleRecetaService detalleRecetaService, IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        private readonly IDetalleRecetaService _detalleRecetaService = detalleRecetaService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Obtiene el detalle de una receta por su ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetalleRecetaById(Guid id)
        {
            var detalle = await _detalleRecetaService.GetDetalleRecetaByIdAsync(id);

            if (detalle == null)
            {
                return Ok(ResponseFromService<DetalleReceta>.Success(new DetalleReceta(), "No se encontró el detalle de la receta."));
            }

            return Ok(ResponseFromService<DetalleReceta>.Success(detalle));
        }

        /// <summary>
        /// Obtiene los detalles de una receta por el ID de la receta.
        /// </summary>
        [HttpGet("receta/{idReceta}")]
        public async Task<IActionResult> GetDetalleRecetasByReceta(Guid idReceta)
        {
            var detalles = (await _detalleRecetaService.GetDetalleRecetasByRecetaAsync(idReceta)).ToList();

            if (detalles == null || detalles.Count == 0)
            {
                return Ok(ResponseFromService<List<DetalleReceta>>.Success(new List<DetalleReceta>(), "No se encontraron detalles para la receta proporcionada."));
            }

            return Ok(ResponseFromService<List<DetalleReceta>>.Success(detalles));
        }

        /// <summary>
        /// Obtiene la reacción de una receta por ID de receta.
        /// </summary>
        [HttpGet("reaccion")]
        public async Task<IActionResult> GetReaccionByIdReceta([FromBody] DetalleRecetaRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idUsuarioString = user?.FindFirst("IdUsuario")?.Value;

            if (!Guid.TryParse(idUsuarioString, out var idUsuario))
            {
                return BadRequest("El IdUsuario no es un GUID válido.");
            }

            var detalle = await _detalleRecetaService.GetReaccionByIdRecetaAsync(request, idUsuario);

            if (detalle == null)
            {
                return Ok(ResponseFromService<DetalleRecetaResponse>.Failure(HttpStatusCode.NotFound, "No se encontró la reacción de la receta."));
            }

            return Ok(ResponseFromService<DetalleRecetaResponse>.Success(detalle));
        }


        /// <summary>
        /// Crea o actualiza una reacción en la receta.
        /// </summary>
        [HttpPost("crear-actualizar")]
        public async Task<IActionResult> CreateUpdateReaccion([FromBody] DetalleRecetaRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idUsuarioString = user?.FindFirst("IdUsuario")?.Value;

            if (!Guid.TryParse(idUsuarioString, out var idUsuario))
            {
                return BadRequest("El IdUsuario no es un GUID válido.");
            }

            (bool resultado, string outMessage) = await _detalleRecetaService.CreateUpdateReaccionAsync(request, idUsuario);

            if (resultado)
            {
                return Ok(ResponseFromService<bool>.Success(resultado, outMessage));
            }
            else
            {
                return BadRequest(ResponseFromService<bool>.Failure(HttpStatusCode.BadRequest, outMessage));
            }
        }


        /// <summary>
        /// Borra lógicamente una reacción de la receta.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteReaccion([FromBody] DetalleRecetaRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idUsuarioString = user?.FindFirst("IdUsuario")?.Value;

            if (!Guid.TryParse(idUsuarioString, out var idUsuario))
            {
                return BadRequest("El IdUsuario no es un GUID válido.");
            }
            var (resultado, outMessage) = await _detalleRecetaService.DeleteReaccionAsync(request, idUsuario);

            if (resultado)
            {
                return Ok(ResponseFromService<bool>.Success(resultado, outMessage));
            }
            else
            {
                return BadRequest(ResponseFromService<bool>.Failure(HttpStatusCode.BadRequest, outMessage));
            }
        }
    }
}
