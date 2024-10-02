using RMD.Models.Consulta;

namespace RMD.Interface.Consulta
{
    public interface IConsultaService
    {
        Task<IEnumerable<RequestSearchAllergy>> GetAllergiesByNameAsync(string name);
        Task<IEnumerable<RequestSearchMolecules>> GetMoleculeByNameAsync(string name);
        Task<IEnumerable<RequestSearchVMP>> GetVMPByNameAsync(string name);
        Task<IEnumerable<RequestSearchProducts>> GetProductsByNameAsync(string name);
        Task<IEnumerable<RequestSearchPackage>> GetPackagesByNameAsync(string name);
        Task<IEnumerable<RequestSearchCIM10>> GetCIM10sByNameAsync(string name);


        Task<List<Cim10Entry>> GetCim10ByName(string name);
        Task<string> ProcessPrescriptionRequest(PrescriptionModel request);
    }
}
