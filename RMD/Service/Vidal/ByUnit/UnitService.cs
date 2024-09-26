using RMD.Extensions.Vidal.ByUnit;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByUnit;
using System.Diagnostics;
using System.Xml;

namespace RMD.Service.Vidal.ByUnit
{
    public class UnitService : IUnitService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public UnitService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<Units>> GetAllUnitsAsync()
        {
            var unitEntries = new List<Units>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    Debug.WriteLine($"Procesando página: {startPage}");
                    var requestUrl = $"{_baseUrl}/units?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var unitsPage = xmlContent.ParseUnitsXml();

                    // Obtener información de paginación
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

                    unitEntries.AddRange(unitsPage);

                    startPage++;
                } while (unitEntries.Count < totalResults);

                Debug.WriteLine("Procesamiento de Units completo.");

                return unitEntries; // Devuelve la lista sin usar ResponseFromService para coincidir con la interfaz
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                return new List<Units>(); // Retorna una lista vacía en caso de error
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                return new List<Units>(); // Retorna una lista vacía en caso de error
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new List<Units>(); // Retorna una lista vacía en caso de error
            }
        }

        public async Task<Unit> GetUnitById(int id)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/unit/{id}?app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return new Unit(); // Retorna un modelo vacío si no se encuentra la unidad
                }

                var xmlContent = await response.Content.ReadAsStringAsync();
                var unit = xmlContent.ParseUnitXml();

                return unit ?? new Unit(); // Retorna el modelo o un modelo vacío si no se encuentra nada
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                return new Unit(); // Retorna un modelo vacío en caso de error
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                return new Unit(); // Retorna un modelo vacío en caso de error
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new Unit(); // Retorna un modelo vacío en caso de error
            }
        }
    }
}
