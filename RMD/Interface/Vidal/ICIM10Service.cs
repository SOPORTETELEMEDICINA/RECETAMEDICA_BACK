using RMD.Shared.Models.Vidal.CIM10;

namespace RMD.Interface.Vidal
{
    public interface ICIM10Service
    {
        Task<ResponseFromService<IEnumerable<RequestSearchCIM10>>> GetCIM10sByNameAsync(PatologiaRequest request);
        List<RequestSearchCIM10> ParseCIM10(string cim10String);
        Task<List<RequestSearchCIM10>> ObtenerCIM10PorIdsAsync(string cim10Ids);
        Task<string?> GetPSumistroByIdProduct(int idProduct);
        Task<bool> AskPSumistroByIdProduct(int idProduct);
    }
}
