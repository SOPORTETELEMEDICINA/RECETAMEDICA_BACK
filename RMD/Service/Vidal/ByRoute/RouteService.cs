using RMD.Extensions.Vidal.ByRoute;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByRoute;
using System.Diagnostics;
using System.Xml;

namespace RMD.Service.Vidal.ByRoute
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public RouteService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<Routes>> GetAllRoutesAsync()
        {
            var routeEntries = new List<Routes>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    Debug.WriteLine("Procesando página: " + startPage);
                    var requestUrl = $"{_baseUrl}/routes?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var routesPage = xmlContent.ParseRoutesXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    routeEntries.AddRange(routesPage);

                    startPage++;
                } while (routeEntries.Count < totalResults);

                Debug.WriteLine("Procesamiento de rutas completo.");

                return routeEntries;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener rutas. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos de la ruta. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }


        public async Task<Routes> GetRouteByIdAsync(int id)
        {
            var requestUrl = $"{_baseUrl}/route/{id}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var route = xmlContent.ParseRouteXml();

            return route;
        }
    }
}
