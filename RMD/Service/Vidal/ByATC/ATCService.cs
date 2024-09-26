using RMD.Extensions.Vidal.ByATC;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByATC;
using System.Diagnostics;
using System.Xml;

namespace RMD.Service.Vidal.ByATC
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

        public async Task<List<ATCVMPEntry>> GetVmpsByAtcClassificationAsync(int atcId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/atc-classification/{atcId}/vmps?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var vmps = ATCXmlParser.ParseATCVMPXml(xmlContent);
            return vmps;
        }

        public async Task<List<AtcProduct>> GetProductsByAtcClassificationAsync(int atcId)
        {
            var url = $"{_baseUrl}/atc-classification/{atcId}/products?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return ATCXmlParser.ParseProductsXml(content);
        }

        public async Task<List<ATCClassificationEntry>> GetAtcChildrenAsync(int atcId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/atc-classification/{atcId}/children?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var children = ATCXmlParser.ParseATCChildrenXml(xmlContent);
            return children;
        }

        public async Task<ATCClassificationDetail> GetAtcClassificationByIdAsync(int atcId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/atc-classification/{atcId}/?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var classificationDetail = xmlContent.ParseATCClassificationByIdXml();

            return classificationDetail;
        }


        public async Task<List<ATCClassification>> GetAllATCClassificationsAsync()
        {
            var atccEntries = new List<ATCClassification>();
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
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    atccEntries.AddRange(pageClassifications);

                    startPage++;
                } while (atccEntries.Count < totalResults);
                Debug.WriteLine("Procesamiento de productos completo.");
                try
                {
                    return atccEntries;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al retornar productos: {ex.Message}");
                    throw; // Re-throw si necesitas manejar la excepción más arriba
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Manejo de errores de HTTP
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener productos. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                // Manejo de errores de XML
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos del producto. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                // Manejo de errores genéricos
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }

        }

    }
}
