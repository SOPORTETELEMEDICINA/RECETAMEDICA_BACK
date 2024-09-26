using RMD.Models.Vidal.ByUnit;

namespace RMD.Interface.Vidal
{
    public interface IUnitService
    {
        Task<List<Units>> GetAllUnitsAsync();
        Task<Unit> GetUnitById(int id);
    }
}
