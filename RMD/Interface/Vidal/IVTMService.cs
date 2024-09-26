using RMD.Models.Vidal.ByVTM;

namespace RMD.Interface.Vidal
{
    public interface IVTMService
    {
        Task<List<VTMMolecule>> GetMoleculesByVtmIdAsync(int vtmId);
        Task<List<VTMS>> GetVtmsAsync();
        Task<VTMEntry> GetVTMById(int vtmId);
    }
}
