using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions.Consulta;
using RMD.Extensions.Vidal.ByPackage;
using RMD.Interface.Consulta;
using RMD.Models.Consulta;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace RMD.Service.Consulta
{
    public class ConsultaService : IConsultaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;
        private readonly ConsultaDbContext _context;

        public ConsultaService(HttpClient httpClient, IConfiguration configuration, ConsultaDbContext context)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
            _context = context;

        }


        public async Task<IEnumerable<RequestSearchAllergy>> GetAllergiesByNameAsync(string name)
        {
            var nameParam = new SqlParameter("@name", name);

            try
            {
                var allergies = await _context.Allergies
                    .FromSqlRaw("EXEC Consulta_GetAllergyByName @name", nameParam)
                    .ToListAsync();

                if (allergies == null || allergies.Count == 0)
                {
                    return new List<RequestSearchAllergy>(); // Devuelve una lista vacía si no se encuentran resultados
                }

                return allergies;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RequestSearchMolecules>> GetMoleculeByNameAsync(string name)
        {
            var nameParam = new SqlParameter("@name", name);

            try
            {
                var molecules = await _context.Molecules
                    .FromSqlRaw("EXEC Consulta_GetMoleculesByName @name", nameParam)
                    .ToListAsync();

                if (molecules == null || molecules.Count == 0)
                {
                    return new List<RequestSearchMolecules>(); // Devuelve una lista vacía si no se encuentran resultados
                }

                return molecules;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RequestSearchVMP>> GetVMPByNameAsync(string name)
        {
            var nameParam = new SqlParameter("@name", name);

            try
            {
                var vmps = await _context.VMPS
                    .FromSqlRaw("EXEC Consulta_GetVMPByName @name", nameParam)
                    .ToListAsync();

                if (vmps == null || vmps.Count == 0)
                {
                    return new List<RequestSearchVMP>(); // Devuelve una lista vacía si no se encuentran resultados
                }

                return vmps;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RequestSearchProducts>> GetProductsByNameAsync(string name)
        {
            var nameParam = new SqlParameter("@name", name);

            try
            {
                var products = await _context.Products
                    .FromSqlRaw("EXEC Consulta_GetProductsByName @name", nameParam)
                    .ToListAsync();

                if (products == null || products.Count == 0)
                {
                    return new List<RequestSearchProducts>(); // Devuelve una lista vacía si no se encuentran resultados
                }

                return products;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RequestSearchPackage>> GetPackagesByNameAsync(string name)
        {
            var nameParam = new SqlParameter("@name", name);

            try
            {
                var packages = await _context.Packages
                    .FromSqlRaw("EXEC Consulta_GetPackagesByName @name", nameParam)
                    .ToListAsync();

                if (packages == null || packages.Count == 0)
                {
                    return new List<RequestSearchPackage>(); // Devuelve una lista vacía si no se encuentran resultados
                }

                return packages;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RequestSearchCIM10>> GetCIM10sByNameAsync(string name)
        {
            var nameParam = new SqlParameter("@name", name);

            try
            {
                var cim10s = await _context.CIM10s
                    .FromSqlRaw("EXEC Consulta_GetCIM10ByName @name", nameParam)
                    .ToListAsync();

                if (cim10s == null || cim10s.Count == 0)
                {
                    return new List<RequestSearchCIM10>(); // Devuelve una lista vacía si no se encuentran resultados
                }

                return cim10s;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
            }
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
                    var requestUrl = $"{_baseUrl}/packages?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var packagePage = xmlContent.ParsePackageConsultaXml();

                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
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


        public async Task<List<Cim10Entry>> GetCim10ByName(string name)
        {
            var cim10Entries = new List<Cim10Entry>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/pathologies?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var cim10Page = xmlContent.ParseCim10Xml();

                    // Obtener información de paginación
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

                    cim10Entries.AddRange(cim10Page);

                    startPage++;
                } while (cim10Entries.Count < totalResults);

                Debug.WriteLine("Procesamiento de CIM10 completo.");
                return cim10Entries;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

       

        public async Task<string> ProcessPrescriptionRequest(PrescriptionModel request)
        {
            try
            {
                // Convertir el modelo a XML utilizando la función de extensión
                var xmlContent = request.ParseToXml();

                // Validar el contenido del XML
                if (!ValidateXmlContent(xmlContent))
                {
                    throw new Exception("El XML generado no es válido.");
                }

                // Enviar el XML al endpoint externo
                var response = await SendXmlToExternalApi(xmlContent);

                // Devolver la respuesta en HTML
                return response; // Aquí estamos retornando el HTML como un string
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.", ex);
            }
        }



        private bool ValidateXmlContent(string xmlContent)
        {
            try
            {
                var xmlDoc = XDocument.Parse(xmlContent);
                var rootElement = xmlDoc.Root;

                if (rootElement == null || rootElement.Name != "prescription")
                {
                    return false; // El XML no tiene el elemento raíz esperado
                }

                var patientElement = rootElement.Element("patient");
                if (patientElement == null)
                {
                    return false; // El XML no contiene la información del paciente
                }

                var prescriptionLinesElement = rootElement.Element("prescription-lines");
                if (prescriptionLinesElement == null || !prescriptionLinesElement.Elements("prescription-line").Any())
                {
                    return false; // El XML no contiene líneas de prescripción
                }

                // Si todo está bien
                return true;
            }
            catch (Exception)
            {
                // Si el XML no está bien formado o no pasa las validaciones, retorna false
                return false;
            }
        }


        private async Task<string> SendXmlToExternalApi(string xmlContent)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/alerts/full/html";

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                requestMessage.Headers.Add("app_id", _appId);
                requestMessage.Headers.Add("app_key", _appKey);
                requestMessage.Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml");
                Console.WriteLine(xmlContent); // Para depuración, puedes eliminarlo en producción

                var response = await _httpClient.SendAsync(requestMessage);

                // Asegúrate de que la respuesta sea exitosa
                response.EnsureSuccessStatusCode();

                // Leer la respuesta del servidor, que se espera sea HTML
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent; // Aquí estás recibiendo el HTML como un string
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error al realizar la solicitud HTTP: {httpEx.Message}");
                throw new Exception("Error al enviar el XML al endpoint externo. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado al enviar el XML. Por favor, intente de nuevo más tarde.");
            }
        }


        private void SavePrescription(string request)
        {
            // Lógica para guardar los datos en tu base de datos
            // Puedes usar _context para acceder a tu DbContext y guardar los datos en la base de datos.
        }

        

    }
}
