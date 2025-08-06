using RMD.Shared.Models.Consulta;
using RMD.Shared.Models.Receta.AlertaToma.Request;

namespace RMD.Interface.Receta
{
    public interface IAlertaTomaService
    {
        Task<ResponseFromService<bool>> ActivarAlertaTomaAsync(ActivarAlertaTomaRequest request, Guid idUsuario);
        Task<ResponseFromService<bool>> DesactivarAlertaTomaAsync(DesactivarAlertaTomaRequest request, Guid idUsuario);
        Task<ResponseFromService<List<Medicamentos>>> BuscarPackagePorNombreAsync(string nombrePackage, Guid idUsuario);
        Task<ResponseFromService<bool>> ActivarAlertaManualAsync(ActivarAlertaManualRequest request, Guid idUsuario);

        Task<ResponseFromService<bool>> DesactivarAlertaManualAsync(DesactivarAlertaManualRequest request,
            Guid idUsuario);

    }
}
