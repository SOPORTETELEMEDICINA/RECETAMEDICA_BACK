using RMD.Interface.Receta;
using RMD.Interface.Security;
using RMD.Shared.Models.Receta.Catalogos;
using System.Data;

namespace RMD.Service.Receta
{
    public class RecetaCatalogosService : IRecetaCatalogosService
    {
        private readonly IDapperService _dapperService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public RecetaCatalogosService(
            IDapperService dapperService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _dapperService = dapperService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        public async Task<ResponseFromService<List<FrecuencyTypeListModel>>> ObtenerFrecuencyTypesAsync()
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[GetFrecuencyTypes]",
                    commandType: CommandType.StoredProcedure);

                var codigo = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigo);
                var result = (await multi.ReadAsync<FrecuencyTypeListModel>()).ToList();

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<List<FrecuencyTypeListModel>>.Success(result, notificacion )
                    : ResponseFromService<List<FrecuencyTypeListModel>>.Failure(notificacion);
            }
            catch
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXCEPCION");
                return ResponseFromService<List<FrecuencyTypeListModel>>.Failure(notif);
            }
        }

        public async Task<ResponseFromService<FrecuencyTypeListModel>> ObtenerFrecuencyTypePorIdAsync(int id)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[GetFrecuencyTypeById]",
                    new { IdFrecuencyType = id },
                    commandType: CommandType.StoredProcedure);

                var codigo = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigo);
                var result = await multi.ReadFirstOrDefaultAsync<FrecuencyTypeListModel>();

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<FrecuencyTypeListModel>.Success(result,notificacion)
                    : ResponseFromService<FrecuencyTypeListModel>.Failure(notificacion);
            }
            catch
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXCEPCION");
                return ResponseFromService<FrecuencyTypeListModel>.Failure(notif);
            }
        }
    }
}
