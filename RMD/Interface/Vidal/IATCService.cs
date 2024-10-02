using RMD.Models.Vidal;

namespace RMD.Interface.Vidal
{
    public interface IATCService
    {
        Task<List<ATCDetail>> GetVmpsByAtcClassificationAsync(int atcId);
        Task<List<ATCDetail>> GetProductsByAtcClassificationAsync(int atcId);
        Task<List<ATCClassification>> GetAtcChildrenAsync(int atcId);
        Task<ATCDetail> GetAtcClassificationByIdAsync(int atcId);
        Task<List<ATCClassification>> GetAllATCClassificationsAsync();
    }
}
