using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByUCDV;
using System.Net;

namespace RMD.Controllers.Vidal.ByUCDV
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class UcdvController : ControllerBase
    {
        private readonly IUcdvService _ucdvService;

        public UcdvController(IUcdvService ucdvService)
        {
            _ucdvService = ucdvService;
        }

        [HttpGet]
        [ActionName("GetAllUcdvs")]
        public async Task<IActionResult> GetUcdvs()
        {
            var ucdvs = await _ucdvService.GetAllUcdvsAsync();

            if (ucdvs == null || ucdvs.Count == 0)
            {
                return Ok(ResponseFromService<List<UCDVS>>.Failure(HttpStatusCode.NotFound, "No UCDVs found."));
            }

            return Ok(ResponseFromService<List<UCDVS>>.Success(ucdvs, "UCDVs retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUcdv(int id)
        {
            var ucdv = await _ucdvService.GetUcdvByIdAsync(id);

            if (ucdv == null)
            {
                return Ok(ResponseFromService<UCDV>.Failure(HttpStatusCode.NotFound, "UCDV not found."));
            }

            return Ok(ResponseFromService<UCDV>.Success(ucdv, "UCDV retrieved successfully."));
        }

        [HttpGet("{ucdvId}/routes")]
        public async Task<IActionResult> GetRoutesForUcdv(int ucdvId)
        {
            try
            {
                var routes = await _ucdvService.GetRoutesForUcdvAsync(ucdvId);
                if (routes == null || routes.Count == 0)
                {
                    return Ok(ResponseFromService<List<UCDVRoute>>.Failure(HttpStatusCode.NotFound, "No routes found for this UCDV."));
                }

                return Ok(ResponseFromService<List<UCDVRoute>>.Success(routes, "Routes retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<UCDVRoute>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{ucdvId}/units")]
        public async Task<IActionResult> GetUnitsForUcdv(int ucdvId)
        {
            try
            {
                var units = await _ucdvService.GetUnitsForUcdvAsync(ucdvId);
                if (units == null || units.Count == 0)
                {
                    return Ok(ResponseFromService<List<UCDVUnit>>.Failure(HttpStatusCode.NotFound, "No units found for this UCDV."));
                }

                return Ok(ResponseFromService<List<UCDVUnit>>.Success(units, "Units retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<UCDVUnit>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{ucdvId}/products")]
        public async Task<IActionResult> GetUcdvProducts(int ucdvId)
        {
            try
            {
                var products = await _ucdvService.GetUcdvProductsAsync(ucdvId);
                if (products == null || products.Count == 0)
                {
                    return Ok(ResponseFromService<List<UCDVProduct>>.Failure(HttpStatusCode.NotFound, "No products found for this UCDV."));
                }

                return Ok(ResponseFromService<List<UCDVProduct>>.Success(products, "Products retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<UCDVProduct>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("ucdv/{ucdvId}/molecules")]
        public async Task<IActionResult> GetMoleculesByUCDVId(int ucdvId)
        {
            try
            {
                var molecules = await _ucdvService.GetMoleculesByUCDVIdAsync(ucdvId);
                if (molecules == null || molecules.Count == 0)
                {
                    return Ok(ResponseFromService<List<UCDVMolecule>>.Failure(HttpStatusCode.NotFound, $"No molecules found for UCDV ID {ucdvId}."));
                }

                return Ok(ResponseFromService<List<UCDVMolecule>>.Success(molecules, "Molecules retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<UCDVMolecule>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("packages")]
        [ActionName("GetPackages")]
        public async Task<IActionResult> GetUcdvPackages(int ucdvId)
        {
            try
            {
                var packages = await _ucdvService.GetUcdvPackagesAsync(ucdvId);
                if (packages == null || packages.Count == 0)
                {
                    return Ok(ResponseFromService<List<UCDVPackage>>.Failure(HttpStatusCode.NotFound, $"No packages found for UCDV ID {ucdvId}."));
                }

                return Ok(ResponseFromService<List<UCDVPackage>>.Success(packages, "Packages retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<UCDVPackage>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
