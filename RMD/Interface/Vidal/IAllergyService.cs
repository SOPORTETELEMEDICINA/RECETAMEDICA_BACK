using RMD.Models.Vidal.ByAllergy;

namespace RMD.Interface.Vidal
{
    public interface IAllergyService
    {
        Task<List<Allergy>> GetAllAllergiesAsync();
        Task<AllergyEntry> GetAllergyByIdAsync(int allergyId);
        Task<List<AllergyMolecule>> GetMoleculesByAllergyIdAsync(int allergyId);
    }

}
