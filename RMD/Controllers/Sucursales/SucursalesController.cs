using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Sucursales;
using RMD.Models.Responses;
using RMD.Models.Sucursales;
using System.Net;

namespace RMD.Controllers.Sucursales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class SucursalesController : ControllerBase
    {
        private readonly ISucursalService _sucursalService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SucursalesController(ISucursalService sucursalService, IHttpContextAccessor httpContextAccessor)
        {
            _sucursalService = sucursalService;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost]
        public async Task<ActionResult<ResponseFromService<bool>>> CreateSucursal([FromBody] CreateSucursalModel model)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            var result = await _sucursalService.CreateSucursalAsync(model);
            if (!result)
            {
                return BadRequest(ResponseFromService<bool>.Failure(HttpStatusCode.BadRequest, "Error al crear la sucursal."));
            }
            return Ok(ResponseFromService<bool>.Success(true, "Sucursal creada exitosamente."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseFromService<bool>>> UpdateSucursal(Guid id, [FromBody] UpdateSucursalModel model)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            var result = await _sucursalService.UpdateSucursalAsync(id, model);
            if (!result)
            {
                return BadRequest(ResponseFromService<bool>.Failure(HttpStatusCode.BadRequest, "Error al actualizar la sucursal."));
            }
            return Ok(ResponseFromService<bool>.Success(true, "Sucursal actualizada exitosamente."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseFromService<bool>>> DeleteSucursal(Guid id)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            var result = await _sucursalService.DeleteSucursalAsync(id);
            if (!result)
            {
                return BadRequest(ResponseFromService<bool>.Failure(HttpStatusCode.BadRequest, "Error al eliminar la sucursal."));
            }
            return Ok(ResponseFromService<bool>.Success(true, "Sucursal eliminada (lógicamente) exitosamente."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseFromService<SucursalDomicilioModel>>> GetSucursalById(Guid id)
        {
            try
            {
                var sucursal = await _sucursalService.GetSucursalByIdSucursalAsync(id);
                return Ok(ResponseFromService<SucursalDomicilioModel>.Success(sucursal, "Sucursal encontrada exitosamente."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseFromService<SucursalDomicilioModel>.Failure(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [HttpGet("gemp/{idGEMP}")]
        public async Task<ActionResult<ResponseFromService<IEnumerable<SucursalDomicilioModel>>>> GetSucursalesByGEMP(Guid idGEMP)
        {
            var sucursales = await _sucursalService.GetSucursalesByIdGEMPAsync(idGEMP);
            if (sucursales == null || !sucursales.Any())
            {
                return NotFound(ResponseFromService<IEnumerable<SucursalDomicilioModel>>.Failure(HttpStatusCode.NotFound, "No se encontraron sucursales para este grupo empresarial."));
            }

            return Ok(ResponseFromService<IEnumerable<SucursalDomicilioModel>>.Success(sucursales, "Sucursales encontradas exitosamente."));
        }

        [HttpGet("gemp/{idGEMP}/asentamiento/{idAsentamiento}")]
        public async Task<ActionResult<ResponseFromService<IEnumerable<SucursalDomicilioModel>>>> GetSucursalesByGEMPAndAsentamiento(Guid idGEMP, int idAsentamiento)
        {
            var sucursales = await _sucursalService.GetSucursalesByIdGEMPAndIdAsentamientoAsync(idGEMP, idAsentamiento);
            if (sucursales == null || !sucursales.Any())
            {
                return NotFound(ResponseFromService<IEnumerable<SucursalDomicilioModel>>.Failure(HttpStatusCode.NotFound, "No se encontraron sucursales para el grupo empresarial y asentamiento especificados."));
            }

            return Ok(ResponseFromService<IEnumerable<SucursalDomicilioModel>>.Success(sucursales, "Sucursales encontradas exitosamente."));
        }

        private bool IsUserAuthorized()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var roleIdClaim = user?.FindFirst("IdRol")?.Value?.ToUpper();  // Convertir a mayúsculas

            // Validar si el IdRol del token es uno de los permitidos
            return roleIdClaim == "7905213C-B0CB-4D42-A997-20094EF41F9C" ||
                   roleIdClaim == "DE5DFDDC-F6CC-4B7F-B805-286732501E57";
        }

    }
}
