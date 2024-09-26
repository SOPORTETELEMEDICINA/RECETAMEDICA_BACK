using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;

namespace RMD.Controllers.Vidal.ByMolecule
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class MoleculeController : ControllerBase
    {
        private readonly IMoleculeService _moleculeService;

        public MoleculeController(IMoleculeService moleculeService)
        {
            _moleculeService = moleculeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMolecules()
        {
            try
            {
                var molecules = await _moleculeService.GetAllMoleculesAsync();
                return Ok(molecules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMoleculeById(int id)
        {
            var molecule = await _moleculeService.GetMoleculeById(id);

            if (molecule == null)
            {
                return NotFound();
            }

            return Ok(molecule);
        }
    }
}
