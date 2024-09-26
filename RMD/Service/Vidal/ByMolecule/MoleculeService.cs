using RMD.Extensions.Vidal.ByMolecule;
using RMD.Interface.Vidal;
using RMD.Models.Consulta;
using RMD.Models.Vidal.ByMolecule;
using System.Diagnostics;

namespace RMD.Service.Vidal.ByMolecule
{
    public class MoleculeService : IMoleculeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public MoleculeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<Molecule>> GetAllMoleculesAsync()
        {
            var moleculeEntries = new List<Molecule>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/molecules?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var moleculesPage = xmlContent.ParseMoleculesXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    moleculeEntries.AddRange(moleculesPage);

                    startPage++;
                } while (moleculeEntries.Count < totalResults);

                return moleculeEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error al obtener moléculas. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<Molecule> GetMoleculeById(int id)
        {
            var requestUrl = $"{_baseUrl}/molecule/{id}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var moleculesPage = xmlContent.ParseMoleculesEntrysXml();

            return moleculesPage;
        }

        public async Task<List<MoleculeEntry>> GetMoleculesByName(string name)
        {
            var moleculeEntries = new List<MoleculeEntry>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/molecules?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var moleculePage = xmlContent.ParseMoleculeConsultaXml();

                    // Obtener información de paginación
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

                    moleculeEntries.AddRange(moleculePage);

                    startPage++;
                } while (moleculeEntries.Count < totalResults);

                return moleculeEntries;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

    }
}
