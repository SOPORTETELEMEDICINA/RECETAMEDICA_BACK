using Dapper;
using Microsoft.Data.SqlClient;
using RMD.Extensions; // Asegúrate de que ToDataTable() esté implementado
using RMD.Interface.Security;
using RMD.Interface.Usuarios;
using RMD.Shared.Models.Usuarios;
using System.Data;

namespace RMD.Service.Usuarios
{
    public class CatGrupoEmpresarialService : ICatGrupoEmpresarialService
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;

        public CatGrupoEmpresarialService(
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService)
        {
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }
        public async Task<ResponseFromService<IEnumerable<CatGrupoEmpresarial>>> GetAllGrupoEmpresarialAsync()
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync("Usuarios_GetAllGrupoEmpresarial");

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                {
                    return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Failure(notificacion);
                }
                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Success( new List<CatGrupoEmpresarial>(), notificacion);

                var grupos = multi.Read<CatGrupoEmpresarial>().ToList();

                return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Success(grupos, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<CatGrupoEmpresarial>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<CatGrupoEmpresarial>> GetGrupoEmpresarialByIdAsync(Guid id)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_GetGrupoEmpresarialById",
                    new { IdGEMP = id },
                    commandType: CommandType.StoredProcedure);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<CatGrupoEmpresarial>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<CatGrupoEmpresarial>.Success(new CatGrupoEmpresarial(), notificacion);

                var grupo = await multi.ReadFirstOrDefaultAsync<CatGrupoEmpresarial>();
                return ResponseFromService<CatGrupoEmpresarial>.Success(grupo, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatGrupoEmpresarial>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<CatGrupoEmpresarial>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> CreateGrupoEmpresarialAsync(CatGrupoEmpresarial grupoEmpresarial)
        {
            try
            {
                var parameters = new DynamicParameters();
                var table = new List<CatGrupoEmpresarial> { grupoEmpresarial }.ToDataTable();
                parameters.Add("@GrupoEmpresarial", table.AsTableValuedParameter("dbo.CatGrupoEmpresarialType"));

                using var multi = await _dapperService.QueryMultipleAsync("Usuarios_CreateGrupoEmpresarial", parameters);

                int codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                {
                    return ResponseFromService<string>.Failure(notificacion);
                }

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<string>.Success(string.Empty, notificacion);

                Guid generatedId = await multi.ReadFirstOrDefaultAsync<Guid>();
                return ResponseFromService<string>.Success($"Grupo empresarial ({generatedId}) creado exitosamente.", notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> UpdateGrupoEmpresarialAsync(CatGrupoEmpresarial grupoEmpresarial)
        {
            try
            {
                var parameters = new DynamicParameters();
                var table = new List<CatGrupoEmpresarial> { grupoEmpresarial }.ToDataTable();
                parameters.Add("@GrupoEmpresarial", table.AsTableValuedParameter("dbo.CatGrupoEmpresarialType"));

                using var multi = await _dapperService.QueryMultipleAsync("Usuarios_UpdateGrupoEmpresarial", parameters);

                int codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() == "ERROR"
                    ? ResponseFromService<string>.Failure(notificacion)
                    : ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

    }
}
