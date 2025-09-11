using Microsoft.Data.SqlClient;
using RMD.Interface.Security;
using RMD.Interface.Vidal;
using RMD.Shared.Models.Vidal.Allergy;

namespace RMD.Service.Vidal.Allergy
{
    public class AllergyService : IAllergyService
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;
        public AllergyService(     
            IDapperService dapperService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }

        public async Task<ResponseFromService<IEnumerable<RequestSearchAllergy>>> GetAllergiesByNameAsync(string name)
        {
            try
            {
                var parameters = new { name };

                using var multi = await _dapperService.QueryMultipleAsync("[Vidal].[GetAllergyByName]", parameters);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<RequestSearchAllergy>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<RequestSearchAllergy>>.Success(new List<RequestSearchAllergy>(), notificacion);

                var lista = multi.Read<RequestSearchAllergy>().ToList();

                return ResponseFromService<IEnumerable<RequestSearchAllergy>>.Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestSearchAllergy>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestSearchAllergy>>.Exeption(ex, error);
            }
        }
        public async Task<List<RequestSearchAllergy>> ObtenerAlergiasPorIdsAsync(string alergiasIds)
        {
            try
            {
                if (string.IsNullOrEmpty(alergiasIds))
                    return new List<RequestSearchAllergy>();

                var result = await _dapperService.QueryAsync<RequestSearchAllergy>(
                    "[Vidal].[GetAllergiesByIds]",
                    new { Ids = alergiasIds }
                );

                return result.ToList();
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }
        public List<RequestSearchAllergy> ParseAllergies(string allergiesString)
        {
            try
            {
                return allergiesString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a =>
                    {
                        var parts = a.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                            return null;
                        return new RequestSearchAllergy
                        {
                            IdAllergy = int.Parse(parts[0].Trim()),
                            NameAllergy = parts[1].Trim()
                        };
                    })
                    .OfType<RequestSearchAllergy>() // ← filtra los null automáticamente sin warnings
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing allergies: {ex}");
                return new List<RequestSearchAllergy>();
            }
        }
       
    }
}
