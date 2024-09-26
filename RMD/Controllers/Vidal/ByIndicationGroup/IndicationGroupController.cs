using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByIndicationGroup;
using System.Net;

namespace RMD.Controllers.Vidal.ByIndicationGroup
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class IndicationGroupController : ControllerBase
    {
        private readonly IIndicationGroupService _indicationGroupService;

        public IndicationGroupController(IIndicationGroupService indicationGroupService)
        {
            _indicationGroupService = indicationGroupService;
        }

        [HttpGet("{indicationGroupId}")]
        public async Task<IActionResult> GetIndicationGroup(int indicationGroupId)
        {
            var indicationGroup = await _indicationGroupService.GetIndicationGroupByIdAsync(indicationGroupId);

            if (indicationGroup == null)
            {
                return Ok(ResponseFromService<IndicationGroup>.Failure(HttpStatusCode.NotFound, "Indication group not found."));
            }

            return Ok(ResponseFromService<IndicationGroup>.Success(indicationGroup, "Indication group found."));
        }

        [HttpGet("{indicationGroupId}/products")]
        public async Task<IActionResult> GetProductsByIndicationGroupId(int indicationGroupId)
        {
            try
            {
                var products = await _indicationGroupService.GetProductsByIndicationGroupIdAsync(indicationGroupId);

                if (products == null || products.Count == 0)
                {
                    return Ok(ResponseFromService<List<IndicationGroupProduct>>.Failure(HttpStatusCode.NotFound, "No products found for this indication group."));
                }

                return Ok(ResponseFromService<List<IndicationGroupProduct>>.Success(products, "Products retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<IndicationGroupProduct>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{indicationGroupId}/cim10s")]
        public async Task<IActionResult> GetCIM10Entries(int indicationGroupId)
        {
            var cim10Entries = await _indicationGroupService.GetCIM10EntriesAsync(indicationGroupId);

            if (cim10Entries == null || cim10Entries.Count == 0)
            {
                return Ok(ResponseFromService<List<CIM10>>.Failure(HttpStatusCode.NotFound, "No CIM10 entries found."));
            }

            return Ok(ResponseFromService<List<CIM10>>.Success(cim10Entries, "CIM10 entries retrieved successfully."));
        }

        [HttpGet("{indicationGroupId}/vmps")]
        public async Task<IActionResult> GetVMPsByIndicationGroupId(int indicationGroupId)
        {
            var vmps = await _indicationGroupService.GetVMPsByIndicationGroupIdAsync(indicationGroupId);

            if (vmps == null || vmps.Count == 0)
            {
                return Ok(ResponseFromService<List<VMP>>.Failure(HttpStatusCode.NotFound, "No VMPs found for this indication group."));
            }

            return Ok(ResponseFromService<List<VMP>>.Success(vmps, "VMPs retrieved successfully."));
        }

        [HttpGet("{indicationGroupId}/indications")]
        public async Task<IActionResult> GetIndicationsByIndicationGroupId(int indicationGroupId)
        {
            var indications = await _indicationGroupService.GetIndicationsByIndicationGroupIdAsync(indicationGroupId);

            if (indications == null || indications.Count == 0)
            {
                return Ok(ResponseFromService<List<Indication>>.Failure(HttpStatusCode.NotFound, "No indications found for this indication group."));
            }

            return Ok(ResponseFromService<List<Indication>>.Success(indications, "Indications retrieved successfully."));
        }
    }
}
