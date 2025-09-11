using RMD.Interface.CargaCatalogos;

namespace RMD.Controllers.CargaCatalogos
{
    [ApiController]
    [AllowAnonymous]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class CargaCatalogosController(ICargaCatalogosService cargaCatalogosService, ICargaCatalogosWebService cargaCatalogosWebService) : ControllerBase
    {
        private readonly ICargaCatalogosService _cargaCatalogosService = cargaCatalogosService;
        private readonly ICargaCatalogosWebService _cargaCatalogosWebService = cargaCatalogosWebService;
        [HttpPost("load-catalogs")]
        public async Task<IActionResult> LoadCatalogs()
        {

            await _cargaCatalogosService.ReloadCatalogs();
            await _cargaCatalogosWebService.CargarCatalogosWebService();

            

            //Retorna el resultado de éxito
            return Ok();

        }

    }
}
