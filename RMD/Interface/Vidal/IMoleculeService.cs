using RMD.Shared.Models.Vidal.Molecule;

namespace RMD.Interface.Vidal
{
    public interface IMoleculeService
    {
        Task<ResponseFromService<IEnumerable<RequestSearchMolecules>>> GetMoleculeByNameAsync(string name);
        List<RequestSearchMolecules> ParseMolecules(string moleculesString);
        Task<List<RequestSearchMolecules>> ObtenerMoleculesPorIdsAsync(string moleculesIds);
    }
}
