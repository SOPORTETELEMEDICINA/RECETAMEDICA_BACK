using RMD.Models.Vidal.ByRoute;

namespace RMD.Interface.Vidal
{
    public interface IRouteService
    {
        Task<Routes> GetRouteByIdAsync(int id);
        Task<List<Routes>> GetAllRoutesAsync();
    }
}
