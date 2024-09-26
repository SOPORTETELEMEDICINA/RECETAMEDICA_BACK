using RMD.Models.Vidal.ByATC;

namespace RMD.Interface.Vidal
{
    public interface IATCService
    {
        Task<List<ATCClassification>> GetAllATCClassificationsAsync();
        Task<List<ATCVMPEntry>> GetVmpsByAtcClassificationAsync(int atcId);
        Task<List<AtcProduct>> GetProductsByAtcClassificationAsync(int atcId);
        Task<List<ATCClassificationEntry>> GetAtcChildrenAsync(int atcId);
        Task<ATCClassificationDetail>GetAtcClassificationByIdAsync(int atcId);
    }
}
