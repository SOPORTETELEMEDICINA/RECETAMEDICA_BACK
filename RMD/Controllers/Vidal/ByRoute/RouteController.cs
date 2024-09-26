using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;

namespace RMD.Controllers.Vidal.ByRoute
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        [HttpGet("routes")]
        public async Task<IActionResult> GetAllRoutes()
        {
            try
            {
                var routesResponse = await _routeService.GetAllRoutesAsync();
                if (routesResponse == null || routesResponse.Count == 0)
                {
                    return NotFound("No routes found.");
                }
                return Ok(routesResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoute(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }
    }

}
