using RMD.Models.Vidal;

namespace RMD.Interface.Vidal
{
    public interface IForeignProductService
    {
        Task<List<ForeignProductEquivalent>> GetEquivalentProductsByForeignProductId(int foreignProductId);
    }
}
