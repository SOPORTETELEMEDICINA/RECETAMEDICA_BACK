using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByIndication;
using RMD.Service.Vidal.ByForeignProduct;
using System.Net;

namespace RMD.Controllers.Vidal.ByIndication
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class IndicationController : Controller
    {
        private readonly IIndicationService _indicationService;

        public IndicationController(IIndicationService indicationService)
        {
            _indicationService = indicationService;
        }
        [HttpGet("indication/{indicationId}")]

        public async Task<IActionResult> GetIndicationById(int indicationId)
        {
            try
            {
                var indication = await _indicationService.GetIndicationByIdAsync(indicationId);
                if (indication == null)
                {
                    return NotFound($"Indication with ID {indicationId} not found.");
                }
                return Ok(indication);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("indication/{indicationId}/products")]
        public async Task<IActionResult> GetProductsByIndicationId(int indicationId)
        {
            try
            {
                var products = await _indicationService.GetProductsByIndicationIdAsync(indicationId);

                // Si no se encontraron productos, retornamos una lista vacía
                if (products == null || products.Count == 0)
                {
                    return Ok(ResponseFromService<List<IndicationProduct>>.Success(new List<IndicationProduct>(), "No se encontraron productos para la indicación proporcionada."));
                }

                return Ok(ResponseFromService<List<IndicationProduct>>.Success(products));
            }
            catch (Exception ex)
            {
                // Manejo del error en el controlador
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, ex.Message));
            }
        }


        [HttpGet("indication/{indicationId}/vmps")]
        public async Task<IActionResult> GetVmpsByIndicationId(int indicationId)
        {
            try
            {
                var vmps = await _indicationService.GetVmpsByIndicationIdAsync(indicationId);

                // Si no se encontraron productos, retornamos una lista vacía
                if (vmps == null || vmps.Count == 0)
                {
                    return Ok(ResponseFromService<List<IndicationVMP>>.Success(new List<IndicationVMP>(), "No se encontraron productos para la indicación proporcionada."));
                }

                return Ok(ResponseFromService<List<IndicationVMP>>.Success(vmps));
            }
            catch (Exception ex)
            {
                // Manejo del error en el controlador, si el ID no existe o ocurre otro error
                return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, ex.Message));
            }
        }



    }
}
