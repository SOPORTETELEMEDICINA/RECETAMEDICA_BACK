using Dapper;
using RMD.Extensions;
using RMD.Interface.Security;
using RMD.Interface.Usuarios;
using RMD.Shared.Models.Usuarios;

namespace RMD.Service.Usuarios
{
    public class TipoUsuarioService : ITipoUsuarioService
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;

        public TipoUsuarioService(
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService)
        {
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }

        public async Task<ResponseFromService<IEnumerable<TipoUsuario>>> GetAllTipoUsuarioAsync()
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync("sp_Cat_GetAllTipoUsuario");

                int codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<TipoUsuario>>.Failure(notificacion);

                var tipos = (await multi.ReadAsync<TipoUsuario>()).ToList();
                return ResponseFromService<IEnumerable<TipoUsuario>>.Success(tipos, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<TipoUsuario>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<TipoUsuario>> GetTipoUsuarioByIdAsync(Guid id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@IdTipoUsuario", id);

                using var multi = await _dapperService.QueryMultipleAsync("sp_Cat_GetTipoUsuarioById", parameters);

                int codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<TipoUsuario>.Failure(notificacion);

                var tipo = await multi.ReadFirstOrDefaultAsync<TipoUsuario>();
                return ResponseFromService<TipoUsuario>.Success(tipo, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<TipoUsuario>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> CreateTipoUsuarioAsync(TipoUsuario tipoUsuario)
        {
            try
            {
                var table = new List<TipoUsuario> { tipoUsuario }.ToDataTable();
                var parameters = new DynamicParameters();
                parameters.Add("@TipoUsuario", table.AsTableValuedParameter("dbo.TipoUsuarioType"));

                using var multi = await _dapperService.QueryMultipleAsync("sp_Cat_CreateTipoUsuario", parameters);

                int codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() == "ERROR"
                    ? ResponseFromService<string>.Failure(notificacion)
                    : ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> UpdateTipoUsuarioAsync(TipoUsuario tipoUsuario)
        {
            try
            {
                var table = new List<TipoUsuario> { tipoUsuario }.ToDataTable();
                var parameters = new DynamicParameters();
                parameters.Add("@TipoUsuario", table.AsTableValuedParameter("dbo.TipoUsuarioType"));

                using var multi = await _dapperService.QueryMultipleAsync("sp_Cat_UpdateTipoUsuario", parameters);

                int codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() == "ERROR"
                    ? ResponseFromService<string>.Failure(notificacion)
                    : ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
    }
}
