using RMD.Interface.Catalogo;
using RMD.Shared.Models.Catalogo;
using RMD.Shared.Models.GlobalResponse;

namespace RMD.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class CatCiudadesController : ControllerBase
    {

        private readonly ICatCiudadesService _service;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public CatCiudadesController(ICatCiudadesService service, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _service = service;
            _catalogoNotificacionService = catalogoNotificacionService;
        }



        #region CRUD para CatCiudades

        [HttpGet("ciudades")]
        public async Task<IActionResult> GetAllCiudades()
        {
            if (!HasPermission("GetAllCiudades"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetAllCiudadesAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("ciudades/{id}")]
        public async Task<IActionResult> GetCiudadById(int id)
        {
            if (!HasPermission("GetCiudadById"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetCiudadByIdAsync(id);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        [HttpGet("ciudades/byName/{name}")]
        public async Task<IActionResult> GetCiudadesByName(string name)
        {
            if (!HasPermission("GetCiudadesByName"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.GetCiudadesByNameAsync(name);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : NotFound(response);
        }

        //[HttpPost("ciudades")]
        //public async Task<IActionResult> CreateCiudad([FromBody] CatCiudades ciudad)
        //{
        //    if (!HasPermission("CreateCiudad"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    ciudad.IdCiudad = 0;
        //    var response = await _service.CreateCiudadAsync(ciudad);
        //    return (response.Toast == "success" || response.Toast == "info")
        //        ? Ok(response)
        //        : BadRequest(response);
        //}

        [HttpPut("ciudades/{id}")]
        public async Task<IActionResult> UpdateCiudad(int id, [FromBody] CatCiudades ciudad)
        {
            if (!HasPermission("UpdateCiudad"))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<int>.Failure(notif));
            }
            var response = await _service.UpdateCiudadAsync(id, ciudad);
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }

        //[HttpDelete("ciudades/{id}")]
        //public async Task<IActionResult> DeleteCiudad(int id)
        //{
        //    if (!HasPermission("DeleteCiudad"))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
        //        return BadRequest(ResponseFromService<int>.Failure(notif));
        //    }
        //    var response = await _service.DeleteCiudadAsync(id);
        //    return (response.Toast == "success" || response.Toast == "info")
        //        ? Ok(response)
        //        : BadRequest(response);
        //}
        #endregion

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.CatalogoController.EndpointRolesCatalogoController[endpointName].Contains(rol);
        }
    }
}
