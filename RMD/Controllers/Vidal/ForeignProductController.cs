using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;

namespace RMD.Controllers.Vidal
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class ForeignProductController : ControllerBase
    {
        private readonly IForeignProductService _foreignProductService;

        public ForeignProductController(IForeignProductService foreignProductService)
        {
            _foreignProductService = foreignProductService;
        }

        [HttpGet("{foreignProductId}/products")]
        public async Task<IActionResult> GetEquivalentProducts(int foreignProductId)
        {
            var products = await _foreignProductService.GetEquivalentProductsByForeignProductId(foreignProductId);

            if (products == null || products.Count == 0)
            {
                return NotFound();
            }

            return Ok(products);
        }
    }
}
