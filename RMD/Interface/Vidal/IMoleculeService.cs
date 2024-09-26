using RMD.Models.Consulta;
using RMD.Models.Vidal.ByMolecule;

namespace RMD.Interface.Vidal
{
    public interface IMoleculeService
    {
        Task<List<Molecule>> GetAllMoleculesAsync();
        Task<Molecule> GetMoleculeById(int id);
        Task<List<MoleculeEntry>> GetMoleculesByName(string name);
    }
}
