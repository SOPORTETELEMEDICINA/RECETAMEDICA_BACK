using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByVMP;
using System.Net;

namespace RMD.Controllers.Vidal.ByVMP
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class VMPController : ControllerBase
    {
        private readonly IVMPService _vmpService;

        public VMPController(IVMPService vmpService)
        {
            _vmpService = vmpService;
        }

       

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVMPById(int id)
        {
            try
            {
                var vmpEntry = await _vmpService.GetVMPById(id);

                if (vmpEntry == null)
                {
                    // Retornar modelo vacío y mensaje de "no encontrado"
                    var emptyModel = new VMPModel();  // Modelo vacío
                    return Ok(ResponseFromService<VMPModel>.Failure(HttpStatusCode.NotFound, "VMP not found."));
                }

                // Retornar el modelo encontrado con el mensaje de éxito
                return Ok(ResponseFromService<VMPEntry>.Success(vmpEntry, "VMP retrieved successfully."));
            }
            catch (Exception ex)
            {
                // En caso de error, retornar un error de servidor
                return StatusCode(500, ResponseFromService<VMPModel>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }


        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetVMPById(int id)
        //{
        //    try
        //    {
        //        var vmpEntry = await _vmpService.GetVMPById(id);
        //        if (vmpEntry == null)
        //        {
        //            return Ok(ResponseFromService<VMPEntry>.Failure(HttpStatusCode.NotFound, "VMP not found."));
        //        }

        //        return Ok(ResponseFromService<VMPEntry>.Success(vmpEntry, "VMP retrieved successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ResponseFromService<VMPEntry>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
        //    }
        //}

        [HttpGet("{vmpId}/products")]
        public async Task<IActionResult> GetProductsByVMPId(int vmpId)
        {
            try
            {
                var products = await _vmpService.GetProductsByVMPId(vmpId);
                if (products == null || products.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPProductEntry>>.Failure(HttpStatusCode.NotFound, "No products found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPProductEntry>>.Success(products, "Products retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPProductEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }


        [HttpGet("{vmpId}/atc-classification")]
        public async Task<IActionResult> GetAtcClassificationByVMPId(int vmpId)
        {
            try
            {
                var classifications = await _vmpService.GetAtcClassificationByVMPId(vmpId);
                if (classifications == null || classifications.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPAtcClassificationEntry>>.Failure(HttpStatusCode.NotFound, "No ATC classifications found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPAtcClassificationEntry>>.Success(classifications, "ATC classifications retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPAtcClassificationEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{vmpId}/molecules")]
        public async Task<IActionResult> GetMoleculesByVMPId(int vmpId)
        {
            try
            {
                var molecules = await _vmpService.GetMoleculesByVMPId(vmpId);
                if (molecules == null || molecules.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPMoleculeEntry>>.Failure(HttpStatusCode.NotFound, "No molecules found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPMoleculeEntry>>.Success(molecules, "Molecules retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPMoleculeEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{vmpId}/units")]
        public async Task<IActionResult> GetUnitsByVMPId(int vmpId)
        {
            try
            {
                var units = await _vmpService.GetUnitsByVMPId(vmpId);
                if (units == null || units.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPUnitEntry>>.Failure(HttpStatusCode.NotFound, "No units found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPUnitEntry>>.Success(units, "Units retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPUnitEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{vmpId}/contraindications")]
        public async Task<IActionResult> GetContraindicationsByVMPId(int vmpId)
        {
            try
            {
                var contraindications = await _vmpService.GetContraindicationsByVMPId(vmpId);
                if (contraindications == null || contraindications.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPContraindicationEntry>>.Failure(HttpStatusCode.NotFound, "No contraindications found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPContraindicationEntry>>.Success(contraindications, "Contraindications retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPContraindicationEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }


        [HttpGet("vmp/{vmpId}/physico-chemical-interactions")]
        public async Task<IActionResult> GetPhysicoChemicalInteractionsByVMPId(int vmpId)
        {
            try
            {
                var interactions = await _vmpService.GetPhysicoChemicalInteractionsByVMPId(vmpId);
                if (interactions == null || interactions.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPPhysicoChemicalInteractionEntry>>.Failure(HttpStatusCode.NotFound, "No interactions found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPPhysicoChemicalInteractionEntry>>.Success(interactions, "Interactions retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPPhysicoChemicalInteractionEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vmp/{vmpId}/routes")]
        public async Task<IActionResult> GetRoutesByVMPId(int vmpId)
        {
            try
            {
                var routes = await _vmpService.GetRoutesByVMPId(vmpId);
                if (routes == null || routes.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPRouteEntry>>.Failure(HttpStatusCode.NotFound, "No routes found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPRouteEntry>>.Success(routes, "Routes retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPRouteEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vmp/{vmpId}/indicators")]
        public async Task<IActionResult> GetIndicatorsByVMPId(int vmpId)
        {
            try
            {
                var indicators = await _vmpService.GetIndicatorsByVMPId(vmpId);
                if (indicators == null || indicators.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPIndicatorEntry>>.Failure(HttpStatusCode.NotFound, "No indicators found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPIndicatorEntry>>.Success(indicators, "Indicators retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPIndicatorEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vmp/{vmpId}/indications")]
        public async Task<IActionResult> GetIndicationsByVMPId(int vmpId)
        {
            try
            {
                var indications = await _vmpService.GetIndicationsByVMPId(vmpId);
                if (indications == null || indications.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPIndicationEntry>>.Failure(HttpStatusCode.NotFound, "No indications found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPIndicationEntry>>.Success(indications, "Indications retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPIndicationEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vmp/{vmpId}/side-effects")]
        public async Task<IActionResult> GetSideEffectsByVMPId(int vmpId)
        {
            try
            {
                var sideEffects = await _vmpService.GetSideEffectsByVMPId(vmpId);
                if (sideEffects == null || sideEffects.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPSideEffectEntry>>.Failure(HttpStatusCode.NotFound, "No side effects found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPSideEffectEntry>>.Success(sideEffects, "Side effects retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPSideEffectEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vmp/{vmpId}/ucdvs")]
        public async Task<IActionResult> GetUcdvsByVMPId(int vmpId)
        {
            try
            {
                var ucdvs = await _vmpService.GetUcdvsByVMPId(vmpId);
                if (ucdvs == null || ucdvs.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPUcdvEntry>>.Failure(HttpStatusCode.NotFound, "No UCDVs found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPUcdvEntry>>.Success(ucdvs, "UCDVs retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPUcdvEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vmp/{vmpId}/ucds")]
        public async Task<IActionResult> GetUcdsByVMPId(int vmpId)
        {
            try
            {
                var ucds = await _vmpService.GetUcdsByVMPId(vmpId);
                if (ucds == null || ucds.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPUcdEntry>>.Failure(HttpStatusCode.NotFound, "No UCDs found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPUcdEntry>>.Success(ucds, "UCDs retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPUcdEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vmp/{vmpId}/allergies")]
        public async Task<IActionResult> GetAllergiesByVMPId(int vmpId)
        {
            try
            {
                var allergies = await _vmpService.GetAllergiesByVMPId(vmpId);
                if (allergies == null || allergies.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPAllergyEntry>>.Failure(HttpStatusCode.NotFound, "No allergies found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPAllergyEntry>>.Success(allergies, "Allergies retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPAllergyEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("vmp/{vmpId}/documents")]
        public async Task<IActionResult> GetDocumentsByVMPId(int vmpId)
        {
            try
            {
                var documents = await _vmpService.GetDocumentsByVMPId(vmpId);
                if (documents == null || documents.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPDocumentEntry>>.Failure(HttpStatusCode.NotFound, "No documents found for this VMP."));
                }

                return Ok(ResponseFromService<List<VMPDocumentEntry>>.Success(documents, "Documents retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPDocumentEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchVMPsByName(string name)
        {
            try
            {
                var vmpEntries = await _vmpService.SearchVMPsByNameAsync(name);

                if (vmpEntries == null || vmpEntries.Count == 0)
                {
                    return Ok(ResponseFromService<List<VMPEntry>>.Failure(HttpStatusCode.NotFound, "No VMPs found with the specified name."));
                }

                return Ok(ResponseFromService<List<VMPEntry>>.Success(vmpEntries, "VMPs retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<List<VMPEntry>>.Failure(HttpStatusCode.InternalServerError, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
