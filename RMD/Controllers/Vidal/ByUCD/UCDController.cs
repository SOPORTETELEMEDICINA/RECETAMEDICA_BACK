using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByUCD;
using System.Net;

namespace RMD.Controllers.Vidal.ByUCD
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class UCDController : ControllerBase
    {
        private readonly IUCDService _ucdService;

        public UCDController(IUCDService ucdService)
        {
            _ucdService = ucdService;
        }

        [HttpGet("{packageId}")]
        public async Task<IActionResult> GetUcdByIdPackage(int packageId)
        {
            try
            {
                var ucd = await _ucdService.GetUcdByIdPackageAsync(packageId);
                if (ucd == null)
                {
                    return Ok(ResponseFromService<UCDByIdPackage>.Failure(HttpStatusCode.NotFound, $"No UCD found for PACKAGE ID {packageId}."));
                }
                return Ok(ResponseFromService<UCDByIdPackage>.Success(ucd, "UCD retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<UCDByIdPackage>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("ucd/{ucdId}")]
        public async Task<IActionResult> GetUcdById(int ucdId)
        {
            try
            {
                var ucd = await _ucdService.GetUcdByIdAsync(ucdId);
                if (ucd == null)
                {
                    return Ok(ResponseFromService<UCDById>.Failure(HttpStatusCode.NotFound, $"No UCD found for UCD ID {ucdId}."));
                }
                return Ok(ResponseFromService<UCDById>.Success(ucd, "UCD retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<UCDById>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("ucd/{ucdId}/packages")]
        public async Task<IActionResult> GetUcdPackagesById(int ucdId)
        {
            try
            {
                var packages = await _ucdService.GetUcdPackagesByIdAsync(ucdId);
                if (packages == null || packages.Count == 0)
                {
                    return Ok(ResponseFromService<List<UcdPackage>>.Failure(HttpStatusCode.NotFound, $"No packages found for UCD ID {ucdId}."));
                }
                return Ok(ResponseFromService<List<UcdPackage>>.Success(packages, "Packages retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<UcdPackage>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("ucd/{ucdId}/products")]
        public async Task<IActionResult> GetUcdProductsById(int ucdId)
        {
            try
            {
                var products = await _ucdService.GetUcdProductsByIdAsync(ucdId);
                if (products == null || products.Count == 0)
                {
                    return Ok(ResponseFromService<List<UcdProduct>>.Failure(HttpStatusCode.NotFound, $"No products found for UCD ID {ucdId}."));
                }
                return Ok(ResponseFromService<List<UcdProduct>>.Success(products, "Products retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<UcdProduct>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{ucdId}/side-effects")]
        public async Task<IActionResult> GetSideEffectsByUcdId(int ucdId)
        {
            try
            {
                var sideEffects = await _ucdService.GetSideEffectsByUcdIdAsync(ucdId);
                if (sideEffects == null || sideEffects.Count == 0)
                {
                    return Ok(ResponseFromService<List<UCDSideEffect>>.Failure(HttpStatusCode.NotFound, $"No side effects found for UCD ID {ucdId}."));
                }
                return Ok(ResponseFromService<List<UCDSideEffect>>.Success(sideEffects, "Side effects retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<UCDSideEffect>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> GetUcds()
        {
            try
            {
                var ucds = await _ucdService.GetAllUcdsAsync();
                return Ok(ResponseFromService<List<Ucd>>.Success(ucds, "UCDs retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<Ucd>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
