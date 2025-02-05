using RMD.Extensions;
using RMD.Interface.Catalogo;
using RMD.Interface.Consulta;
using RMD.Interface.Pacientes;
using RMD.Models.Consulta;
using RMD.Models.Pacientes;
using RMD.Models.Responses;

namespace RMD.Controllers.Consulta
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class ConsultaController(
        IConsultaService consultaService,
        ICatalogoService catalogoService,
        IPacienteService pacienteService,
        IHttpContextAccessor httpContextAccessor

            ) : ControllerBase
    {
        private readonly IConsultaService _consultaService = consultaService;
        private readonly ICatalogoService _catalogoService = catalogoService;
        private readonly IPacienteService _pacienteService = pacienteService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        [HttpPost("AllergyByName")]
        public async Task<IActionResult> GetAllergiesByName(string name)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var allergies = await _consultaService.GetAllergiesByNameAsync(name);

                if (!allergies.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<RequestSearchAllergy>>.Success([], "No allergies found."));
                }

                return Ok(ResponseFromService<IEnumerable<RequestSearchAllergy>>.Success(allergies, "Allergies retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
            }
        }

        [HttpPost("MoleculeByName")]
        public async Task<IActionResult> GetMoleculeByName(string name)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var molecules = await _consultaService.GetMoleculeByNameAsync(name);

                if (!molecules.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<RequestSearchMolecules>>.Success(new List<RequestSearchMolecules>(), "No allergies found."));
                }

                return Ok(ResponseFromService<IEnumerable<RequestSearchMolecules>>.Success(molecules, "Allergies retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
            }
        }

        //[HttpPost("VMPByName")]
        //public async Task<IActionResult> GetVMPByName(string name)
        //{
        //    if (!IsUserAuthorized())
        //    {
        //        return Forbid("No tiene permisos para realizar esta acción.");
        //    }
        //    try
        //    {
        //        var vmps = await _consultaService.GetVMPByNameAsync(name);

        //        if (!vmps.Any())
        //        {
        //            return Ok(ResponseFromService<IEnumerable<RequestSearchVMP>>.Success(new List<RequestSearchVMP>(), "No allergies found."));
        //        }

        //        return Ok(ResponseFromService<IEnumerable<RequestSearchVMP>>.Success(vmps, "Allergies retrieved successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
        //    }
        //}

        //[HttpPost("ProductByName")]
        //public async Task<IActionResult> GetProductsByName(string name)
        //{
        //    if (!IsUserAuthorized())
        //    {
        //        return Forbid("No tiene permisos para realizar esta acción.");
        //    }
        //    try
        //    {

        //        var products = await _consultaService.GetProductsByNameAsync(name);

        //        if (!products.Any())
        //        {
        //            return Ok(ResponseFromService<IEnumerable<RequestSearchProducts>>.Success(new List<RequestSearchProducts>(), "No allergies found."));
        //        }

        //        return Ok(ResponseFromService<IEnumerable<RequestSearchProducts>>.Success(products, "Allergies retrieved successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
        //    }
        //}

        //[HttpPost("PackagesByName")]
        //public async Task<IActionResult> GetPackagesByName(string name)
        //{
        //    if (!IsUserAuthorized())
        //    {
        //        return Forbid("No tiene permisos para realizar esta acción.");
        //    }
        //    try
        //    {
        //        var packages = await _consultaService.GetPackagesByNameAsync(name);

        //        if (!packages.Any())
        //        {
        //            return Ok(ResponseFromService<IEnumerable<RequestSearchPackage>>.Success(new List<RequestSearchPackage>(), "No allergies found."));
        //        }

        //        return Ok(ResponseFromService<IEnumerable<RequestSearchPackage>>.Success(packages, "Allergies retrieved successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
        //    }
        //}

        [HttpPost("CIM10ByName")]
        public async Task<IActionResult> GeCIM10ByName(string name)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var packages = await _consultaService.GetCIM10sByNameAsync(name);

                if (!packages.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<RequestSearchCIM10>>.Success(new List<RequestSearchCIM10>(), "No allergies found."));
                }

                return Ok(ResponseFromService<IEnumerable<RequestSearchCIM10>>.Success(packages, "Allergies retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
            }
        }

        [HttpPost("GetRelaciones")]
        public async Task<IActionResult> GetIdsFromLink([FromQuery] int Id, [FromQuery] string IdType , [FromQuery] string RelacionType)
        {
            try
            {
                // Hacer la consulta al servicio y obtener el resultado como JSON o string vacío
                var result = await _consultaService.GetIdsFromLink(Id, IdType, RelacionType);

                // Validar si el resultado está vacío
                if (string.IsNullOrEmpty(result) || result == "{}")
                {
                    // Retornar un modelo vacío en lugar de un string vacío
                    if (RelacionType == "UNITS")
                    {
                        return Ok(ResponseFromService<IEnumerable<UnitModel>>.Success(new List<UnitModel>(), "No units found."));
                    }
                    else if (RelacionType == "ROUTES")
                    {
                        return Ok(ResponseFromService<IEnumerable<RouteModel>>.Success(new List<RouteModel>(), "No routes found."));
                    }
                    else
                    {
                        return BadRequest("Unknown relation type.");
                    }
                }

                // Devolver el JSON que ya fue generado en el servicio (puede ser una lista de UNITS o ROUTES)
                return Ok(result); // Retorna el JSON como string
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener los datos: {ex.Message}"));
            }
        }



        [HttpPost("Analisis")]
        public async Task<IActionResult> AnalyzePrescription([FromBody] PrescriptionModel request)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            if (request == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Datos de solicitud inválidos."));
            }

            try
            {
                // Llamar al servicio para procesar la solicitud
                var response = await _consultaService.ProcessPrescriptionRequest(request);

                // Devolver el HTML recibido como contenido principal
                return Ok(new
                {
                    HtmlResponse = response.HtmlResponse,
                    MedicamentoActivo = response.MedicamentoActivo // Opcionalmente incluir esta lista
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Se produjo un error al procesar la solicitud: {ex.Message}"));
            }
        }

        [HttpPost("AnalisisXml")]
        public async Task<IActionResult> AnalyzePrescriptionXML([FromBody] PrescriptionModel request)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            if (request == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Datos de solicitud inválidos."));
            }

            try
            {
                // Llamar al servicio para procesar la solicitud
                var response = await _consultaService.ProcessPrescriptionXMLRequest(request);

                // Devolver el XML y MedicamentoActivo como respuesta
                return Ok(new
                {
                    XMLResponse = response.XMLResponse.ToString(),
                    MedicamentoActivo = response.MedicamentoActivo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Se produjo un error al procesar la solicitud: {ex.Message}"));
            }
        }


        [HttpGet("AsentamientoByNames")]
        public async Task<IActionResult> SearchAsentamiento([FromQuery] AsentamientoSearchModel searchModel)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var results = await _catalogoService.BuscarAsentamientosAsync(searchModel);
                if (!results?.Any() ?? true)
                {
                    return Ok(ResponseFromService<IEnumerable<AsentamientoResultModel>>.Success(new List<AsentamientoResultModel>(), "No se encontraron asentamientos."));
                }

                return Ok(ResponseFromService<IEnumerable<AsentamientoResultModel>>.Success(results, "Asentamientos encontrados con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Se produjo un error en el servidor.{ex}"));
            }
        }

        [HttpPost("PacienteByName")]
        public async Task<IActionResult> SearchPacienteByName(string pacientName)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            try
            {
                var rol = User.FindFirstValue(ClaimTypes.Role);

                // Declarar y asignar el GUID en una sola línea
                if (!Guid.TryParse(User.FindFirstValue("GEMP"), out Guid finalIdGemp))
                {
                    return BadRequest("El GEMP proporcionado no es un GUID válido.");
                }

                // Llamar al servicio con el `IdGemp` obtenido
                var pacientes = await _pacienteService.GetPacienteByNameAsync(pacientName, finalIdGemp);

                if (!pacientes?.Any() ?? true)
                {
                    return Ok(ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(new List<PacienteConsultaRequest>(), "No se encontraron pacientes."));
                }

                return Ok(ResponseFromService<IEnumerable<PacienteConsultaRequest>>.Success(pacientes, "Pacientes encontrados con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Se produjo un error en el servidor: {ex.Message}"));
            }
        }


        [HttpPost("MedicamentoByName")]
        public async Task<IActionResult> GetMedicamentoByName(string name)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            try
            {
                var medicamentos = await _consultaService.GetMedicamentoByNameAsync(name);

                if (!medicamentos.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<Medicamentos>>.Success(new List<Medicamentos>(), "No se encontraron medicamentos."));
                }

                return Ok(ResponseFromService<IEnumerable<Medicamentos>>.Success(medicamentos, "Medicamentos encontrados con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener los medicamentos: {ex.Message}"));
            }
        }

        [HttpPost("RegistrarReceta")]
        public async Task<IActionResult> RegistrarReceta([FromBody] RecetaRequestModel request)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            // Usar la propiedad Authorization para obtener el token
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (request == null)
            {
                return BadRequest(ResponseFromService<object>.Failure(HttpStatusCode.BadRequest, "La solicitud es inválida."));
            }

            try
            {
                // Registrar receta y obtener el IdReceta generado
                var idReceta = await _consultaService.RegistrarRecetaAsync(request, token);

                // Retornar mensaje de éxito y el IdReceta en un objeto anónimo
                return Ok(ResponseFromService<object>.Success(new
                {
                    Message = "Receta registrada exitosamente.",
                    IdReceta = idReceta
                }));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<object>.Failure(HttpStatusCode.InternalServerError, $"Error al registrar la receta: {ex.Message}"));
            }
        }

        [HttpDelete("EliminarReceta/{idReceta}")]
        public async Task<IActionResult> EliminarReceta(Guid idReceta)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            // Usar la propiedad Authorization para obtener el token
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var user = _httpContextAccessor.HttpContext?.User;
            var idUsuarioToken = user?.FindFirst("IdUsuario")?.Value?.ToUpper();
            var idRolToken = user?.FindFirst("IdRol")?.Value?.ToUpper();

            try
            {
                if (idRolToken == "7905213C-B0CB-4D42-A997-20094EF41F9C")
                {
                    await _consultaService.EliminarRecetaAsync(idReceta);
                    return Ok(ResponseFromService<string>.Success("Receta eliminada exitosamente."));
                }

                if (!Guid.TryParse(idUsuarioToken, out var idUsuario))
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Token inválido."));
                }

                var idMedico = await _consultaService.ObtenerIdMedicoPorUsuarioAsync(idUsuario);

                if (idMedico == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"No tiene permisos para eliminar esta receta."));
                }

                var receta = await _consultaService.ObtenerRecetaPorIdAsync(idReceta);
                if (receta == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "La receta no existe."));
                }

                if (receta.IdMedico == idMedico)
                {
                    await _consultaService.EliminarRecetaAsync(idReceta);
                    return Ok(ResponseFromService<string>.Success("Receta eliminada exitosamente."));
                }

                return StatusCode((int)HttpStatusCode.InternalServerError,
                  ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"No tiene permisos para eliminar esta receta."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al eliminar la receta: {ex.Message}"));
            }
        }


        [HttpGet("ConsultarReceta/{idReceta}")]
        public async Task<IActionResult> ConsultarReceta(Guid idReceta)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            
            var user = _httpContextAccessor.HttpContext?.User;
            var idUsuarioToken = user?.FindFirst("IdUsuario")?.Value?.ToUpper();
            var idRolToken = user?.FindFirst("IdRol")?.Value?.ToUpper();

            try
            {
                dynamic resultadoFinal = null;
                // Validar si el usuario es Super Admin
                if (idRolToken == "7905213C-B0CB-4D42-A997-20094EF41F9C")
                {
                    var resultadoRecetaAdmin = await _consultaService.ConsultarRecetaAsync(idReceta, null);
                    resultadoFinal = resultadoRecetaAdmin;
                }

                if (!Guid.TryParse(idUsuarioToken, out var idUsuario))
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                        ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Token inválido."));
                }

                // Obtener el IdMedico asociado al IdUsuario
                var idMedico = await _consultaService.ObtenerIdMedicoPorUsuarioAsync(idUsuario);

                //if (idMedico == null)
                //{
                //    if
                //    return StatusCode((int)HttpStatusCode.Forbidden,
                //        ResponseFromService<string>.Failure(HttpStatusCode.Forbidden, "No tiene permisos para consultar esta receta."));
                //}
                // Consultar receta si el IdMedico coincide
                var resultadoRecetaMedico = await _consultaService.ConsultarRecetaAsync(idReceta, idMedico);
                resultadoFinal = resultadoRecetaMedico;
                if (resultadoRecetaMedico == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "La receta no existe."));
                }
                if (resultadoFinal == null)
                {
                    return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "La receta no existe."));
                }
                return Ok(ResponseFromService<dynamic>.Success(resultadoFinal, "Receta consultada exitosamente."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al consultar la receta: {ex.Message}"));
            }
        }


        [HttpPut("ActualizarReceta")]
        public async Task<IActionResult> ActualizarReceta([FromBody] RecetaGetRequest request)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            var user = _httpContextAccessor.HttpContext?.User;
            var idUsuarioToken = user?.FindFirst("IdUsuario")?.Value?.ToUpper();
            var idRolToken = user?.FindFirst("IdRol")?.Value?.ToUpper();

            try
            {
                if (!Guid.TryParse(idUsuarioToken, out var idUsuario))
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                        ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Token inválido."));
                }

                // Validar si el usuario es Super Admin o el médico que creó la receta
                var idMedico = await _consultaService.ObtenerIdMedicoPorUsuarioAsync(idUsuario);
                if (idRolToken != "7905213C-B0CB-4D42-A997-20094EF41F9C" && idMedico != request.Receta.IdMedico)
                {
                    return Forbid("No tiene permisos para actualizar esta receta.");
                }

                await _consultaService.ActualizarRecetaAsync(request);
                return Ok(ResponseFromService<string>.Success("Receta actualizada exitosamente."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al actualizar la receta: {ex.Message}"));
            }
        }

        [HttpGet("RecetasPorMedico")]
        public async Task<IActionResult> ObtenerRecetasPorMedico([FromQuery] Guid idSucursal)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idUsuarioToken = user?.FindFirst("IdUsuario")?.Value?.ToUpper();

            if (!Guid.TryParse(idUsuarioToken, out var idUsuario))
            {
                return Unauthorized("Token inválido.");
            }

            try
            {
                var idMedico = await _consultaService.ObtenerIdMedicoPorUsuarioAsync(idUsuario);

                if (idMedico == null)
                {
                    return Forbid("No tiene permisos para consultar recetas.");
                }

                var recetas = await _consultaService.ObtenerRecetasPorMedicoAsync(idMedico.Value, idSucursal);

                if (!recetas.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<RecetaPacienteModel>>.Success(new List<RecetaPacienteModel>(), "No se encontraron recetas."));
                }

                return Ok(ResponseFromService<IEnumerable<RecetaPacienteModel>>.Success(recetas, "Recetas obtenidas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las recetas: {ex.Message}"));
            }
        }

        [HttpGet("SucursalesPorUsuario")]
        public async Task<IActionResult> ObtenerSucursalesPorUsuario()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idUsuarioToken = user?.FindFirst("IdUsuario")?.Value?.ToUpper();

            if (!Guid.TryParse(idUsuarioToken, out var idUsuario))
            {
                return Unauthorized("Token inválido.");
            }

            try
            {
                var sucursales = await _consultaService.ObtenerSucursalesPorUsuarioAsync(idUsuario);

                if (!sucursales.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<SucursalModel>>.Success(new List<SucursalModel>(), "No se encontraron sucursales."));
                }

                return Ok(ResponseFromService<IEnumerable<SucursalModel>>.Success(sucursales, "Sucursales obtenidas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las sucursales: {ex.Message}"));
            }
        }

        [HttpGet("GetReaccionMedicamentoPrevio/{idPaciente}")]
        public async Task<IActionResult> GetReaccionMedicamentoPrevio(Guid idPaciente)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }

            try
            {
                var reaccionesPrevias = await _consultaService.GetReaccionMedicamentoPrevioAsync(idPaciente);

                if (reaccionesPrevias == null || !reaccionesPrevias.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<DetalleRecetaResponse>>.Success(new List<DetalleRecetaResponse>(), "No se encontraron reacciones previas para el paciente."));
                }

                return Ok(ResponseFromService<IEnumerable<DetalleRecetaResponse>>.Success(reaccionesPrevias, "Reacciones previas encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las reacciones previas del paciente: {ex.Message}"));
            }
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
