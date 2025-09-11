using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Security;
using RMD.Interface.Sucursales;
using RMD.Shared.Models.Sucursales;

namespace RMD.Service.Sucursales
{
    public class SucursalService : ISucursalService
    {
        private readonly CatalogoDbContext _catalogo;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;

        public SucursalService(CatalogoDbContext catalogo, 
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService)
        {
            _catalogoNotificacionService = catalogoNotificacionService;
            _catalogo = catalogo;
            _dapperService = dapperService;
        }
        public async Task<ResponseFromService<bool>> CreateSucursalAsync(CreateSucursalModel model)
        {
            try
            {
                var asentamientoExiste = await _catalogo.CatAsentamientos
                    .AnyAsync(a => a.IdAsentamiento == model.IdAsentamiento);

                if (!asentamientoExiste)
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_NO_ENCONTRADO");

                    return ResponseFromService<bool>.Failure(notif);
                }

                // Ejecuta el SP con Dapper y obtiene el código de notificación
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Sucursales_Create",
                    new
                    {
                        SucursalData = new List<CreateSucursalModel> { model }.ToDataTable("dbo.SucursalCreateTableType")
                    }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");

                return ResponseFromService<bool>.Exeption(ex, notif);
            }
        }

        public async Task<ResponseFromService<bool>> UpdateSucursalAsync(Guid idSucursal, UpdateSucursalModel model)
        {
            try
            {
                var asentamientoExiste = await _catalogo.CatAsentamientos
                    .AnyAsync(a => a.IdAsentamiento == model.IdAsentamiento);

                if (!asentamientoExiste)
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_NO_ENCONTRADO");
                    return ResponseFromService<bool>.Failure(notif);
                }

                // Ejecutar el SP con Dapper
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Sucursales_Update",
                    new
                    {
                        IdSucursal = idSucursal,
                        SucursalData = new List<UpdateSucursalModel> { model }
                            .ToDataTable("dbo.SucursalUpdateTableType")
                    }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }
        public async Task<ResponseFromService<bool>> DeleteSucursalAsync(Guid idSucursal)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Sucursales_Delete",
                    new { IdSucursal = idSucursal }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<SucursalRequest>> GetSucursalByIdSucursalAsync(Guid idSucursal)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Sucursales_GetSucursalByIdSucursal",
                    new { IdSucursal = idSucursal }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<SucursalRequest>.Failure(notificacion);

                var sucursal = multi.Read<SucursalRequest>().FirstOrDefault();

                return ResponseFromService<SucursalRequest>.Success(sucursal, notificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<SucursalRequest>.Exeption(ex, errorNotificacion);
            }
        }
        public async Task<ResponseFromService<IEnumerable<SucursalRequest>>> GetSucursalesByIdGEMPAsync(Guid idGEMP)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Sucursales_GetSucursalByIdGEMP",
                    new { IdGEMP = idGEMP }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<SucursalRequest>>.Failure(notificacion);

                var sucursales = multi.Read<SucursalRequest>().ToList();

                return ResponseFromService<IEnumerable<SucursalRequest>>.Success(sucursales, notificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<SucursalRequest>>.Exeption(ex, errorNotificacion);
            }
        }
    }
}
