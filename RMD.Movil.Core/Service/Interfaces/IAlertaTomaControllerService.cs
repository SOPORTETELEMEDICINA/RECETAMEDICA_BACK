using RMD.Shared.Models.Consulta;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.AlertaToma.Request;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IAlertaTomaControllerService
    {
        Task<ResponseFromService<bool>> ActivarAlertaTomaAsync(ActivarAlertaTomaRequest request);
        Task<ResponseFromService<bool>> DesactivarAlertaTomaAsync(DesactivarAlertaTomaRequest request);
        Task<ResponseFromService<bool>> ActivarAlertaManualAsync(ActivarAlertaManualRequest request);
        Task<ResponseFromService<bool>> DesactivarAlertaManualAsync(DesactivarAlertaManualRequest request);
        Task<ResponseFromService<List<Medicamentos>>> BuscarPackagePorNombreAsync(BuscarPackageRequest request);
    }
}
