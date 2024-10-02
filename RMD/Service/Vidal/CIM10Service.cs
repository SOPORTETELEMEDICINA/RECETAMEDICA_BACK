using RMD.Extensions.Vidal;
using RMD.Interface.Vidal;
using RMD.Models.Vidal;

namespace RMD.Service.Vidal
{
    public class CIM10Service : ICIM10Service
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public CIM10Service(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<CIM10>> GetAllCIM10sAsync()
        {
            var cim10Entries = new List<CIM10>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/cim10s?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var cim10Page = xmlContent.ParseCIM10sXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    cim10Entries.AddRange(cim10Page);

                    startPage++;
                } while (cim10Entries.Count < totalResults);

                return cim10Entries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error al obtener CIM10s. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<CIM10> GetCIM10ByIdAsync(int id)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/cim10/{id}?app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var cim10Entry = xmlContent.ParseCIM10Xml();

                return cim10Entry;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error al obtener el CIM10. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<List<CIM10Child>> GetCIM10ChildrenAsync(int parentId)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/cim10/{parentId}/children?Content-Type=text/html&app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var cim10Children = xmlContent.ParseCIM10ChildrenXml();

                return cim10Children;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error al obtener los hijos de CIM10. Por favor, intente de nuevo más tarde.");
            }
        }
    }
}
