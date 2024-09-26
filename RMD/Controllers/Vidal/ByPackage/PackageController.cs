using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByPackage;

namespace RMD.Controllers.Vidal.ByPackage
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Package>>> GetAllPackages()
        {
            var packages = await _packageService.GetAllPackagesAsync();
            return Ok(packages);
        }

        [HttpGet("{packageId}/routes")]
        public async Task<IActionResult> GetPackageRoutes(int packageId)
        {
            var packageRoutes = await _packageService.GetPackageRoutesAsync(packageId);
            return Ok(packageRoutes);
        }

        [HttpGet("{packageId}/indicators")]
        public async Task<IActionResult> GetPackageIndicators(int packageId)
        {
            var packageIndicators = await _packageService.GetPackageIndicatorsAsync(packageId);
            return Ok(packageIndicators);
        }

        [HttpGet("{packageId}/units")]
        public async Task<IActionResult> GetPackageUnits(int packageId)
        {
            var units = await _packageService.GetPackageUnitsByPackageIdAsync(packageId);
            return Ok(units);
        }

        [HttpGet("{packageId}/indications")]
        public async Task<IActionResult> GetPackageIndications(int packageId)
        {
            var indications = await _packageService.GetPackageIndicationsByPackageIdAsync(packageId);
            return Ok(indications);
        }

        [HttpGet("{packageId}/side-effects")]
        public async Task<IActionResult> GetPackageSideEffects(int packageId)
        {
            var sideEffects = await _packageService.GetPackageSideEffectsAsync(packageId);
            return Ok(sideEffects);
        }

        [HttpGet("{packageId}/vmp")]
        public async Task<IActionResult> GetVmpByPackageId(int packageId)
        {
            try
            {
                var vmpPackage = await _packageService.GetVmpByPackageIdAsync(packageId);
                if (vmpPackage == null)
                {
                    return NotFound();
                }
                return Ok(vmpPackage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{packageId}/product")]
        public async Task<IActionResult> GetPackageProductById(string packageId)
        {
            var packageProduct = await _packageService.GetPackageProductByIdAsync(packageId);

            if (packageProduct == null)
            {
                return NotFound();
            }

            return Ok(packageProduct);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetPackagesByName([FromQuery] string q)
        {
            try
            {
                var packages = await _packageService.GetPackagesByNameAsync(q);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
