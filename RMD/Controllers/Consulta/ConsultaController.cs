using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Catalogos;
using RMD.Interface.Consulta;
using RMD.Interface.Pacientes;
using RMD.Interface.Vidal;
using RMD.Models.Consulta;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByUnit;
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
        IVMPService vmpService
            ) : ControllerBase
    {
        private readonly IConsultaService _consultaService = consultaService;
        private readonly ICatalogoService _catalogoService = catalogoService;
        private readonly IPacienteService _pacienteService = pacienteService;
        private readonly IPackageService _packageService = packageService;
        private readonly IProductService _productService = productService;
        private readonly IMoleculeService _moleculeService = moleculeService;
        private readonly IVMPService _vmpService = vmpService;

        [HttpGet("VmpByName")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetVmpByName(string name)
        {
            try
            {
                var vmpEntries = await _vmpService.GetVmpByName(name);

                if (!vmpEntries?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<VmpEntry>>.Success(new List<VmpEntry>(), "No se encontraron entradas."));
                }

                return Ok(ResponseFromService<List<VmpEntry>>.Success(vmpEntries, "Entradas encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        [HttpGet("VmpUnit")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchUnitvmp(string link)
        {
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

        [HttpGet("ProductByName")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetProductsByName(string name)
        {
            try
            {
                var productEntries = await _productService.GetProductsByName(name);

                if (!productEntries?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<Models.Vidal.ByProduct.ProductEntry>>.Success(new List<Models.Vidal.ByProduct.ProductEntry>(), "No se encontraron productos."));
                }

                return Ok(ResponseFromService<List<Models.Vidal.ByProduct.ProductEntry>>.Success(productEntries, "Productos encontrados con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        [HttpGet("ProductUnit")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchUnitProduct(string link)
        {
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

        [HttpGet("PackeByName")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<ActionResult<List<PackageEntry>>> GetPackagesByName([FromQuery] string name)
        {
            try
            {
                var packages = await _packageService.GetPackagesByName(name);
                if (!packages?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<PackageEntry>>.Success(new List<PackageEntry>(), "No se encontraron paquetes."));
                }

                return Ok(ResponseFromService<List<PackageEntry>>.Success(packages, "Paquetes encontrados con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        [HttpGet("PackageUnit")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchUnitPackage(string link)
        {
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

        [HttpGet("Cim10ByName")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetCim10ByName(string name)
        {
            try
            {
                var cim10Entries = await _consultaService.GetCim10ByName(name);

                if (!cim10Entries?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<Cim10Entry>>.Success(new List<Cim10Entry>(), "No se encontraron entradas."));
                }

                return Ok(ResponseFromService<List<Cim10Entry>>.Success(cim10Entries, "Entradas encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        [HttpGet("AllergysByName")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetAllergiesByName(string name)
        {
            try
            {
                var allergyEntries = await _consultaService.GetAllergiesByName(name);

                if (!allergyEntries?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<AllergyEntry>>.Success(new List<AllergyEntry>(), "No se encontraron alergias."));
                }

                return Ok(ResponseFromService<List<AllergyEntry>>.Success(allergyEntries, "Alergias encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        [HttpPost("Analisis")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> AnalyzePrescription([FromBody] PrescriptionModel request)
        {
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




        [HttpGet("MoleculesByName")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> GetMoleculesByName(string name)
        {
            try
            {
                var moleculeEntries = await _moleculeService.GetMoleculesByName(name);

                if (!moleculeEntries?.Any() ?? true)
                {
                    return Ok(ResponseFromService<List<MoleculeEntry>>.Success(new List<MoleculeEntry>(), "No se encontraron moléculas."));
                }

                return Ok(ResponseFromService<List<MoleculeEntry>>.Success(moleculeEntries, "Moléculas encontradas con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Se produjo un error en el servidor."));
            }
        }

        [HttpGet("AsentamientoByNames")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchAsentamiento([FromQuery] AsentamientoSearchModel searchModel)
        {
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

        [HttpGet("PacienteByName")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> SearchPacienteByName(string pacientName)
        {
            try
            {
                var pacientes = await _pacienteService.GetPacienteByNameAsync(pacientName);

                if (!pacientes?.Any() ?? true)
                {
                    return Ok(ResponseFromService<IEnumerable<Models.Pacientes.UsuarioPaciente>>.Success(new List<Models.Pacientes.UsuarioPaciente>(), "No se encontraron pacientes."));
                }

                return Ok(ResponseFromService<IEnumerable<Models.Pacientes.UsuarioPaciente>>.Success(pacientes, "Pacientes encontrados con éxito."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Se produjo un error en el servidor: {ex.Message}"));
            }
        }
    }
}
