using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Extensions.Vidal.ByVMP;
using RMD.Extensions.Vidal.ByVTM;
using RMD.Interface.Vidal;
using RMD.Models.Consulta;
using RMD.Models.Responses;
using RMD.Models.Vidal.ByVMP;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Xml;

namespace RMD.Service.Vidal.ByVMP
{
    public class VMPService : IVMPService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _basicUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;
        private readonly VidalDbContext _context;

        public VMPService(HttpClient httpClient, IConfiguration configuration, VidalDbContext context)
        {
            _httpClient = httpClient;
            _basicUrl = configuration["VidalApi:BasicUrl"] ?? throw new ArgumentNullException(nameof(_basicUrl));
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
            _context = context;
        }




        //public async Task<VMPModel> GetVMPById(int id)
        //{
        //    try
        //    {
        //        // Ejecutar el procedimiento almacenado Vidal_VMP para obtener el VMP por Id
        //        var vmpParam = new SqlParameter("@IdVMP", id);

        //        // Ejecutar el SP y obtener los resultados en una lista de VMPEntry
        //        var vmpEntry = await _context.VMPs
        //            .FromSqlRaw("EXEC Vidal_VMPById @IdVMP", vmpParam)
        //            .FirstOrDefaultAsync();

        //        if (vmpEntry == null)
        //        {
        //            throw new Exception($"No se encontró un VMP con el Id {id}");
        //        }

        //        return vmpEntry;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        throw new Exception("Error al obtener el VMP por Id. Por favor, intente de nuevo más tarde.");
        //    }
        //}




        public async Task<VMPEntry> GetVMPById(int id)
        {
            var requestUrl = $"{_baseUrl}/vmp/{id}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPByIdXml();
        }

        public async Task<List<VMPProductEntry>> GetProductsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/products?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPProductXml();
        }


        public async Task<List<VMPAtcClassificationEntry>> GetAtcClassificationByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/atc-classification?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPAtcClassificationXml();
        }

        public async Task<List<VMPMoleculeEntry>> GetMoleculesByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/molecules?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPMoleculeXml();
        }

        public async Task<List<VMPUnitEntry>> GetUnitsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/units?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPUnitXml();
        }

        public async Task<List<VMPContraindicationEntry>> GetContraindicationsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/contraindications?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPContraindicationXml();
        }


        public async Task<List<VMPPhysicoChemicalInteractionEntry>> GetPhysicoChemicalInteractionsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/physico-chemical-interactions?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPPhysicoChemicalInteractionXml();
        }

        public async Task<List<VMPRouteEntry>> GetRoutesByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/routes?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPRouteXml();
        }

        public async Task<List<VMPIndicatorEntry>> GetIndicatorsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/indicators?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPIndicatorXml();
        }

        public async Task<List<VMPIndicationEntry>> GetIndicationsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/indications?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPIndicationXml();
        }

        public async Task<List<VMPSideEffectEntry>> GetSideEffectsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/side-effects?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPSideEffectXml();
        }

        public async Task<List<VMPUcdvEntry>> GetUcdvsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/ucdvs?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPUcdvXml();
        }

        public async Task<List<VMPUcdEntry>> GetUcdsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/ucds?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPUcdXml();
        }

        public async Task<List<VMPAllergyEntry>> GetAllergiesByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/allergies?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            // Verificamos si la respuesta no tiene contenido
            if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()))
            {
                return new List<VMPAllergyEntry>(); // Retornamos un modelo vacío si no hay contenido
            }

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPAllergyXml();
        }


        public async Task<List<VMPDocumentEntry>> GetDocumentsByVMPId(int vmpId)
        {
            var requestUrl = $"{_baseUrl}/vmp/{vmpId}/documents?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            return xmlContent.ParseVMPDocumentXml();
        }

       

        public async Task<List<VMPUnitEntry>> GetVmpUnitsByLinkAsync(string link)
        {
            var response = await _httpClient.GetAsync($"{_basicUrl}{link}?app_id={_appId}&app_key={_appKey}");
            response.EnsureSuccessStatusCode();
            var xmlContent = await response.Content.ReadAsStringAsync();
            var vmpUnits = xmlContent.ParseVMPUnitXml();
            return vmpUnits;
        }

        public async Task<List<VmpEntry>> GetVmpByName(string name)
        {
            var vmptEntries = new List<VmpEntry>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;
            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/vmps?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var vmpPage = xmlContent.ParseVmpConsultaXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    vmptEntries.AddRange(vmpPage);

                    startPage++;
                } while (vmptEntries.Count < totalResults);

                return vmptEntries;
            }
            catch (HttpRequestException httpEx)
            {
                // Manejo de errores de HTTP
                throw new Exception("Error al obtener productos. Por favor, intente de nuevo más tarde.");
            }
            catch (XmlException xmlEx)
            {
                // Manejo de errores de XML
                throw new Exception("Error al procesar los datos del producto. Por favor, intente de nuevo más tarde.");
            }
            catch (Exception ex)
            {
                // Manejo de errores genéricos
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<List<VMPEntry>> SearchVMPsByNameAsync(string name)
        {
            var vmpEntries = new List<VMPEntry>();
            var startPage = 1;
            var pageSize = 25;
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/vmps?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var vmpPage = xmlContent.ParseVMPXml();

                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    vmpEntries.AddRange(vmpPage);

                    startPage++;
                } while (vmpEntries.Count < totalResults);

                return vmpEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error al buscar VMPs. Por favor, intente de nuevo más tarde.");
            }
        }
    }
}
