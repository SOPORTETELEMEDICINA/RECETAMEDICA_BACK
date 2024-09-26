using RMD.Models.Responses;
using RMD.Models.Vidal;
using RMD.Models.Vidal.ByProduct;

namespace RMD.Interface.Vidal
{
    public interface IProductService
    {
        
        Task<ProductModel> GetProductByIdAsync(int productId);
        //Task<List<Product>> GetAllProductsAsync();
        //Task<ProductById> GetProductByIdAsync(int productId);
        Task<List<ProductPackage>> GetProductPackagesAsync(int productId);
        Task<List<ProductMolecule>> GetProductMoleculesAsync(int productId);
        Task<List<ProductForeign>> GetProductForeignAsync(int productId);
        Task<List<ProductIndication>> GetProductIndicationsAsync(int productId);
        Task<List<ProductUcd>> GetProductUcdsAsync(int productId);
        Task<List<ProductUnit>> GetProductUnitsAsync(int productId);
        Task<List<ProductRoute>> GetProductRoutesAsync(int productId);
        Task<ProductIndicator> GetProductIndicatorsAsync(int productId);
        Task<ProductSideEffect> GetProductSideEffectsAsync(int productId);
        Task<ProductUCDV> GetProductUCDVAsync(int productId);
        Task<ProductAllergy> GetProductAllergyAsync(int productId);
        Task<ProductAtcClassification> GetProductAtcClassificationAsync(int productId);
        Task<ProductVMPGroup> GetVmpByProductGroupAsync(int productGroupId);
        Task<List<Product>> GetProductsByNameAsync(string productName);
        Task<List<ProductEntry>> GetProductsByName(string name);

        Task<List<ProductUnit>> GetProductUnitsByLinkAsync(string link);
    }
}
