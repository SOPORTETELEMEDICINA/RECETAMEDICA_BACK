using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;

namespace RMD.Controllers.Vidal
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class CIM10Controller : ControllerBase
    {
        private readonly ICIM10Service _cim10Service;

        public CIM10Controller(ICIM10Service cim10Service)
        {
            _cim10Service = cim10Service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCIM10s()
        {
            try
            {
                var cim10s = await _cim10Service.GetAllCIM10sAsync();
                return Ok(cim10s);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCIM10ById(int id)
        {
            try
            {
                var cim10 = await _cim10Service.GetCIM10ByIdAsync(id);
                if (cim10 == null)
                {
                    return NotFound();
                }
                return Ok(cim10);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{parentId}/children")]
        public async Task<IActionResult> GetCIM10Children(int parentId)
        {
            try
            {
                var children = await _cim10Service.GetCIM10ChildrenAsync(parentId);
                if (children == null || children.Count == 0)
                {
                    return NotFound();
                }
                return Ok(children);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
