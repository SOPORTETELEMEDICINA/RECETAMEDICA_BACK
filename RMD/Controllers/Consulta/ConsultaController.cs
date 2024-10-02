using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Catalogos;
using RMD.Interface.Consulta;
using RMD.Interface.Pacientes;
using RMD.Interface.Vidal;
using RMD.Models.Consulta;
using RMD.Models.Pacientes;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByVMP;
using System.Net;

namespace RMD.Controllers.Consulta
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultaController(
        IConsultaService consultaService,
        ICatalogoService catalogoService,
        IPacienteService pacienteService,
        IPackageService packageService,
        IProductService productService,
        IMoleculeService moleculeService,
        IVMPService vmpService,
        IHttpContextAccessor httpContextAccessor

            ) : ControllerBase
    {
        private readonly IConsultaService _consultaService = consultaService;
        private readonly ICatalogoService _catalogoService = catalogoService;
        private readonly IPacienteService _pacienteService = pacienteService;
        private readonly IPackageService _packageService = packageService;
        private readonly IProductService _productService = productService;
        private readonly IMoleculeService _moleculeService = moleculeService;
        private readonly IVMPService _vmpService = vmpService;

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
                    return Ok(ResponseFromService<IEnumerable<RequestSearchAllergy>>.Success(new List<RequestSearchAllergy>(), "No allergies found."));
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

        [HttpPost("VMPByName")]
        public async Task<IActionResult> GetVMPByName(string name)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var vmps = await _consultaService.GetVMPByNameAsync(name);

                if (!vmps.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<RequestSearchVMP>>.Success(new List<RequestSearchVMP>(), "No allergies found."));
                }

                return Ok(ResponseFromService<IEnumerable<RequestSearchVMP>>.Success(vmps, "Allergies retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
            }
        }

        [HttpPost("ProductByName")]
        public async Task<IActionResult> GetProductsByName(string name)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var products = await _consultaService.GetProductsByNameAsync(name);

                if (!products.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<RequestSearchProducts>>.Success(new List<RequestSearchProducts>(), "No allergies found."));
                }

                return Ok(ResponseFromService<IEnumerable<RequestSearchProducts>>.Success(products, "Allergies retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
            }
        }

        [HttpPost("PackagesByName")]
        public async Task<IActionResult> GetPackagesByName(string name)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var packages = await _consultaService.GetPackagesByNameAsync(name);

                if (!packages.Any())
                {
                    return Ok(ResponseFromService<IEnumerable<RequestSearchPackage>>.Success(new List<RequestSearchPackage>(), "No allergies found."));
                }

                return Ok(ResponseFromService<IEnumerable<RequestSearchPackage>>.Success(packages, "Allergies retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al obtener las alergias: {ex.Message}"));
            }
        }

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

        //[HttpGet("VmpByName")]
        //[Authorize]
        //[ServiceFilter(typeof(ValidateTokenFilter))]
        //public async Task<IActionResult> GetVmpByName(string name)
        //{
        //    try
        //    {
        //        var vmpEntries = await _vmpService.GetVmpByName(name);

        //        if (!vmpEntries?.Any() ?? true)
        //        {
        //            return Ok(ResponseFromService<List<VmpEntry>>.Success(new List<VmpEntry>(), "No se encontraron entradas."));
        //        }

        //        return Ok(ResponseFromService<List<VmpEntry>>.Success(vmpEntries, "Entradas encontradas con éxito."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
        //    }
        //}

        [HttpGet("VmpUnit")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchUnitvmp(string link)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var units = await _vmpService.GetVmpUnitsByLinkAsync(link);

                if (!units?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<VMPUnitEntry>>.Success(new List<VMPUnitEntry>(), "No se encontraron unidades."));
                }
                return Ok(ResponseFromService<List<VMPUnitEntry>>.Success(units, "Unidades encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        //[HttpGet("ProductByName")]
        //[AllowAnonymous]
        //[ServiceFilter(typeof(ValidateTokenFilter))]
        //public async Task<IActionResult> GetProductsByName(string name)
        //{
        //    try
        //    {
        //        var productEntries = await _productService.GetProductsByName(name);

        //        if (!productEntries?.Any() ?? true)
        //        {
        //            return Ok(ResponseFromService<List<Models.Vidal.ByProduct.ProductEntry>>.Success(new List<Models.Vidal.ByProduct.ProductEntry>(), "No se encontraron productos."));
        //        }

        //        return Ok(ResponseFromService<List<Models.Vidal.ByProduct.ProductEntry>>.Success(productEntries, "Productos encontrados con éxito."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
        //    }
        //}

        [HttpGet("ProductUnit")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchUnitProduct(string link)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var units = await _productService.GetProductUnitsByLinkAsync(link);

                if (!units?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<Models.Vidal.ByProduct.ProductUnit>>.Success(new List<Models.Vidal.ByProduct.ProductUnit>(), "No se encontraron unidades."));
                }

                return Ok(ResponseFromService<List<Models.Vidal.ByProduct.ProductUnit>>.Success(units, "Unidades encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        //[HttpGet("PackeByName")]
        //[AllowAnonymous]
        //[ServiceFilter(typeof(ValidateTokenFilter))]
        //public async Task<ActionResult<List<PackageEntry>>> GetPackagesByName([FromQuery] string name)
        //{
        //    try
        //    {
        //        var packages = await _packageService.GetPackagesByName(name);
        //        if (!packages?.Any() ?? true)
        //        {
        //            return Ok(ResponseFromService<List<PackageEntry>>.Success(new List<PackageEntry>(), "No se encontraron paquetes."));
        //        }

        //        return Ok(ResponseFromService<List<PackageEntry>>.Success(packages, "Paquetes encontrados con éxito."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
        //    }
        //}

        [HttpGet("PackageUnit")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchUnitPackage(string link)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var units = await _packageService.GetPackageUnitsByLinkAsync(link);

                if (!units?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<Models.Vidal.ByPackage.PackageUnit>>.Success(new List<Models.Vidal.ByPackage.PackageUnit>(), "No se encontraron unidades."));
                }

                return Ok(ResponseFromService<List<Models.Vidal.ByPackage.PackageUnit>>.Success(units, "Unidades encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        //[HttpGet("Cim10ByName")]
        //[AllowAnonymous]
        //[ServiceFilter(typeof(ValidateTokenFilter))]
        //public async Task<IActionResult> GetCim10ByName(string name)
        //{
        //    if (!IsUserAuthorized())
        //    {
        //        return Forbid("No tiene permisos para realizar esta acción.");
        //    }
        //    try
        //    {
        //        var cim10Entries = await _consultaService.GetCim10ByName(name);

        //        if (!cim10Entries?.Any() ?? true)
        //        {
        //            return Ok(ResponseFromService<List<Cim10Entry>>.Success(new List<Cim10Entry>(), "No se encontraron entradas."));
        //        }

        //        return Ok(ResponseFromService<List<Cim10Entry>>.Success(cim10Entries, "Entradas encontradas con éxito."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
        //    }
        //}



        [HttpPost("Analisis")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
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
                var htmlResponse = await _consultaService.ProcessPrescriptionRequest(request); // Se espera que esto devuelva un string

                // Devolver el HTML recibido
                return Content(htmlResponse, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Se produjo un error al procesar la solicitud: {ex.Message}"));
            }
        }

        //[HttpGet("MoleculesByName")]
        //[Authorize]
        //[ServiceFilter(typeof(ValidateTokenFilter))]
        //public async Task<IActionResult> GetMoleculesByName(string name)
        //{
        //    try
        //    {
        //        var moleculeEntries = await _moleculeService.GetMoleculesByName(name);

        //        if (!moleculeEntries?.Any() ?? true)
        //        {
        //            return Ok(ResponseFromService<List<MoleculeEntry>>.Success(new List<MoleculeEntry>(), "No se encontraron moléculas."));
        //        }

        //        return Ok(ResponseFromService<List<MoleculeEntry>>.Success(moleculeEntries, "Moléculas encontradas con éxito."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
        //    }
        //}

        [HttpPost("AsentamientoByNames")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
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
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        [HttpPost("PacienteByName")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchPacienteByName(string pacientName)
        {
            if (!IsUserAuthorized())
            {
                return Forbid("No tiene permisos para realizar esta acción.");
            }
            try
            {
                var pacientes = await _pacienteService.GetPacienteByNameAsync(pacientName);

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
