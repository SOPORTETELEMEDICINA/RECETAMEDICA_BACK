using RMD.Extensions.Vidal.ByUCDV;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByUCDV;
using System.Diagnostics;
using System.Net;
using System.Xml;

namespace RMD.Service.Vidal.ByUCDV
{
    public class UcdvService : IUcdvService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public UcdvService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<UCDVS>> GetAllUcdvsAsync()
        {
            var ucdvEntries = new List<UCDVS>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/ucdvs?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var ucdvsPage = xmlContent.ParseUcdvsXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    ucdvEntries.AddRange(ucdvsPage);

                    startPage++;
                } while (ucdvEntries.Count < totalResults);

                return ucdvEntries;
            }
            catch (Exception ex)
            {
                // Manejar errores
                throw new Exception($"Error al obtener UCDVs: {ex.Message}");
            }
        }

        public async Task<UCDV> GetUcdvByIdAsync(int id)
        {
            var requestUrl = $"{_baseUrl}/ucdv/{id}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var ucdv = xmlContent.ParseUcdvXml();

            return ucdv;
        }

        public async Task<List<UCDVRoute>> GetRoutesForUcdvAsync(int ucdvId)
        {
            var requestUrl = $"{_baseUrl}/ucdv/{ucdvId}/routes?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUcdvRoutesXml();
        }

        public async Task<List<UCDVUnit>> GetUnitsForUcdvAsync(int ucdvId)
        {
            var requestUrl = $"{_baseUrl}/ucdv/{ucdvId}/units?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUcdvUnitsXml();
        }

        public async Task<List<UCDVPackage>> GetUcdvPackagesAsync(int ucdvId)
        {
            var packageEntries = new List<UCDVPackage>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/ucdv/{ucdvId}/packages?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var packagesPage = xmlContent.ParseUcdvPackagesXml();

                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    packageEntries.AddRange(packagesPage);

                    startPage++;
                } while (packageEntries.Count < totalResults);

                return packageEntries;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener Packages: {ex.Message}");
            }
        }

        public async Task<List<UCDVProduct>> GetUcdvProductsAsync(int ucdvId)
        {
            var requestUrl = $"{_baseUrl}/ucdv/{ucdvId}/products?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUcdvProductsXml();
        }

        public async Task<List<UCDVMolecule>> GetMoleculesByUCDVIdAsync(int ucdvId)
        {
            var requestUrl = $"{_baseUrl}/ucdv/{ucdvId}/molecules?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUcdvMoleculesXml();
        }
    }

}
