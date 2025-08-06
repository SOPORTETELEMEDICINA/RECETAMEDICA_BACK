using RMD.Interface.Pacientes;

namespace RMD.Controllers.Pacientes
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class PacientesCatalogoController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacientesCatalogoController(
            IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet("entidades-federativas")]
        public async Task<IActionResult> GetEntidadesFederativas()
        {
            // No hay permiso específico, sólo devolvemos el catálogo
            var response = await _pacienteService.GetEntidadesFederativasAsync();
            return (response.Toast == "success" || response.Toast == "info")
                ? Ok(response)
                : BadRequest(response);
        }
    }
}
