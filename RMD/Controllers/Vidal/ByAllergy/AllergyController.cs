using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByAllergy;

namespace RMD.Controllers.Vidal.ByAllergy
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class AllergyController : ControllerBase
    {
        private readonly IAllergyService _allergyService;

        public AllergyController(IAllergyService allergyService)
        {
            _allergyService = allergyService;
        }

        [HttpGet("{allergyId}")]
        public async Task<ActionResult<AllergyEntry>> GetAllergyById(int allergyId)
        {
            try
            {
                var allergy = await _allergyService.GetAllergyByIdAsync(allergyId);
                return Ok(allergy);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{allergyId}/molecules")]
        public async Task<ActionResult<List<AllergyMolecule>>> GetMoleculesByAllergyId(int allergyId)
        {
            try
            {
                var molecules = await _allergyService.GetMoleculesByAllergyIdAsync(allergyId);
                return Ok(molecules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

     

        [HttpGet]
        public async Task<IActionResult> GetAllAllergies()
        {
            try
            {
                var allergies = await _allergyService.GetAllAllergiesAsync();
                return Ok(allergies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
