using RMD.Extensions.Vidal.ByVTM;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByVTM;
using System.Diagnostics;
using System.Xml;

namespace RMD.Service.Vidal.ByVTM
{
    public class VTMService : IVTMService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public VTMService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<VTMEntry> GetVTMById(int vtmId)
        {
            var requestUrl = $"{_baseUrl}/vtm/{vtmId}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVTMXml();
        }

        public async Task<List<VTMS>> GetVtmsAsync()
        {
            var vtmEntries = new List<VTMS>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    Debug.WriteLine($"Procesando página: {startPage}");
                    var requestUrl = $"{_baseUrl}/vtms?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var vtmsPage = xmlContent.ParseVtmXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    vtmEntries.AddRange(vtmsPage);

                    startPage++;
                } while (vtmEntries.Count < totalResults);
                Debug.WriteLine("Procesamiento de VTMs completo.");
                return vtmEntries;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener VTMs. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos de VTM. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<List<VTMMolecule>> GetMoleculesByVtmIdAsync(int vtmId)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/vtm/{vtmId}/molecules?app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var molecules = xmlContent.ParseMoleculeXml();

                return molecules;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener las moléculas. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos de moléculas. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }
    }
}
