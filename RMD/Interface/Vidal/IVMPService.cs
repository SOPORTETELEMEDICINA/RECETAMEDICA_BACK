using RMD.Models.Consulta;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByVMP;
using System.Data;

namespace RMD.Interface.Vidal
{
    public interface IVMPService
    {
       
        Task<VMPModel> GetVMPById(int id);

        /******************************/
        //Task<List<VMPEntry>> GetAllVMPsAsync();
        //Task<VMPEntry> GetVMPById(int id);
        Task<List<VMPProductEntry>> GetProductsByVMPId(int vmpId);
        Task<List<VMPAtcClassificationEntry>> GetAtcClassificationByVMPId(int vmpId);
        Task<List<VMPMoleculeEntry>> GetMoleculesByVMPId(int vmpId);
        Task<List<VMPUnitEntry>> GetUnitsByVMPId(int vmpId);
        Task<List<VMPContraindicationEntry>> GetContraindicationsByVMPId(int vmpId);
        Task<List<VMPPhysicoChemicalInteractionEntry>> GetPhysicoChemicalInteractionsByVMPId(int vmpId);
        Task<List<VMPRouteEntry>> GetRoutesByVMPId(int vmpId);
        Task<List<VMPIndicatorEntry>> GetIndicatorsByVMPId(int vmpId);
        Task<List<VMPIndicationEntry>> GetIndicationsByVMPId(int vmpId);
        Task<List<VMPSideEffectEntry>> GetSideEffectsByVMPId(int vmpId);
        Task<List<VMPUcdvEntry>> GetUcdvsByVMPId(int vmpId);
        Task<List<VMPUcdEntry>> GetUcdsByVMPId(int vmpId);
        Task<List<VMPAllergyEntry>> GetAllergiesByVMPId(int vmpId);
        Task<List<VMPDocumentEntry>> GetDocumentsByVMPId(int vmpId);
        Task<List<VMPEntry>> SearchVMPsByNameAsync(string name);

        Task<List<VmpEntry>> GetVmpByName(string name);
        Task<List<VMPUnitEntry>> GetVmpUnitsByLinkAsync(string link);
    }
}
