using RMD.Models.Vidal.ByForeignProduct;

namespace RMD.Interface.Vidal
{
    public interface IForeignProductService
    {
        Task<List<ForeignProductEquivalent>> GetEquivalentProductsByForeignProductId(int foreignProductId);
    }
}
