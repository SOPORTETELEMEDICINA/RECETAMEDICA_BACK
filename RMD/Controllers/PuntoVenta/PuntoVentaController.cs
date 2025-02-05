using RMD.Extensions;
using RMD.Interface.PuntoVenta;
using RMD.Models.PuntoVenta;
using RMD.Models.Responses;

namespace RMD.Controllers.PuntoVenta
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PuntoVentaController(IPuntoVentaService puntoVentaService) : ControllerBase
    {
        private readonly IPuntoVentaService _puntoVentaService = puntoVentaService;

        [HttpPost("buscar-receta")]
        public async Task<IActionResult> BuscarReceta([FromBody] string qrEncriptado)
        {
            try
            {
                // Desencriptar el QR
                var datosDesencriptados = EncryptionHelper.Decrypt(qrEncriptado);

                // Separar los datos (IdReceta|IdMedico|FechaUltimaModificacion)
                var partes = datosDesencriptados.Split('|');
                if (partes.Length != 3)
                {
                    return BadRequest(ResponseFromService<string>.Failure(
                        HttpStatusCode.BadRequest,
                        "QR inválido."
                    ));
                }

                var idReceta = Guid.Parse(partes[0]);
                var idMedico = Guid.Parse(partes[1]);
                var fechaUltimaModificacion = DateTime.Parse(partes[2]);

                // Llamar al servicio para obtener la receta desde la base de datos
                var (receta, detalles, paciente, medico, grupoEmpresarial) =
                    await _puntoVentaService.ObtenerRecetaAsync(idReceta, idMedico, fechaUltimaModificacion);

                if (receta == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(
                        HttpStatusCode.NotFound,
                        "Receta no encontrada o ya no está surtida."
                    ));
                }

                // Preparar la respuesta con los datos obtenidos
                var responseData = new
                {
                    Receta = receta,
                    Detalles = detalles,
                    Paciente = paciente,
                    Medico = medico,
                    GrupoEmpresarial = grupoEmpresarial
                };

                return Ok(ResponseFromService<object>.Success(responseData, "Receta encontrada exitosamente."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(
                    HttpStatusCode.BadRequest,
                    $"Error al procesar el QR: {ex.Message}"
                ));
            }
        }

        [HttpPost("surtir-medicamentos")]
        public async Task<IActionResult> SurtirMedicamentos([FromBody] SurtirRecetaRequest request)
        {
            try
            {
                await _puntoVentaService.SurtirMedicamentosAsync(request.IdReceta, request.DetallesReceta);

                return Ok(ResponseFromService<string>.Success(
                    "Los medicamentos han sido surtidos exitosamente."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(
                    System.Net.HttpStatusCode.BadRequest,
                    $"Error al surtir los medicamentos: {ex.Message}"));
            }
        }

        [HttpGet("consultar-receta/{id}")]
        public async Task<IActionResult> ConsultarRecetaPorId(int id)
        {
            try
            {
                // Llama al servicio que ejecuta el SP
                var (receta, detalles) = await _puntoVentaService.ConsultarRecetaPorIdAsync(id);

                if (receta == null) // Verifica si la receta es null
                {
                    return NotFound(ResponseFromService<string>.Failure(
                        HttpStatusCode.NotFound,
                        "No se encontró ninguna receta con el ID proporcionado."
                    ));
                }

                // Construye la respuesta con receta y detalles
                var response = new
                {
                    Receta = receta,
                    Detalles = detalles
                };

                return Ok(ResponseFromService<object>.Success(
                    response,
                    "Receta obtenida exitosamente."
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(
                    HttpStatusCode.BadRequest,
                    $"Error al consultar la receta: {ex.Message}"
                ));
            }
        }




    }
}
