using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Sucursales;
using RMD.Models.Sucursales;

namespace RMD.Controllers.Sucursales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class SucursalesController : ControllerBase
    {
        private readonly ISucursalService _sucursalService;

        public SucursalesController(ISucursalService sucursalService)
        {
            _sucursalService = sucursalService;
        }

        /// <summary>
        /// Gets a branch by its ID.
        /// </summary>
        /// <param name="id">The ID of the branch.</param>
        /// <returns>The requested branch.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Sucursal>> GetSucursalById(Guid id)
        {
            var sucursal = await _sucursalService.GetSucursalByIdAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }
            return Ok(sucursal);
        }

        /// <summary>
        /// Gets branches by GEMP ID.
        /// </summary>
        /// <param name="idGEMP">The GEMP ID.</param>
        /// <returns>A list of branches.</returns>
        [HttpGet("gemp/{idGEMP}")]
        public async Task<ActionResult<IEnumerable<Sucursal>>> GetSucursalesByGEMP(Guid idGEMP)
        {
            var sucursales = await _sucursalService.GetSucursalesByGEMPAsync(idGEMP);
            if (sucursales == null || !sucursales.Any())
            {
                return NotFound();
            }
            return Ok(sucursales);
        }

        /// <summary>
        /// Gets branches by settlement ID.
        /// </summary>
        /// <param name="idAsentamiento">The settlement ID.</param>
        /// <returns>A list of branches.</returns>
        [HttpGet("asentamiento/{idAsentamiento}")]
        public async Task<ActionResult<IEnumerable<Sucursal>>> GetSucursalesByAsentamiento(int idAsentamiento)
        {
            var sucursales = await _sucursalService.GetSucursalesByAsentamientoAsync(idAsentamiento);
            if (sucursales == null || !sucursales.Any())
            {
                return NotFound();
            }
            return Ok(sucursales);
        }
    }
}
