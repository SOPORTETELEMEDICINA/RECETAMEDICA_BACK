using RMD.Extensions.Vidal.ByIndicationGroup;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.ByIndicationGroup;

namespace RMD.Service.Vidal.ByIndicationGroup
{
    public class IndicationGroupService : IIndicationGroupService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public IndicationGroupService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<IndicationGroup> GetIndicationGroupByIdAsync(int indicationGroupId)
        {
            var requestUrl = $"{_baseUrl}/indication-group/{indicationGroupId}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseIndicationGroupXml();
        }

        public async Task<List<IndicationGroupProduct>> GetProductsByIndicationGroupIdAsync(int indicationGroupId)
        {
            var productEntries = new List<IndicationGroupProduct>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;
            var currentPageResults = 0; // Número de resultados en la página actual

            do
            {
                var requestUrl = $"{_baseUrl}/indication-group/{indicationGroupId}/products?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var productsPage = xmlContent.ParseIndicationGroupProductsXml();

                // Obtener información de paginación
                if (totalResults == 0)
                {
                    // Solo establecer `totalResults` en la primera iteración
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                }

                // Número de resultados en la página actual
                currentPageResults = productsPage.Count;

                // Agregar productos de la página actual a la lista general
                productEntries.AddRange(productsPage);

                // Incrementar la página
                startPage++;

            } while (productEntries.Count < totalResults && currentPageResults > 0);  // Salir si no se obtienen más resultados

            return productEntries;
        }



        public async Task<List<CIM10>> GetCIM10EntriesAsync(int indicationGroupId)
        {
            var requestUrl = $"{_baseUrl}/indication-group/{indicationGroupId}/cim10s?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var cim10Entries = xmlContent.ParseCIM10EntriesXml();

            return cim10Entries;
        }

        public async Task<List<VMP>> GetVMPsByIndicationGroupIdAsync(int indicationGroupId)
        {
            var vmpEntries = new List<VMP>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;
            var currentPageResults = 0; // Número de resultados en la página actual

            do
            {
                var requestUrl = $"{_baseUrl}/indication-group/{indicationGroupId}/vmps?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var vmpsPage = xmlContent.ParseVMPsXml();

                // Obtener información de paginación
                if (totalResults == 0)
                {
                    // Solo establecer `totalResults` en la primera iteración
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                }

                // Número de resultados en la página actual
                currentPageResults = vmpsPage.Count;

                // Agregar los resultados de la página actual a la lista completa
                vmpEntries.AddRange(vmpsPage);

                // Incrementar la página
                startPage++;

            } while (vmpEntries.Count < totalResults && currentPageResults > 0); // Salir si no hay más resultados en la página actual

            return vmpEntries;
        }


        public async Task<List<Indication>> GetIndicationsByIndicationGroupIdAsync(int indicationGroupId)
        {
            var indications = new List<Indication>();

            var requestUrl = $"{_baseUrl}/indication-group/{indicationGroupId}/indications?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            indications = xmlContent.ParseIndicationsXml();

            return indications;
        }

    }
}
