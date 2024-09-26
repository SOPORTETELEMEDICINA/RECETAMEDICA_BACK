//using RMD.Extensions;
//using RMD.Interface.Vidal;
//using RMD.Models.Vidal;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Xml.Linq;
//using Microsoft.Extensions.Configuration;
//using System.Xml;
//using System.Net;

//namespace RMD.Service.Vidal
//{
//    public class VidalApiService : IVidalApiService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _baseUrl;
//        private readonly string _appId;
//        private readonly string _appKey;

//        public VidalApiService(HttpClient httpClient, IConfiguration configuration)
//        {
//            _httpClient = httpClient;
//            _baseUrl = configuration["VidalApi:BaseUrl"];
//            _appId = configuration["VidalApi:AppId"];
//            _appKey = configuration["VidalApi:AppKey"];
//        }


//        public async Task<Drug> GetDrugByIdAsync(string id)
//        {
//            var response = await _httpClient.GetAsync($"/rest/api/product/{id}?app_id={_appId}&app_key={_appKey}");
//            response.EnsureSuccessStatusCode();
//            var drug = await response.Content.ReadFromJsonAsync<Drug>();
//            return drug ?? new Drug(); // Proporcionar un valor por defecto
//        }

//        public async Task<List<Drug>> SearchDrugsAsync(string query)
//        {
//            var response = await _httpClient.GetAsync($"/rest/api/products?q={query}&app_id={_appId}&app_key={_appKey}");
//            response.EnsureSuccessStatusCode();
//            var drugs = await response.Content.ReadFromJsonAsync<List<Drug>>();
//            return drugs ?? new List<Drug>(); // Proporcionar un valor por defecto
//        }

//        public async Task<Prescription> AnalyzePrescriptionAsync(Prescription prescription)
//        {
//            var response = await _httpClient.PostAsJsonAsync($"/rest/api/alerts/full?app_id={_appId}&app_key={_appKey}", prescription);
//            response.EnsureSuccessStatusCode();
//            var analysisResult = await response.Content.ReadFromJsonAsync<Prescription>();
//            return analysisResult ?? new Prescription(); // Proporcionar un valor por defecto
//        }

//        //public async Task<List<ProductEntry>> GetProductsAsync()
//        //{
//        //    var productEntries = new List<ProductEntry>();
//        //    var startPage = 20;
//        //    var pageSize = 25;
//        //    var totalResults = 0;

//        //    do
//        //    {
//        //        var requestUrl = $"{_baseUrl}/products?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
//        //        var response = await _httpClient.GetAsync(requestUrl);

//        //        response.EnsureSuccessStatusCode();

//        //        var xmlContent = await response.Content.ReadAsStringAsync();
//        //        var productsPage = xmlContent.ParseProductsXml();

//        //        // Obtener información de paginación
//        //        var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
//        //        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
//        //        var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

//        //        productEntries.AddRange(productsPage);

//        //        foreach (var entry in productsPage)
//        //        {
//        //            // Obtener y asignar el detalle del producto
//        //            entry.ProductDetail = await GetProductDetailAsync(entry.ProductId);
//        //            // Obtener y asignar los paquetes del producto
//        //            entry.Packages = await GetPackagesAsync(entry.ProductId);
//        //            // Obtener y asignar Molecules del Producto
//        //            entry.Molecules = await GetMoleculesAsync(entry.ProductId);
//        //        }

//        //        startPage++;
//        //    } while (productEntries.Count < totalResults);

//        //    return productEntries;
//        //}

//        public async Task<List<ProductEntry>> GetProductsAsync()
//        {
//            var requestUrl = $"{_baseUrl}/products?app_id={_appId}&app_key={_appKey}";
//            var response = await _httpClient.GetAsync(requestUrl);

//            response.EnsureSuccessStatusCode();

//            var xmlContent = await response.Content.ReadAsStringAsync();
//            var productEntries = xmlContent.ParseProductsXml();

//            foreach (var entry in productEntries)
//            {
//                // Obtener y asignar el detalle del producto
//                entry.ProductDetail = await GetProductDetailAsync(entry.ProductId);
//                // Obtener y asignar los paquetes del producto
//                if (!string.IsNullOrEmpty(entry.PackagesLink))
//                {
//                    entry.Packages = await GetPackagesAsync(entry.ProductId);
//                }
//                // Obtener y asignar Molecules del Producto
//                if (!string.IsNullOrEmpty(entry.MoleculesLink))
//                {
//                    entry.Molecules = await GetMoleculesAsync(entry.ProductId);
//                }
//                // Obtener las indicaciones y asignarlas si no están vacías
//                if (!string.IsNullOrEmpty(entry.IndicationsLink))
//                {
//                    var indicationsResponse = await GetIndicationsAsync(entry.ProductId);
//                    if (indicationsResponse.Indications.Any() || indicationsResponse.IndicationGroups.Any())
//                    {
//                        entry.Indications = indicationsResponse;
//                    }
//                }
//            }

