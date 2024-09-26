using RMD.Extensions.Consulta;
using RMD.Extensions.Vidal.ByPackage;
using RMD.Interface.Consulta;
using RMD.Models.Consulta;
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

        public ConsultaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));

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

        public async Task<List<AllergyEntry>> GetAllergiesByName(string name)
        {
            var allergyEntries = new List<AllergyEntry>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/allergies?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var allergyPage = xmlContent.ParseAllergyXml();

                    // Obtener información de paginación
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

                    allergyEntries.AddRange(allergyPage);

                    startPage++;
                } while (allergyEntries.Count < totalResults);

                Debug.WriteLine("Procesamiento de alergias completo.");
                return allergyEntries;
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
