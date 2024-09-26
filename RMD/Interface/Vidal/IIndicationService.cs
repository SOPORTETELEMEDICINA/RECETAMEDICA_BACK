using RMD.Models.Vidal.ByIndication;

namespace RMD.Interface.Vidal
{
    public interface IIndicationService
    {
        Task<IndicationDetail> GetIndicationByIdAsync(int indicationId);
        Task<List<IndicationProduct>> GetProductsByIndicationIdAsync(int indicationId);
        Task<List<IndicationVMP>> GetVmpsByIndicationIdAsync(int indicationId);
    }
}
