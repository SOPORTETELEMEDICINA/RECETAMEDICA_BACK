using RMD.Interface.Security;
using RMD.Interface.Vidal;
using RMD.Shared.Models.Vidal.Molecule;

namespace RMD.Service.Vidal.Molecule
{
    public class MoleculeService:IMoleculeService
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;
        public MoleculeService(IDapperService dapperService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }
        public async Task<ResponseFromService<IEnumerable<RequestSearchMolecules>>> GetMoleculeByNameAsync(string name)
        {
            try
            {
                var parameters = new { name };

                using var multi = await _dapperService.QueryMultipleAsync("[VIDAL].[GetMoleculesByName]", parameters);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<RequestSearchMolecules>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<RequestSearchMolecules>>.Success(new List<RequestSearchMolecules>(), notificacion);

                var lista = multi.Read<RequestSearchMolecules>().ToList();
                return ResponseFromService<IEnumerable<RequestSearchMolecules>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestSearchMolecules>>.Exeption(ex, error);
            }
        }
        public async Task<List<RequestSearchMolecules>> ObtenerMoleculesPorIdsAsync(string moleculesIds)
        {
            try
            {
                if (string.IsNullOrEmpty(moleculesIds))
                    return new List<RequestSearchMolecules>();

                var result = await _dapperService.QueryAsync<RequestSearchMolecules>(
                    "[Vidal].[GetMoleculesByIds]",
                    new { Ids = moleculesIds }
                );

                return result.ToList();
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }
        public List<RequestSearchMolecules> ParseMolecules(string moleculesString)
        {
            try
            {
                return moleculesString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(m =>
                    {
                        var parts = m.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                            return null;
                        return new RequestSearchMolecules
                        {
                            IdMolecule = int.Parse(parts[0].Trim()),
                            NameMolecule = parts[1].Trim()
                        };
                    })
                    .OfType<RequestSearchMolecules>() // filtra nulls sin warnings
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing molecules: {ex}");
                return new List<RequestSearchMolecules>();
            }
        }


    }
}
