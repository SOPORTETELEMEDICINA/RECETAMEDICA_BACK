using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RMD.Extensions.Vidal.ByPackage;
using RMD.Interface.Vidal;
using RMD.Models.Consulta;
using RMD.Models.Vidal.ByPackage;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static RMD.Extensions.Vidal.ByPackage.XmlExtensionsByPackage;

namespace RMD.Service.Vidal.ByPackage
{
    public class PackageService : IPackageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;
        private readonly string _basicUrl;

        public PackageService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _basicUrl = configuration["VidalApi:BasicUrl"] ?? throw new ArgumentNullException(nameof(_basicUrl));
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
            var packageEntries = new List<Package>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/packages?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var packagesPage = xmlContent.ParsePackagesXml();  // Método de extensión que parsea el XML a una lista de objetos `Package`

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    packageEntries.AddRange(packagesPage);

                    startPage++;
                } while (packageEntries.Count < totalResults);
                Debug.WriteLine("Procesamiento de paquetes completo.");
                try
                {
                    return packageEntries;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al retornar paquetes: {ex.Message}");
                    throw;
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener paquetes. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos del paquete. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<PackageById> GetPackageByIdAsync(int packageId)
        {
            var url = $"{_baseUrl}/package/{packageId}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Manejar error de la respuesta
                return null;
            }

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParsePackageXml();
        }

        public async Task<List<PackageRoute>> GetPackageRoutesAsync(int packageId)
        {
            var requestUrl = $"{_baseUrl}/package/{packageId}/routes?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var packageRoutes = xmlContent.ParsePackageRoutesXml();

            return packageRoutes;
        }

        public async Task<List<PackageIndicator>> GetPackageIndicatorsAsync(int packageId)
        {
            var requestUrl = $"{_baseUrl}/package/{packageId}/indicators?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var packageIndicators = xmlContent.ParsePackageIndicatorsXml();

            return packageIndicators;
        }

        public async Task<List<PackageUnit>> GetPackageUnitsByPackageIdAsync(int packageId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/package/{packageId}/units?app_id=75f87542&app_key=f5d38f3120e884a77ddac6329c281830");
            response.EnsureSuccessStatusCode();
            var xmlContent = await response.Content.ReadAsStringAsync();
            var packageUnits = xmlContent.PackageUnitXmlParser();
            return packageUnits;
        }

        public async Task<List<PackageIndication>> GetPackageIndicationsByPackageIdAsync(int packageId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/package/{packageId}/indications?app_id=75f87542&app_key=f5d38f3120e884a77ddac6329c281830");
            response.EnsureSuccessStatusCode();
            var xmlContent = await response.Content.ReadAsStringAsync();
            var packageIndications = xmlContent.PackageIndicationXmlParser();
            return packageIndications;
        }

        public async Task<List<PackageSideEffect>> GetPackageSideEffectsAsync(int packageId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/package/{packageId}/side-effects?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();
            var xmlContent = await response.Content.ReadAsStringAsync();
            var sideEffects = xmlContent.PackageSideEffectsXmlParser();
            return sideEffects;
        }

        public async Task<List<PackageAtcClassification>> GetPackageAtcClassificationsAsync(int packageId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/package/{packageId}/atc-classification?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();
            var xmlContent = await response.Content.ReadAsStringAsync();
            var atcClassifications = xmlContent.PackageAtcClassificationXmlParser();
            return atcClassifications;
        }

        public async Task<PackageVmp> GetVmpByPackageIdAsync(int packageId)
        {
            var requestUrl = $"{_baseUrl}/package/{packageId}?aggregate=VMP&app_id={_appId}&app_key={_appKey}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var vmpPackage = xmlContent.ParsePackageVmpXml();

                return vmpPackage;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener el VMP. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos del VMP. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }


        public async Task<PackageProduct> GetPackageProductByIdAsync(string packageId)
        {
            var url = $"{_baseUrl}/packages/{packageId}?aggregate=product&app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(url);
           
            response.EnsureSuccessStatusCode();

            var xmlString = await response.Content.ReadAsStringAsync();
            var packageProduct = xmlString.ParsePackageProductXml();

            return packageProduct;
        }

        public async Task<List<Package>> GetPackagesByNameAsync(string packageName)
        {
            var packageEntries = new List<Package>();

            try
            {
                var requestUrl = $"{_baseUrl}/packages?q={packageName}&app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                packageEntries = xmlContent.ParsePackagesXml();

                return packageEntries;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener paquetes por nombre. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos del paquete por nombre. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<List<PackageUnit>> GetPackageUnitsByLinkAsync(string link)
        {
            var response = await _httpClient.GetAsync($"{_basicUrl}{link}?app_id=75f87542&app_key=f5d38f3120e884a77ddac6329c281830");
            response.EnsureSuccessStatusCode();
            var xmlContent = await response.Content.ReadAsStringAsync();
            var packageUnits = xmlContent.PackageUnitByLinkXmlParser();
            return packageUnits;
        }

        public async Task<List<PackageEntry>> GetPackagesByName(string name)
        {
            var packageEntries = new List<PackageEntry>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/packages?q={name}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var packagePage = xmlContent.ParsePackageConsultaXml();


                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    packageEntries.AddRange(packagePage);

                    startPage++;
                } while (packageEntries.Count < totalResults);
                return packageEntries;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }
    }
}
