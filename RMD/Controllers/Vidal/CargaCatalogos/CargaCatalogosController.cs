using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using System.Net;

namespace RMD.Controllers.Vidal.CargaCatalogos
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class CargaCatalogosController : ControllerBase
    {
        private readonly ICargaCatalogosService _cargaCatalogosService;

        public CargaCatalogosController(ICargaCatalogosService cargaCatalogosService)
        {
            _cargaCatalogosService = cargaCatalogosService;
        }

        [HttpGet("vmps")]
        public async Task<IActionResult> GetVMPs()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllVMPsAsync();

                if (result.Data == null) // Si falla la operación
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, result);
                }

                // Retorna el resultado de éxito
                return Ok(result);
            }
            catch (Exception ex)
            {
                // En caso de un error no controlado
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllProductsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("packages")]
        public async Task<IActionResult> GetPackages()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllPackagesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("units")]
        public async Task<IActionResult> GetUnits()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllUnitsAsync();

                if (result.Data == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, result);
                }

                // Retorna el resultado de éxito
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("allergies")]
        public async Task<IActionResult> GetAllergies()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllAllergiesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error interno del servidor: {ex.Message}"));
            }
        }


        [HttpGet("molecules")]
        public async Task<IActionResult> GetMolecules()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllMoleculesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("routes")]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllRoutesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpGet("cim10")]
        public async Task<IActionResult> GetCIM10()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllCIM10Async();

                if (result.Data == null) // Si falla la operación
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, result);
                }

                // Retorna el resultado de éxito
                return Ok(result);
            }
            catch (Exception ex)
            {
                // En caso de un error no controlado
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vtms")]
        public async Task<IActionResult> GetVTMs()
        {
            try
            {
                var result = await _cargaCatalogosService.GetAllVTMsAsync();

                if (result.Data == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

    }
}
