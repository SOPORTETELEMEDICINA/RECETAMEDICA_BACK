using Dapper;
using Microsoft.Data.SqlClient;
using RMD.Extensions;
using RMD.Extensions.Consulta;
using RMD.Interface.Consulta;
using RMD.Interface.Security;
using RMD.Shared.Models.Consulta;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace RMD.Service.Consulta
{
    public class ConsultaService : IConsultaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _basicUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;

        // Diccionario para rastrear si el IdPaciente ya fue validado
        private readonly Dictionary<Guid, bool> _validacionMedicamentos = new();

        public ConsultaService(
            HttpClient httpClient,
            IConfiguration configuration,
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService,
            byte[] commonKey)
        {
            _httpClient = httpClient;
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;

            var country = configuration["Country"]?.ToUpper() ?? "MX";
            var prefix = country == "ES" ? "VidalApiEs" : "VidalApi";

            var _urlBase = configuration[$"{prefix}:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "BaseUrl es nulo.");
            // Quita la última diagonal si existe
            _baseUrl = _urlBase.EndsWith("/")
                ? _urlBase.Substring(0, _urlBase.Length - 1)
                : _urlBase;
            _basicUrl = configuration[$"{prefix}:BasicUrl"] ?? throw new ArgumentNullException(nameof(configuration), "BasicUrl es nulo.");

            var encryptedAppId = configuration[$"{prefix}:AppId"] ?? throw new ArgumentNullException(nameof(configuration), "AppId es nulo.");
            var encryptedAppKey = configuration[$"{prefix}:AppKey"] ?? throw new ArgumentNullException(nameof(configuration), "AppKey es nulo.");
            _appId = EncryptionHelper.Decrypt(encryptedAppId, commonKey);
            _appKey = EncryptionHelper.Decrypt(encryptedAppKey, commonKey);

            _startPage = configuration[$"{prefix}:StartPage"] ?? throw new ArgumentNullException(nameof(configuration), "StartPage es nulo.");
            _pageSize = configuration[$"{prefix}:SizePage"] ?? throw new ArgumentNullException(nameof(configuration), "SizePage es nulo.");
        }


        public async Task<ResponseFromService<object>> GetIdsFromLink(int id, string idType, string relacionType)
        {
            try
            {
                if (relacionType != "INDICATIONS")
                {
                    var idList = new List<int>();
                    int startPage = int.Parse(_startPage);
                    int pageSize = int.Parse(_pageSize);
                    int totalResults = 0;
                    int currentPage = 0;
                    var link = await GetLinkAsync(id, idType, relacionType);

                    if (link.Toast == "ERROR")
                    {
                        var error = await _catalogoNotificacionService.GetNotificationByCodeAsync(link.Code);
                        return ResponseFromService<object>.Failure(error);
                    }

                    do
                    {
                        var requestUrl = $"{_basicUrl}{link.Data}?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                        var response = await _httpClient.GetAsync(requestUrl);
                        response.EnsureSuccessStatusCode();

                        var xmlContent = await response.Content.ReadAsStringAsync();
                        var ids = xmlContent.ParseXmlForIds();

                        if (ids.Count == 0) break;

                        idList.AddRange(ids);

                        if (totalResults == 0)
                        {
                            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                            if (totalResults == 0) break;
                        }

                        currentPage++;
                        startPage++;

                    } while (idList.Count < totalResults && currentPage * pageSize < totalResults);

                    if (idList.Count == 0)
                    {
                        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                        return ResponseFromService<object>.Success("{}", success);
                    }

                    var table = new DataTable();
                    table.Columns.Add("Id", typeof(int));
                    foreach (var item in idList)
                        table.Rows.Add(item);

                    var multi = await _dapperService.QueryMultipleAsync(
                        "[Vidal].[GetRelaciones]",
                        new
                        {
                            Id = id,
                            RelacionType = relacionType,
                            Ids = table.AsTableValuedParameter("dbo.IdTableType")
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    var codigoNotificacion = multi.ReadFirstOrDefault<int>();
                    var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                    if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                        return ResponseFromService<object>.Failure(notificacion);

                    if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                        return ResponseFromService<object>.Success(new object(), notificacion);

                    var resultList = multi.Read().Select(r => (IDictionary<string, object>)r).ToList();
                    //var json = JsonSerializer.Serialize(resultList, _jsonSerializerOptions);

                    return ResponseFromService<object>.Success(resultList, notificacion);
                }
                else
                {
                    var indicationList = new List<IndicationModel>();
                    int startPage = int.Parse(_startPage);
                    int pageSize = int.Parse(_pageSize);
                    int totalResults = 0;
                    int currentPage = 0;

                    var link = await GetLinkAsync(id, idType, relacionType);

                    do
                    {
                        var requestUrl = $"{_basicUrl}{link.Data}?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                        var response = await _httpClient.GetAsync(requestUrl);
                        response.EnsureSuccessStatusCode();

                        var xmlContent = await response.Content.ReadAsStringAsync();
                        var indications = xmlContent.ParseXmlForIndications();

                        if (indications.Count == 0) break;

                        indicationList.AddRange(indications);

                        if (totalResults == 0)
                        {
                            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                            if (totalResults == 0) break;
                        }

                        currentPage++;
                        startPage++;

                    } while (indicationList.Count < totalResults && currentPage * pageSize < totalResults);

                    if (indicationList.Count == 0)
                    {
                        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                        return ResponseFromService<object>.Success("{}", success);
                    }

                    //var json = JsonSerializer.Serialize(indicationList, _jsonSerializerOptions);
                    var successMsg = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                    return ResponseFromService<object>.Success(indicationList, successMsg);
                }
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<object>.Exeption(ex, error);
            }
        }

        private async Task<ResponseFromService<string>> GetLinkAsync(int id, string idType, string relacionType)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Consulta_GetLink",
                    new
                    {
                        Id = id,
                        IdType = idType,
                        RelacionType = relacionType
                    },
                    commandType: CommandType.StoredProcedure
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<string>.Success(string.Empty, notificacion);

                string link = multi.Read<string>().FirstOrDefault() ?? string.Empty;
                return ResponseFromService<string>.Success(link, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<PrescriptionResponseHTML>> ProcessPrescriptionRequest(PrescriptionModel request)
        {
            try
            {
                var idPaciente = request.Patient.IdPaciente;

                // Obtener los medicamentos activos si aún no se han validado.
                if (!_validacionMedicamentos.TryGetValue(idPaciente, out bool validado) || !validado)
                {
                    if (request.MedicamentoActivo == null || !request.MedicamentoActivo.Any())
                    {
                        var medResponse = await GetMedicamentoActivoFromDatabase(idPaciente);
                        if (medResponse.Toast.ToUpperInvariant() == "ERROR")
                        {
                            var error = await _catalogoNotificacionService.GetNotificationByCodeAsync(medResponse.Code);
                            return ResponseFromService<PrescriptionResponseHTML>.Failure(error);
                        }
                        request.MedicamentoActivo = medResponse.Data;
                        _validacionMedicamentos[idPaciente] = true;
                    }
                }
                var xmlContent = request.ParseToXml();
                if (!ValidateXmlContent(xmlContent))
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<PrescriptionResponseHTML>.Failure(error);
                }
                var htmlResponse = await SendXmlToExternalApi(xmlContent);
                _validacionMedicamentos.Remove(idPaciente);
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                var responseObj = new PrescriptionResponseHTML
                {
                    HtmlResponse = htmlResponse.Data,
                    MedicamentoActivo = request.MedicamentoActivo
                };
                return ResponseFromService<PrescriptionResponseHTML>.Success(responseObj, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PrescriptionResponseHTML>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<PrescriptionResponseXML>> ProcessPrescriptionXMLRequest(PrescriptionModel request)
        {
            try
            {
                var idPaciente = request.Patient.IdPaciente;
                if (!_validacionMedicamentos.TryGetValue(idPaciente, out bool validado) || !validado)
                {
                    if (request.MedicamentoActivo == null || !request.MedicamentoActivo.Any())
                    {
                        var medResponse = await GetMedicamentoActivoFromDatabase(idPaciente);
           
                        request.MedicamentoActivo = medResponse.Data;
                        _validacionMedicamentos[idPaciente] = true;
                    }
                }
                var xmlContent = request.ParseToXml();
                if (!ValidateXmlContent(xmlContent))
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<PrescriptionResponseXML>.Failure(error);
                }
                var xmlResponse = await SendXmlToExternalApiReturnXml(xmlContent);
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                var responseObj = new PrescriptionResponseXML
                {
                    XMLResponse = xmlResponse.Data.ToString(SaveOptions.None),
                    MedicamentoActivo = request.MedicamentoActivo
                };

                return ResponseFromService<PrescriptionResponseXML>.Success(responseObj, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PrescriptionResponseXML>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<Medicamentos>>> GetMedicamentoByNameAsync(string name)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Consulta].[Get_MedicamentoByName]",
                    new { ByName = name },
                    commandType: CommandType.StoredProcedure
                );


                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<Medicamentos>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<Medicamentos>>.Success(new List<Medicamentos>(), notificacion);
                // 2) Leer medicamentos (segundo result set)
                var medicamentos = multi.Read<Medicamentos>().ToList();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<IEnumerable<Medicamentos>>.Success(medicamentos, success);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<Medicamentos>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<Medicamentos>>.Exeption(ex, error);
            }
        }
        private static bool ValidateXmlContent(string xmlContent)
        {
            try
            {
                var xmlDoc = XDocument.Parse(xmlContent);
                var rootElement = xmlDoc.Root;
                if (rootElement == null || rootElement.Name != "prescription")
                    return false;
                var patientElement = rootElement.Element("patient");
                if (patientElement == null)
                    return false;
                var prescriptionLinesElement = rootElement.Element("prescription-lines");
                if (prescriptionLinesElement == null || !prescriptionLinesElement.Elements("prescription-line").Any())
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async Task<ResponseFromService<string>> SendXmlToExternalApi(string xmlContent)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/alerts/full/html";
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                requestMessage.Headers.Add("app_id", _appId);
                requestMessage.Headers.Add("app_key", _appKey);
                requestMessage.Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml");
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return ResponseFromService<string>.Success(responseContent,
                    await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA"));
            }
            catch (HttpRequestException httpEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.HttpRequestException(httpEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        private async Task<ResponseFromService<XDocument>> SendXmlToExternalApiReturnXml(string xmlContent)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/alerts/full";
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                requestMessage.Headers.Add("app_id", _appId);
                requestMessage.Headers.Add("app_key", _appKey);
                requestMessage.Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml");
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var xmlResponse = XDocument.Parse(responseContent);
                return ResponseFromService<XDocument>.Success(xmlResponse,
                    await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA"));
            }
            catch (HttpRequestException httpEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<XDocument>.HttpRequestException(httpEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<XDocument>.Exeption(ex, error);
            }
        }
        private async Task<ResponseFromService<List<PrescriptionLineModel>>> GetMedicamentoActivoFromDatabase(Guid idPaciente)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Consulta].[Get_MedcamentoActivoAnalisis]",
                    new { IdPaciente = idPaciente },
                    commandType: CommandType.StoredProcedure
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<List<PrescriptionLineModel>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<List<PrescriptionLineModel>>.Success(new List<PrescriptionLineModel>(), notificacion);

                var result = multi.Read<PrescriptionLineModel>().ToList();

                return ResponseFromService<List<PrescriptionLineModel>>.Success(result, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<PrescriptionLineModel>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<PrescriptionLineModel>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<bool>> GetTieneEventoSaludAsync(Guid idPaciente)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Consulta_PacienteGetEventoSalud",
                    new { IdPaciente = idPaciente },
                    commandType: CommandType.StoredProcedure
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<bool>.Success(false, notificacion);

                bool tieneEvento = multi.ReadFirstOrDefault<bool>();

                return ResponseFromService<bool>.Success(tieneEvento, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<bool>> GetTieneEventoMedicamentosoAsync(Guid idPaciente)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Consulta].[GetEventoMedicamentosoByIdPaciente]",
                    new { IdPaciente = idPaciente },
                    commandType: CommandType.StoredProcedure
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<bool>.Success(false, notificacion);

                bool tieneEvento = multi.ReadFirstOrDefault<bool>();

                return ResponseFromService<bool>.Success(tieneEvento, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<SucursalModel>>> ObtenerSucursalesPorUsuarioAsync(Guid idUsuario)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Consulta_SucursalesPorUsuario",
                    new { IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<SucursalModel>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<SucursalModel>>.Success(new List<SucursalModel>(), notificacion);

                var lista = multi.Read<SucursalModel>().ToList();

                return ResponseFromService<IEnumerable<SucursalModel>>.Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<SucursalModel>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<SucursalModel>>.Exeption(ex, error);
            }
        }
    }
}



