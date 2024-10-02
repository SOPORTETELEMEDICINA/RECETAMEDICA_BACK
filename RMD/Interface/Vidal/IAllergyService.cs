using RMD.Models.Vidal;

namespace RMD.Interface.Vidal
{
    public interface IAllergyService
    {
        Task<List<Allergy>> GetAllergiesByName(string name);
        Task<List<Allergy>> GetAllAllergiesAsync();
        Task<Allergy> GetAllergyByIdAsync(int allergyId);
        Task<List<AllergyMolecule>> GetMoleculesByAllergyIdAsync(int allergyId);
    }

}
