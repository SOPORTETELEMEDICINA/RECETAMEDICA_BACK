using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal;
using System.Net;

namespace RMD.Controllers.Vidal
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
        public async Task<ActionResult<Allergy>> GetAllergyById(int allergyId)
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

        [HttpGet("AllergysByName")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetAllergiesByName(string name)
        {
            try
            {
                var allergyEntries = await _allergyService.GetAllergiesByName(name);

                if (!allergyEntries?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<Allergy>>.Success(new List<Allergy>(), "No se encontraron alergias."));
                }

                return Ok(ResponseFromService<List<Allergy>>.Success(allergyEntries, "Alergias encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
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
