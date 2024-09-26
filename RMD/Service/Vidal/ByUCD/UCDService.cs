using RMD.Extensions.Vidal.ByUCD;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByUCD;

namespace RMD.Service.Vidal.ByUCD
{
    public class UCDService : IUCDService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public UCDService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<Ucd>> GetAllUcdsAsync()
        {
            var ucdEntries = new List<Ucd>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/ucds?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var ucdsPage = xmlContent.ParseUcdsXml();

                    // Obtener información de paginación
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

                    ucdEntries.AddRange(ucdsPage);
                    startPage++;
                } while (ucdEntries.Count < totalResults);

                return ucdEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Error al obtener UCDs. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<UCDByIdPackage> GetUcdByIdPackageAsync(int packageId)
        {
            var requestUrl = $"{_baseUrl}/ucd/{packageId}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUCDXml();
        }

        public async Task<UCDById> GetUcdByIdAsync(int ucdId)
        {
            var requestUrl = $"{_baseUrl}/ucd/{ucdId}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUcdXml();
        }

        public async Task<List<UcdPackage>> GetUcdPackagesByIdAsync(int ucdId)
        {
            var requestUrl = $"{_baseUrl}/ucd/{ucdId}/packages?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUcdPackagesXml();
        }

        public async Task<List<UcdProduct>> GetUcdProductsByIdAsync(int ucdId)
        {
            var requestUrl = $"{_baseUrl}/ucd/{ucdId}/products?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUcdProductsXml();
        }

        public async Task<List<UCDSideEffect>> GetSideEffectsByUcdIdAsync(int ucdId)
        {
            var requestUrl = $"{_baseUrl}/ucd/{ucdId}/side-effects?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseUcdSideEffectsXml();
        }
    }
}
