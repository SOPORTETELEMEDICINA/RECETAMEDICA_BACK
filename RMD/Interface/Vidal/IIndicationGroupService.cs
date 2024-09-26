using RMD.Models.Vidal.ByIndicationGroup;

namespace RMD.Interface.Vidal
{
    public interface IIndicationGroupService
    {
        Task<IndicationGroup> GetIndicationGroupByIdAsync(int indicationGroupId);
        Task<List<IndicationGroupProduct>> GetProductsByIndicationGroupIdAsync(int indicationGroupId);
        Task<List<CIM10>> GetCIM10EntriesAsync(int indicationGroupId);
        Task<List<VMP>> GetVMPsByIndicationGroupIdAsync(int indicationGroupId);
        Task<List<Indication>> GetIndicationsByIndicationGroupIdAsync(int indicationGroupId);

    }
}
