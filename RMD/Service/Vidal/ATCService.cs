using RMD.Extensions.Vidal;
using RMD.Interface.Vidal;
using RMD.Models.Vidal;
using System.Diagnostics;
using System.Xml;

namespace RMD.Service.Vidal
{
    public class ATCService : IATCService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public ATCService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<ATCDetail>> GetVmpsByAtcClassificationAsync(int atcId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/atc-classification/{atcId}/vmps?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var vmps = XmlExtensionsByATC.ParseATCVMPXml(xmlContent);
            return vmps;
        }

        public async Task<List<ATCDetail>> GetProductsByAtcClassificationAsync(int atcId)
        {
            var url = $"{_baseUrl}/atc-classification/{atcId}/products?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return XmlExtensionsByATC.ParseProductsXml(content);
        }

        public async Task<List<ATCClassification>> GetAtcChildrenAsync(int atcId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/atc-classification/{atcId}/children?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var children = XmlExtensionsByATC.ParseATCChildrenXml(xmlContent);
            return children;
        }

        public async Task<ATCDetail> GetAtcClassificationByIdAsync(int atcId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/atc-classification/{atcId}/?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var classificationDetail = xmlContent.ParseATCClassificationByIdXml();

            return classificationDetail;
        }

        public async Task<List<ATCClassification>> GetAllATCClassificationsAsync()
        {
            var atcEntries = new List<ATCClassification>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/atc-classifications?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var pageClassifications = xmlContent.ParseATCClassificationsXml();

                    // Obtener información de paginación
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

                    atcEntries.AddRange(pageClassifications);
                    startPage++;

                } while (atcEntries.Count < totalResults);

                Debug.WriteLine("Procesamiento de clasificaciones ATC completo.");
                return atcEntries;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener clasificaciones ATC. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos de clasificaciones ATC. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

    }
}
