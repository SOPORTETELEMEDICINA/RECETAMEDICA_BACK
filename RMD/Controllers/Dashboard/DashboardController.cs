using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Dashborad;
using RMD.Models.Dashboard;
using RMD.Models.Responses;
using System.Net;
using System.Security.Claims;

namespace RMD.Controllers.Dashboard
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        [HttpPost("sucursales-pacientes")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetSucursalPacientes()
        {
            try
            {
                // Obtener información del token
                var idGemp = Guid.Parse(User.FindFirst("GEMP")?.Value ?? Guid.Empty.ToString());
                var idSucursal = Guid.Parse(User.FindFirst("IdSucursal")?.Value ?? Guid.Empty.ToString());
                var idTipoUsuario = Guid.Parse(User.FindFirst("IdRol")?.Value ?? Guid.Empty.ToString());

                if (idGemp == Guid.Empty || idSucursal == Guid.Empty || idTipoUsuario == Guid.Empty)
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Error al obtener la información del token."));
                }

                // Obtener la información de las sucursales y los pacientes
                var sucursalPacientes = await _dashboardService.GetSucursalesPacientesAsync(idGemp, idSucursal, idTipoUsuario);

                if (sucursalPacientes == null || !sucursalPacientes.Any())
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "No se encontraron datos."));
                }

                var response = ResponseFromService<object>.Success(sucursalPacientes, "Datos obtenidos con éxito.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Ocurrió un error en el servidor: {ex.Message}"));
            }
        }

        [HttpPost("GetKPIPacientesRecetas")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetKpiPacientesRecetas()
        {
            try
            {
                // Obtener el IdUsuario del token JWT
                var idUsuario = User.FindFirstValue("IdUsuario");
                var idRol = Guid.Parse(User.FindFirst("IdRol")?.Value ?? Guid.Empty.ToString());
                if (string.IsNullOrEmpty(idUsuario))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "No se pudo obtener el IdUsuario del token."));
                }

                // Convertir a GUID
                if (!Guid.TryParse(idUsuario, out var idUsuarioGuid))
                {
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no válido."));
                }

                // Llamar al servicio para obtener la información
                var result = await _dashboardService.GetKPIPacientesRecetas(idUsuarioGuid, idRol);
                if (result == null || result.Count == 0)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "No se encontraron sucursales para el médico."));
                }

                // Devolver la respuesta
                return Ok(ResponseFromService<List<DashBoardKPIPacientesRecetas>>.Success(result, "Sucursales obtenidas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error en el servidor: {ex.Message}"));
            }
        }
    }
}
