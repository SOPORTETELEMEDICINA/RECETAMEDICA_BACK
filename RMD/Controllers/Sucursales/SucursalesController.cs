using RMD.Extensions;
using RMD.Interface.Sucursales;
using RMD.Models.Responses;
using RMD.Models.Sucursales;

namespace RMD.Controllers.Sucursales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class SucursalesController(ISucursalService sucursalService
            //, IHttpContextAccessor httpContextAccessor
            ) : ControllerBase
    {
        private readonly ISucursalService _sucursalService = sucursalService;

        [HttpPost]
        public async Task<ActionResult<ResponseFromService<bool>>> CreateSucursal([FromBody] CreateSucursalModel model)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.SucursalesController.EndpointRolesSucursalesController["CreateSucursal"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
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
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.SucursalesController.EndpointRolesSucursalesController["UpdateSucursal"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
            //if (!IsUserAuthorized("Editar"))
            //{
            //    return Forbid("No tiene permisos para realizar esta acción.");
            //}

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
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.SucursalesController.EndpointRolesSucursalesController["DeleteSucursal"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }

            //if (!IsUserAuthorized("Delete"))
            //{
            //    return Forbid("No tiene permisos para realizar esta acción.");
            //}

            var result = await _sucursalService.DeleteSucursalAsync(id);
            if (!result)
            {
                return BadRequest(ResponseFromService<bool>.Failure(HttpStatusCode.BadRequest, "Error al eliminar la sucursal."));
            }
            return Ok(ResponseFromService<bool>.Success(true, "Sucursal eliminada (lógicamente) exitosamente."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseFromService<SucursalRequest>>> GetSucursalById(Guid id)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.SucursalesController.EndpointRolesSucursalesController["GetSucursalById"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
            try
            {
                var sucursal = await _sucursalService.GetSucursalByIdSucursalAsync(id);
                return Ok(ResponseFromService<SucursalRequest>.Success(sucursal, "Sucursal encontrada exitosamente."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseFromService<SucursalRequest>.Failure(HttpStatusCode.NotFound, ex.Message));
            }
        }

        [HttpGet("gemp/{idGEMP}")]
        public async Task<ActionResult<ResponseFromService<IEnumerable<SucursalRequest>>>> GetSucursalesByGEMP(Guid idGEMP)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.SucursalesController.EndpointRolesSucursalesController["GetSucursalesByGEMP"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
            //if (!IsUserAuthorized("Consulta"))
            //{
            //    return Forbid("No tiene permisos para realizar esta acción.");
            //}
            var sucursales = await _sucursalService.GetSucursalesByIdGEMPAsync(idGEMP);
            if (sucursales == null || !sucursales.Any())
            {
                return NotFound(ResponseFromService<IEnumerable<SucursalRequest>>.Failure(HttpStatusCode.NotFound, "No se encontraron sucursales para este grupo empresarial."));
            }

            return Ok(ResponseFromService<IEnumerable<SucursalRequest>>.Success(sucursales, "Sucursales encontradas exitosamente."));
        }

        [HttpGet("gemp/{idGEMP}/asentamiento/{idAsentamiento}")]
        public async Task<ActionResult<ResponseFromService<IEnumerable<SucursalRequest>>>> GetSucursalesByGEMPAndAsentamiento(Guid idGEMP, int idAsentamiento)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.SucursalesController.EndpointRolesSucursalesController["GetSucursalesByGEMPAndAsentamiento"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
            var sucursales = await _sucursalService.GetSucursalesByIdGEMPAndIdAsentamientoAsync(idGEMP, idAsentamiento);
            if (sucursales == null || !sucursales.Any())
            {
                return NotFound(ResponseFromService<IEnumerable<SucursalRequest>>.Failure(HttpStatusCode.NotFound, "No se encontraron sucursales para el grupo empresarial y asentamiento especificados."));
            }

            return Ok(ResponseFromService<IEnumerable<SucursalRequest>>.Success(sucursales, "Sucursales encontradas exitosamente."));
        }

        //private bool IsUserAuthorized( string accion)
        //{
        //    var user = _httpContextAccessor.HttpContext?.User;
        //    var roleIdClaim = user?.FindFirst("IdRol")?.Value?.ToUpper();  // Convertir a mayúsculas

        //    if (accion == "Crear" || accion == "Editar" || accion == "Delete" || accion == "Consultar")
        //    {
        //        return roleIdClaim == "7905213C-B0CB-4D42-A997-20094EF41F9C" ||
        //          roleIdClaim == "68C87CA4-2499-4A9C-B0DC-EEE99B7078BF";
        //    }
        //    else
        //    {
        //        return roleIdClaim == "7905213C-B0CB-4D42-A997-20094EF41F9C" ||
        //          roleIdClaim == "DE5DFDDC-F6CC-4B7F-B805-286732501E57" ||
        //          roleIdClaim == "68C87CA4-2499-4A9C-B0DC-EEE99B7078BF";
        //    }
        //    // Validar si el IdRol del token es uno de los permitidos
           
        //}

    }
}
