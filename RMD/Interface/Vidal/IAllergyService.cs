using RMD.Shared.Models.Vidal.Allergy;

namespace RMD.Interface.Vidal
{
    public interface IAllergyService
    {
        Task<ResponseFromService<IEnumerable<RequestSearchAllergy>>> GetAllergiesByNameAsync(string name);
        List<RequestSearchAllergy> ParseAllergies(string allergiesString);
        Task<List<RequestSearchAllergy>> ObtenerAlergiasPorIdsAsync(string alergiasIds);
    }
}
