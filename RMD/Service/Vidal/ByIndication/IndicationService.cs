using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByIndication;
using RMD.Extensions.Vidal.ByIndication;
using System.Diagnostics;
using System.Xml;


namespace RMD.Service.Vidal.ByIndication
{
    public class IndicationService : IIndicationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;


        public IndicationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<IndicationVMP>> GetVmpsByIndicationIdAsync(int indicationId)
        {
            var VmpsEntries = new List<IndicationVMP>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/indication/{indicationId}/vmps?app_id={_appId}&app_key={_appKey}&start-page={_startPage}&page-size={_pageSize}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    // Verificar si la respuesta es 404 (Not Found)
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Retornar una lista vacía y lanzar una excepción personalizada
                        throw new Exception($"El ID de indicación {indicationId} no existe.");
                    }

                    // Asegurarse de que la respuesta fue exitosa (status 2xx)
                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var vmps = xmlContent.ParseIndicationVmpsXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    VmpsEntries.AddRange(vmps);

                    startPage++;
                } while (VmpsEntries.Count < totalResults);

                return VmpsEntries;
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
                throw new Exception($"Ha ocurrido un error: {ex.Message}");
            }
        }


        public async Task<IndicationDetail> GetIndicationByIdAsync(int indicationId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/indication/{indicationId}?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var indication = IndicationXmlParser.ParseIndicationXml(xmlContent);
            return indication;
        }

        public async Task<List<IndicationProduct>> GetProductsByIndicationIdAsync(int indicationId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/indication/{indicationId}/products?app_id={_appId}&app_key={_appKey}");

                // Verificar si la respuesta fue exitosa (2xx) o no
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Si el ID no existe, retorna una lista vacía y un mensaje adecuado
                        return new List<IndicationProduct>();
                    }

                    // Para otros errores, arroja una excepción genérica
                    throw new Exception($"Error al obtener productos: {response.StatusCode}");
                }

                var xmlContent = await response.Content.ReadAsStringAsync();
                var products = IndicationXmlParser.ParseIndicationProductsXml(xmlContent);
                return products;
            }
            catch (HttpRequestException httpEx)
            {
                // Manejo específico de errores de solicitud HTTP
                Console.WriteLine($"Error HTTP: {httpEx.Message}");
                throw new Exception("Error en la solicitud al servidor.");
            }
            catch (Exception ex)
            {
                // Manejo genérico de otros errores
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Ocurrió un error inesperado al obtener los productos.");
            }
        }






    }
}

