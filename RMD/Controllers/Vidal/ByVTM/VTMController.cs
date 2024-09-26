using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Service.Vidal.ByMolecule;

namespace RMD.Controllers.Vidal.ByVTM
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class VTMController : ControllerBase
    {
        private readonly IVTMService _vtmService;

        public VTMController(IVTMService vtmService)
        {
            _vtmService = vtmService;
        }

        [HttpGet("vtm/{vtmId}")]
        public async Task<IActionResult> GetVTMById(int vtmId)
        {
            var vtm = await _vtmService.GetVTMById(vtmId);

            if (vtm == null)
            {
                return NotFound();
            }

            return Ok(vtm);
        }

        [HttpGet]
        public async Task<IActionResult> GetVtms()
        {
            var vtms = await _vtmService.GetVtmsAsync();
            return Ok(vtms);
        }

        [HttpGet("{id}/molecules")]
        public async Task<IActionResult> GetMoleculesByVtmId(int vtmId)
        {
            var molecules = await _vtmService.GetMoleculesByVtmIdAsync(vtmId);
            return Ok(molecules);
        }
    }
}