//            return productEntries;
//        }

//        private async Task<List<ProductEntry>> GetProductsAsync(string productId)
//        {
//            var requestUrl = $"{_baseUrl}/products?app_id={_appId}&app_key={_appKey}";
//            var response = await _httpClient.GetAsync(requestUrl);

//            response.EnsureSuccessStatusCode();

//            var xmlContent = await response.Content.ReadAsStringAsync();
//            var productEntries = xmlContent.ParseProductsXml();

//            foreach (var entry in productEntries)
//            {
//                // Obtener y asignar el detalle del producto
//                entry.ProductDetail = await GetProductDetailAsync(entry.ProductId);
//                // Obtener y asignar los paquetes del producto
//                if (!string.IsNullOrEmpty(entry.PackagesLink))
//                {
//                    entry.Packages = await GetPackagesAsync(entry.ProductId);
//                }
//                // Obtener y asignar Molecules del Producto
//                if (!string.IsNullOrEmpty(entry.MoleculesLink))
//                {
//                    entry.Molecules = await GetMoleculesAsync(entry.ProductId);
//                }
//                // Obtener las indicaciones y asignarlas si no están vacías
//                if (!string.IsNullOrEmpty(entry.IndicationsLink))
//                {
//                    var indicationsResponse = await GetIndicationsAsync(entry.ProductId);
//                    if (indicationsResponse.Indications.Any() || indicationsResponse.IndicationGroups.Any())
//                    {
//                        entry.Indications = indicationsResponse;
//                    }
//                }
//            }
//            return productEntries;
//        }


//        private async Task<ProductDetail> GetProductDetailAsync(string productId)
//        {
//            var requestUrl = $"{_baseUrl}/product/{productId}?app_id={_appId}&app_key={_appKey}";
//            var response = await _httpClient.GetAsync(requestUrl);
//            response.EnsureSuccessStatusCode();
//            var xmlContent = await response.Content.ReadAsStringAsync();
//            return xmlContent.ParseProductDetailXml();
//        }

//        private async Task<List<Package>> GetPackagesAsync(string productId)
//        {
//            var requestUrl = $"{_baseUrl}/product/{productId}/packages?app_id={_appId}&app_key={_appKey}";
//            var response = await _httpClient.GetAsync(requestUrl);
//            response.EnsureSuccessStatusCode();
//            var xmlContent = await response.Content.ReadAsStringAsync();
//            return xmlContent.ParsePackagesXml();
//        }

//        private async Task<List<MoleculeEntry>> GetMoleculesAsync(string productId)
//        {
//            var requestUrl = $"{_baseUrl}/product/{productId}/molecules?app_id={_appId}&app_key={_appKey}";
//            var response = await _httpClient.GetAsync(requestUrl);
//            response.EnsureSuccessStatusCode();
//            var xmlContent = await response.Content.ReadAsStringAsync();
//            return xmlContent.ParseMoleculesXml();
//        }

//        private async Task<IndicationsResponse> GetIndicationsAsync(string productId)
//        {
//            try
//            {
//                var requestUrl = $"{_baseUrl}/product/{productId}/indications?app_id={_appId}&app_key={_appKey}";
//                var response = await _httpClient.GetAsync(requestUrl);

//                // Si la respuesta es 204 No Content, devuelve un objeto IndicationsResponse vacío
//                if (response.StatusCode == HttpStatusCode.NoContent)
//                {
//                    return new IndicationsResponse();
//                }

//                // Verifica que la respuesta HTTP fue exitosa
//                response.EnsureSuccessStatusCode();

//                // Lee el contenido de la respuesta
//                var xmlContent = await response.Content.ReadAsStringAsync();

//                // Verificar si el contenido XML está vacío
//                if (string.IsNullOrWhiteSpace(xmlContent))
//                {
//                    return new IndicationsResponse();
//                }

//                // Parsea el contenido XML y devuelve el resultado
//                return xmlContent.ParseIndicationsXml();
//            }
//            catch (HttpRequestException httpEx)
//            {
//                // Manejo de errores relacionados con la solicitud HTTP
//                Console.WriteLine($"HTTP Request error: {httpEx.Message}");
//                // Puedes registrar el error o lanzar una excepción personalizada si es necesario
//                throw;
//            }
//            catch (XmlException xmlEx)
//            {
//                // Manejo de errores relacionados con el parsing del XML
//                Console.WriteLine($"XML Parsing error: {xmlEx.Message}");
//                // Puedes registrar el error o lanzar una excepción personalizada si es necesario
//                throw;
//            }
//            catch (Exception ex)
//            {
//                // Manejo de cualquier otro tipo de excepción
//                Console.WriteLine($"Unexpected error: {ex.Message}");
//                // Puedes registrar el error o lanzar una excepción personalizada si es necesario
//                throw;
//            }
//        }



//    }
//}
