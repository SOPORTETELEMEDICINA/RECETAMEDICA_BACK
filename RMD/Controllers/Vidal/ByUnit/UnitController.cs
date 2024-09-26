using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses; // Asegúrate de tener esto para ResponseFromService
using RMD.Models.Vidal.ByUnit;
using System.Net;

namespace RMD.Controllers.Vidal.ByUnit
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitById(int id)
        {
            var unit = await _unitService.GetUnitById(id);

            if (unit == null)
            {
                // Retorna un modelo vacío en lugar de null
                return Ok(ResponseFromService<Unit>.Success(new Unit(), "No unit found, returning empty model."));
            }

            return Ok(ResponseFromService<Unit>.Success(unit, "Unit retrieved successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetUnits()
        {
            try
            {
                var units = await _unitService.GetAllUnitsAsync();
                if (units == null || !units.Any())
                {
                    // Retorna una lista vacía en lugar de null
                    return Ok(ResponseFromService<List<Units>>.Success(new List<Units>(), "No units found, returning empty list."));
                }

                return Ok(ResponseFromService<List<Units>>.Success(units, "Units retrieved successfully."));
            }
            catch (Exception ex)
            {
                // Retorna un error 500 con el mensaje de excepción
                return StatusCode(500, ResponseFromService<List<Units>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
