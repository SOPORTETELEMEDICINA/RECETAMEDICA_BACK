using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByProduct;
using System.Net;

namespace RMD.Controllers.Vidal.ByProduct
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

       
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                // Llamar al servicio para obtener el producto por Id
                var product = await _productService.GetProductByIdAsync(productId);

                if (product == null)
                {
                    return NotFound(ResponseFromService<ProductModel>.Failure(HttpStatusCode.NotFound, "Producto no encontrado."));
                }

                // Retornar el producto usando ResponseFromService
                return Ok(ResponseFromService<ProductModel>.Success(product, "Producto recuperado exitosamente."));
            }
            catch (Exception ex)
            {
                // Manejar errores
                return StatusCode(500, ResponseFromService<ProductModel>.Failure(HttpStatusCode.InternalServerError, $"Error interno del servidor: {ex.Message}"));
            }
        }

        //[HttpGet("{productId}")]
        //public async Task<IActionResult> GetProductById(int productId)
        //{
        //    try
        //    {
        //        // Llamar al servicio para obtener el producto por Id
        //        var product = await _productService.GetProductByIdAsync(productId);

        //        if (product == null)
        //        {
        //            return NotFound(ResponseFromService<ProductModel>.Failure(HttpStatusCode.NotFound, "Producto no encontrado."));
        //        }

        //        // Retornar el producto usando ResponseFromService
        //        return Ok(ResponseFromService<ProductModel>.Success(product, "Producto recuperado exitosamente."));
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejar errores
        //        return StatusCode(500, ResponseFromService<ProductModel>.Failure(HttpStatusCode.InternalServerError, $"Error interno del servidor: {ex.Message}"));
        //    }
        //}


        //[HttpGet]
        //public async Task<object> GetProducts()
        //{
        //    try
        //    {
        //        //List<Product> producto = new List<Product>();
        //        var products = JsonConvert.SerializeObject(await _productService.GetAllProductsAsync());

        //       // producto = products.ToList();
        //        // Convertir el JSON a bytes
        //       // var byteSize = Encoding.UTF8.GetByteCount(json);
        //        return Ok(products);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejo de errores
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        //[HttpGet("{productId}")]
        //public async Task<IActionResult> GetProductById(int id)
        //{
        //    var product = await _productService.GetProductByIdAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(product);
        //}

        [HttpGet("{productId}/packages")]
        public async Task<IActionResult> GetProductPackages(int productId)
        {
            try
            {
                var packages = await _productService.GetProductPackagesAsync(productId);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/molecules")]
        public async Task<ActionResult<List<ProductMolecule>>> GetProductMolecules(int productId)
        {
            try
            {
                var molecules = await _productService.GetProductMoleculesAsync(productId);
                return Ok(molecules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/foreign-products")]
        public async Task<ActionResult<List<ProductForeign>>> GetProductForeign(int productId)
        {
            try
            {
                var foreignProducts = await _productService.GetProductForeignAsync(productId);
                return Ok(foreignProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/indications")]
        public async Task<ActionResult<List<ProductIndication>>> GetProductIndications(int productId)
        {
            try
            {
                var indications = await _productService.GetProductIndicationsAsync(productId);
                return Ok(indications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/ucds")]
        public async Task<ActionResult<List<ProductUcd>>> GetProductUcds(int productId)
        {
            try
            {
                var ucds = await _productService.GetProductUcdsAsync(productId);
                return Ok(ucds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/units")]
        public async Task<ActionResult<List<ProductUnit>>> GetProductUnits(int productId)
        {
            try
            {
                var units = await _productService.GetProductUnitsAsync(productId);
                return Ok(units);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/routes")]
        public async Task<IActionResult> GetProductRoutes(int productId)
        {
            try
            {
                var routes = await _productService.GetProductRoutesAsync(productId);
                return Ok(routes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/indicators")]
        public async Task<IActionResult> GetProductIndicators(int productId)
        {
            try
            {
                var indicators = await _productService.GetProductIndicatorsAsync(productId);
                return Ok(indicators);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/side-effects")]
        public async Task<IActionResult> GetProductSideEffects(int productId)
        {
            try
            {
                var sideEffects = await _productService.GetProductSideEffectsAsync(productId);
                return Ok(sideEffects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/ucdvs")]
        public async Task<IActionResult> GetProductUCDV(int productId)
        {
            try
            {
                var ucdvs = await _productService.GetProductUCDVAsync(productId);
                return Ok(ucdvs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/allergies")]
        public async Task<IActionResult> GetProductAllergy(int productId)
        {
            try
            {
                var allergies = await _productService.GetProductAllergyAsync(productId);
                return Ok(allergies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}/atc-classification")]
        public async Task<IActionResult> GetProductAtcClassification(int productId)
        {
            try
            {
                var atcClassification = await _productService.GetProductAtcClassificationAsync(productId);
                return Ok(atcClassification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{productGroupId}/vmp")]
        public async Task<IActionResult> GetVmpByProductGroup(int productGroupId)
        {
            try
            {
                var vmpGroup = await _productService.GetVmpByProductGroupAsync(productGroupId);
                if (vmpGroup == null)
                {
                    return NotFound();
                }
                return Ok(vmpGroup);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetProductsByName([FromQuery] string q)
        {
            try
            {
                var products = await _productService.GetProductsByNameAsync(q);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
