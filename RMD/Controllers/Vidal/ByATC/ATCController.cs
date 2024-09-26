using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;

namespace RMD.Controllers.Vidal.ByATC
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class ATCController : Controller
    {
        private readonly IATCService _atcService;

        public ATCController(IATCService atcService)
        {
            _atcService = atcService;
        }

        [HttpGet("GetATCAll")]
        public async Task<IActionResult> GetATCAll()
        {
            try
            {
                var classifications = await _atcService.GetAllATCClassificationsAsync();
                return Ok(classifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{atcId}/vmps")]
        public async Task<IActionResult> GetVmpsByAtcClassification(int atcId)
        {
            try
            {
                var vmps = await _atcService.GetVmpsByAtcClassificationAsync(atcId);
                return Ok(vmps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{atcId}/products")]
        public async Task<IActionResult> GetProductsByAtcClassification(int atcId)
        {
            try
            {
                var products = await _atcService.GetProductsByAtcClassificationAsync(atcId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{atcId}/children")]
        public async Task<IActionResult> GetAtcChildren(int atcId)
        {
            try
            {
                var children = await _atcService.GetAtcChildrenAsync(atcId);
                return Ok(children);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{atcId}")]
        public async Task<IActionResult> GetAtcClassificationById(int atcId)
        {
            try
            {
                var atcDetails = await _atcService.GetAtcClassificationByIdAsync(atcId);
                return Ok(atcDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
