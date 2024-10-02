using RMD.Models.Vidal;

namespace RMD.Interface.Vidal
{
    public interface ICIM10Service
    {
        Task<List<CIM10>> GetAllCIM10sAsync();
        Task<CIM10> GetCIM10ByIdAsync(int id);
        Task<List<CIM10Child>> GetCIM10ChildrenAsync(int parentId);
    }
}
