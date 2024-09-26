using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Extensions.Vidal.ByProduct;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal;
using RMD.Models.Vidal.ByProduct;
using RMD.Models.Vidal.ByRoute;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Xml;

namespace RMD.Service.Vidal.ByProduct
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _basicUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;
        private readonly VidalDbContext _context;


        public ProductService(HttpClient httpClient, IConfiguration configuration, VidalDbContext context)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
            _basicUrl = configuration["VidalApi:BasicUrl"] ?? throw new ArgumentNullException(nameof(_basicUrl));
            _context = context;
        }

        
        public async Task<ProductModel> GetProductByIdAsync(int productId)
        {
            try
            {
                // Llamar al SP para obtener el producto por IdProduct
                var product = await _context.Productos
                    .FromSqlRaw("EXEC Vidal_GetProductById @IdProduct", new SqlParameter("@IdProduct", productId))
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return null;
                }

                return product;
            }
            catch (Exception ex)
            {
                // Manejar excepciones
                Console.WriteLine($"Error al obtener producto por ID: {ex.Message}");
                throw;
            }
        }

        //public async Task<ProductModel> GetProductByIdAsync(int productId)
        //{
        //    try
        //    {
        //        // Llamar al SP para obtener el producto por IdProduct
        //        var product = await _context.Productos
        //            .FromSqlRaw("EXEC Vidal_GetProductById @IdProduct", new SqlParameter("@IdProduct", productId))
        //            .FirstOrDefaultAsync();

        //        if (product == null)
        //        {
        //            return null;
        //        }

        //        return product;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejar excepciones
        //        Console.WriteLine($"Error al obtener producto por ID: {ex.Message}");
        //        throw;
        //    }
        //}


        //public async Task<List<Product>> GetAllProductsAsync()
        //{
        //    var productEntries = new List<Product>();
        //    int startPage = int.Parse(_startPage);
        //    int pageSize = int.Parse(_pageSize); 
        //    var totalResults = 0;

        //    try
        //    {
        //        do
        //        {
        //            Debug.WriteLine("Este es un mensaje en la ventana de salida de Visual Studio." + startPage);
        //            var requestUrl = $"{_baseUrl}/products?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
        //            var response = await _httpClient.GetAsync(requestUrl);

        //            response.EnsureSuccessStatusCode();

        //            var xmlContent = await response.Content.ReadAsStringAsync();
        //            var productsPage = xmlContent.ParseProductsXml();

        //            // Obtener información de paginación
        //            var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
        //            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
        //            var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

        //            productEntries.AddRange(productsPage);

        //            startPage++;
        //        } while (productEntries.Count < totalResults);
        //        Debug.WriteLine("Procesamiento de productos completo.");
        //        try
        //        {
        //            return productEntries;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Error al retornar productos: {ex.Message}");
        //            throw; // Re-throw si necesitas manejar la excepción más arriba
        //        }
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        // Manejo de errores de HTTP
        //        Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
        //        throw new Exception("Error al obtener productos. Por favor, intente de nuevo más tarde.");
        //    }
        //    catch (XmlException xmlEx)
        //    {
        //        // Manejo de errores de XML
        //        Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
        //        throw new Exception("Error al procesar los datos del producto. Por favor, intente de nuevo más tarde.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejo de errores genéricos
        //        Console.WriteLine($"Error inesperado: {ex.Message}");
        //        throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
        //    }
        //}


        //public async Task<ProductById> GetProductByIdAsync(int productId)
        //{
        //    var url = $"{_baseUrl}/product/{productId}?app_id={_appId}&app_key={_appKey}";
        //    var response = await _httpClient.GetAsync(url);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        // Manejar error de la respuesta
        //        return null;
        //    }

        //    var xmlContent = await response.Content.ReadAsStringAsync();
        //    return xmlContent.ParseProductByIdXml();
        //}

        public async Task<List<ProductPackage>> GetProductPackagesAsync(int productId)
        {
            var packages = new List<ProductPackage>();
            var requestUrl = $"{_baseUrl}/product/{productId}/packages?app_id={_appId}&app_key={_appKey}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                packages = xmlContent.ParseProductPackagesXml();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener paquetes. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos de los paquetes. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }

            return packages;
        }

        public async Task<List<ProductMolecule>> GetProductMoleculesAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/molecules?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var molecules = xmlContent.ParseProductMoleculesXml();

            return molecules;
        }

        public async Task<List<ProductForeign>> GetProductForeignAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/foreign-products?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var foreignProducts = xmlContent.ParseProductForeignXml();

            return foreignProducts;
        }

        public async Task<List<ProductIndication>> GetProductIndicationsAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/indications?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var indications = xmlContent.ParseProductIndicationsXml();

            return indications;
        }

        public async Task<List<ProductUcd>> GetProductUcdsAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/ucds?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var ucds = xmlContent.ParseProductUcdsXml();

            return ucds;
        }

        public async Task<List<ProductUnit>> GetProductUnitsAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/units?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var units = xmlContent.ParseProductUnitsXml();

            return units;
        }

        public async Task<List<ProductRoute>> GetProductRoutesAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/routes?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseProductRoutesXml();
        }


        public async Task<ProductIndicator> GetProductIndicatorsAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/indicators?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseProductIndicatorsXml();
        }

        public async Task<ProductSideEffect> GetProductSideEffectsAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/{productId}/side-effects?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseProductSideEffectsXml();
        }

        public async Task<ProductUCDV> GetProductUCDVAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/{productId}/ucdvs?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseProductUCDVXml();
        }

        public async Task<ProductAllergy> GetProductAllergyAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/allergies?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseProductAllergyXml();
        }

        public async Task<ProductAtcClassification> GetProductAtcClassificationAsync(int productId)
        {
            var requestUrl = $"{_baseUrl}/product/{productId}/atc-classification?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseProductAtcClassificationXml();
        }

        public async Task<ProductVMPGroup> GetVmpByProductGroupAsync(int productGroupId)
        {
            var requestUrl = $"{_baseUrl}/product-group/{productGroupId}/vmp?app_id={_appId}&app_key={_appKey}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var vmpGroup = xmlContent.ParseProductVmpGroupXml();

                return vmpGroup;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener VMPs. Por favor, intente de nuevo más tarde.");
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

        public async Task<List<Product>> GetProductsByNameAsync(string productName)
        {
            var productEntries = new List<Product>();

            try
            {
                var requestUrl = $"{_baseUrl}/products?q={productName}&app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                productEntries = xmlContent.ParseProductsXml();

                return productEntries;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al obtener productos por nombre. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw new Exception("Error al procesar los datos del producto por nombre. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<List<ProductEntry>> GetProductsByName(string name)
        {
            var productEntries = new List<ProductEntry>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/products?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var productPage = xmlContent.ParseProductConsultaXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    productEntries.AddRange(productPage);

                    startPage++;
                } while (productEntries.Count < totalResults);
                return productEntries;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<List<ProductUnit>> GetProductUnitsByLinkAsync(string link)
        {
            var response = await _httpClient.GetAsync($"{_basicUrl}{link}?app_id=75f87542&app_key=f5d38f3120e884a77ddac6329c281830");
            response.EnsureSuccessStatusCode();
            var xmlContent = await response.Content.ReadAsStringAsync();
            var packageUnits = xmlContent.PackageUnitByLinkXmlParser();
            return packageUnits;
        }

    }
}
