using RMD.Interface.Security;
using RMD.Interface.Vidal;
using RMD.Shared.Models.Vidal.CIM10;
namespace RMD.Service.Vidal.CIM10
{
    public class CIM10Service : ICIM10Service
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;
        public CIM10Service(         
            IDapperService dapperService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {           
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }
        public async Task<ResponseFromService<IEnumerable<RequestSearchCIM10>>> GetCIM10sByNameAsync(PatologiaRequest request)
        {
            try
            {
                var parameters = new
                {
                    name = request.Nombre,
                    edad = request.Edad,
                    genero = request.Genero
                };

                using var multi = await _dapperService.QueryMultipleAsync("[Vidal].[GetCIM10ByName]", parameters);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<RequestSearchCIM10>>.Failure(notificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<RequestSearchCIM10>>.Success(new List<RequestSearchCIM10>(), notificacion);

                var lista = multi.Read<RequestSearchCIM10>().ToList();
                return ResponseFromService<IEnumerable<RequestSearchCIM10>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestSearchCIM10>>.Exeption(ex, error);
            }
        }
        public async Task<List<RequestSearchCIM10>> ObtenerCIM10PorIdsAsync(string cim10Ids)
        {
            try
            {
                if (string.IsNullOrEmpty(cim10Ids))
                    return new List<RequestSearchCIM10>();

                var result = await _dapperService.QueryAsync<RequestSearchCIM10>(
                    "[Vidal].[GetCIM10ByIds]",
                    new { Ids = cim10Ids }
                );

                return result.ToList();
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }

        public List<RequestSearchCIM10> ParseCIM10(string cim10String)
        {
            try
            {
                return cim10String.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c =>
                    {
                        var parts = c.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 3)
                            return null; // <-- aquí está el warning si el tipo no es nullable
                        return new RequestSearchCIM10
                        {
                            IdCIM10 = int.Parse(parts[0].Trim()),
                            NameCIM10 = parts[1].Trim(),
                            Code = parts[2].Trim()
                        };
                    })
                    .OfType<RequestSearchCIM10>() // ← Esto filtra automáticamente los nulls sin warning
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing CIM10: {ex}");
                return new List<RequestSearchCIM10>();
            }
        }
        public async Task<bool> AskPSumistroByIdProduct(int idProduct)
        {
            try
            {
                var result = await _dapperService.QueryFirstOrDefaultAsync<bool>(
                    "[Vidal].[AskPSumistroByIdProduct]",
                    new { IdProduct = idProduct }
                );

                return result;
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }
        public async Task<string?> GetPSumistroByIdProduct(int idProduct)
        {
            try
            {
                var result = await _dapperService.QueryFirstOrDefaultAsync<string>(
                    "[Vidal].[GetPSumistroByIdProduct]",
                    new { IdProduct = idProduct }
                );

                return result;
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }


    }
}
