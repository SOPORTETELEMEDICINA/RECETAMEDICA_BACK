using RMD.Models.Consulta;

namespace RMD.Interface.Consulta
{
    public interface IConsultaService
    {
        
        Task<List<Cim10Entry>> GetCim10ByName(string name);
        Task<List<AllergyEntry>> GetAllergiesByName(string name);
        Task<string> ProcessPrescriptionRequest(PrescriptionModel request);
    }
}
