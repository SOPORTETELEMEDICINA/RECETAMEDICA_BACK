using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using RMD.Data;
using RMD.Extensions;
using RMD.Extensions.Consulta;
using RMD.Interface.Consulta;
using RMD.Interface.Notificaciones;
using RMD.Models.Consulta;
using RMD.Models.Responses;

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
        private readonly ConsultaDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        // Diccionario para rastrear si el IdPaciente ya fue validado
        private readonly Dictionary<Guid, bool> _validacionMedicamentos = new();
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        public ConsultaService(
            HttpClient httpClient,
            IConfiguration configuration,
            ConsultaDbContext context,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "BaseUrl es nulo.");
            _basicUrl = configuration["VidalApi:BasicUrl"] ?? throw new ArgumentNullException(nameof(configuration), "BasicUrl es nulo.");
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(configuration), "AppId es nulo.");
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(configuration), "AppKey es nulo.");
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(configuration), "StartPage es nulo.");
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(configuration), "SizePage es nulo.");
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        public string SerializarObjeto(object resultList)
        {
            return JsonSerializer.Serialize(resultList, _jsonSerializerOptions);
        }

        public async Task<ResponseFromService<IEnumerable<RequestSearchAllergy>>> GetAllergiesByNameAsync(string name)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_GetAllergyByName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@name", name);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<RequestSearchAllergy>>.Failure(notificacion);

                // 2) Leer segundo conjunto: lista de alergias
                var lista = new List<RequestSearchAllergy>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(RequestSearchAllergy.FromDataReader(reader));
                    }
                }

                return ResponseFromService<IEnumerable<RequestSearchAllergy>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestSearchAllergy>>.Exeption(ex, error);
            }
        }


        public async Task<ResponseFromService<IEnumerable<RequestSearchMolecules>>> GetMoleculeByNameAsync(string name)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_GetMoleculesByName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@name", name);

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<RequestSearchMolecules>>.Failure(notificacion);

                var lista = new List<RequestSearchMolecules>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(RequestSearchMolecules.FromDataReader(reader));
                    }
                }

                return ResponseFromService<IEnumerable<RequestSearchMolecules>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestSearchMolecules>>.Exeption(ex, error);
            }
        }


        public async Task<ResponseFromService<IEnumerable<RequestSearchCIM10>>> GetCIM10sByNameAsync(string name)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_GetCIM10ByName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@name", name);

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<RequestSearchCIM10>>.Failure(notificacion);

                var lista = new List<RequestSearchCIM10>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(RequestSearchCIM10.FromDataReader(reader));
                    }
                }

                return ResponseFromService<IEnumerable<RequestSearchCIM10>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestSearchCIM10>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> GetIdsFromLink(int id, string idType, string relacionType)
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
                        return ResponseFromService<string>.Failure(error);
                    }
                    do
                    {
                        var requestUrl = $"{_basicUrl}{link.Data}?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                        var response = await _httpClient.GetAsync(requestUrl);
                        response.EnsureSuccessStatusCode();
                        var xmlContent = await response.Content.ReadAsStringAsync();
                        var ids = xmlContent.ParseXmlForIds();
                        if (ids == null || ids.Count == 0)
                            break;
                        idList.AddRange(ids);
                        if (totalResults == 0)
                        {
                            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                            if (totalResults == 0)
                                break;
                        }
                        currentPage++;
                        startPage++;
                    }
                    while (idList.Count < totalResults && currentPage * pageSize < totalResults);

                    if (idList.Count == 0)
                    {
                        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                        return ResponseFromService<string>.Success("{}", success);
                    }

                    using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                    await connection.OpenAsync();
                    using var command = new SqlCommand("Vidal_Relaciones", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@RelacionType", relacionType);
                    var idTable = new DataTable();
                    idTable.Columns.Add("Id", typeof(int));
                    foreach (var idItem in idList)
                        idTable.Rows.Add(idItem);
                    var idsParam = new SqlParameter
                    {
                        ParameterName = "@Ids",
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.IdTableType",
                        Value = idTable
                    };
                    command.Parameters.Add(idsParam);
                    using var reader = await command.ExecuteReaderAsync();
                    var resultList = new List<Dictionary<string, object>>();
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            row[columnName] = reader.GetValue(i);
                        }
                        resultList.Add(row);
                    }
                    var jsonResult = JsonSerializer.Serialize(resultList, _jsonSerializerOptions);
                    var successMsg = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                    return ResponseFromService<string>.Success(jsonResult, successMsg);
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
                        if (indications == null || indications.Count == 0)
                            break;
                        indicationList.AddRange(indications);
                        if (totalResults == 0)
                        {
                            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                            if (totalResults == 0)
                                break;
                        }
                        currentPage++;
                        startPage++;
                    }
                    while (indicationList.Count < totalResults && currentPage * pageSize < totalResults);

                    if (indicationList.Count == 0)
                    {
                        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                        return ResponseFromService<string>.Success("{}", success);
                    }
                    var jsonResult = JsonSerializer.Serialize(indicationList, _jsonSerializerOptions);
                    var successMsg2 = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                    return ResponseFromService<string>.Success(jsonResult, successMsg2);
                }
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        private async Task<ResponseFromService<string>> GetLinkAsync(int id, string idType, string relacionType)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_GetLink", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@IdType", idType);
                command.Parameters.AddWithValue("@RelacionType", relacionType);

                using var reader = await command.ExecuteReaderAsync();

                // Primer conjunto: código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                // Segundo conjunto: el link
                string link = string.Empty;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    link = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                }

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
                var xmlResponse = await SendXmlToExternalApiReturnXML(xmlContent);
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                var responseObj = new PrescriptionResponseXML
                {
                    XMLResponse = xmlResponse.Data,
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
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand("Consulta_GetMedicamentoByName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ByName", name);
                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación (primer result set)
                if (!await reader.ReadAsync())
                    throw new Exception("No se obtuvo código de notificación del SP");

                var codigoNotificacion = reader.GetInt32(reader.GetOrdinal("CodigoNotificacion"));
                var notif = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                // 2) Leer datos (segundo result set)
                var medicamentos = new List<Medicamentos>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        medicamentos.Add(new Medicamentos
                        {
                            IdType = reader["IdType"].ToString(),
                            Id = Convert.ToInt32(reader["Id"]),
                            Summary = reader["Summary"].ToString(),
                            Nombre = reader["Nombre"].ToString()
                        });
                    }
                }
               
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

        private async Task<ResponseFromService<XDocument>> SendXmlToExternalApiReturnXML(string xmlContent)
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

        public async Task<ResponseFromService<Guid>> RegistrarRecetaAsync(RecetaRequestModel request, string token)
        {
            try
            {
                var idGEMP = ObtenerValorDesdeToken(token, "GEMP");
                var idSucursal = ObtenerValorDesdeToken(token, "IdSucursal");
                var idUsuario = Guid.Parse(ObtenerValorDesdeToken(token, "IdUsuario"));

                if (string.IsNullOrEmpty(idGEMP) || string.IsNullOrEmpty(idSucursal))
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<Guid>.Failure(error);
                }

                var idMedico = await _context.Medicos
                    .Where(m => m.IdUsuario == idUsuario)
                    .Select(m => m.IdMedico)
                    .FirstOrDefaultAsync();

                if (idMedico == Guid.Empty)
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<Guid>.Failure(error);
                }

                if (request.Paciente.Peso < 0 || request.Paciente.Peso > 999.99m)
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<Guid>.Failure(error);
                }

                if (request.Paciente.Talla < 0 || request.Paciente.Talla > 999.99m)
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<Guid>.Failure(error);
                }

                if (request.Paciente.Creatinina.HasValue && (request.Paciente.Creatinina < 0 || request.Paciente.Creatinina > 999.99m))
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<Guid>.Failure(error);
                }

                var nuevaReceta = new RecetaCreate
                {
                    IdReceta = Guid.NewGuid(),
                    IdMedico = idMedico,
                    IdPaciente = request.IdPaciente,
                    PacPeso = request.Paciente.Peso,
                    PacTalla = request.Paciente.Talla,
                    PacEmbarazo = request.Paciente.Embarazo,
                    PacSemAmenorrea = request.Paciente.SemanasAmenorrea ?? 0,
                    PacLactancia = request.Paciente.Lactancia,
                    PacCreatinina = request.Paciente.Creatinina ?? 0m,
                    Alergias = ConvertirListaAString(request.Paciente.Alergias),
                    Molecules = ConvertirListaAString(request.Paciente.Moleculas),
                    Patologias = ConvertirListaAString(request.Paciente.Patologias),
                    IdSucursal = Guid.Parse(idSucursal),
                    IdGEMP = Guid.Parse(idGEMP),
                    FechaCreacion = DateTime.Now,
                    FechaUltimaModificacion = DateTime.Now
                };

                var actualizarResponse = await ActualizarPacienteDesdeReceta(
                    request.IdPaciente,
                    ConvertirListaAString(request.AlergiasCronicas)?.Replace(",", ";") ?? "",
                    ConvertirListaAString(request.MoleculasCronicas)?.Replace(",", ";") ?? "",
                    ConvertirListaAString(request.PatologiasCronicas)?.Replace(",", ";") ?? "",
                    DateTime.Now
                );
    

                _context.Recetas.Add(nuevaReceta);
                await _context.SaveChangesAsync();

                foreach (var detalle in request.PrescriptionLines)
                {
                    if (detalle.PeriodoInicio > detalle.PeriodoTerminacion)
                    {
                        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                        return ResponseFromService<Guid>.Failure(error);
                    }

                    var nuevoDetalle = new DetalleReceta
                    {
                        IdDetalleReceta = Guid.NewGuid(),
                        IdReceta = nuevaReceta.IdReceta,
                        MedicamentoId = detalle.IdMedicamento,
                        MedicamentoType = detalle.TipoMedicamento,
                        UnidadDispensacionId = detalle.UnidadDispensacionId,
                        RutaAdministracionId = detalle.RutaAdministracionId,
                        CantidadDiaria = detalle.CantidadDiaria,
                        Indicacion = detalle.Indicacion ?? "",
                        IndicacionNombre = detalle.IndicacionNombre ?? "",
                        Frecuencia = detalle.Frecuencia ?? "",
                        Observaciones = detalle.Observaciones ?? "",
                        Duracion = detalle.Duracion,
                        UnidadDuracion = detalle.UnidadDuracion,
                        PeriodoInicio = detalle.PeriodoInicio,
                        PeriodoTerminacion = detalle.PeriodoTerminacion
                    };
                    _context.DetalleRecetas.Add(nuevoDetalle);
                }
                await _context.SaveChangesAsync();

                try
                {
                    var textoOriginal = $"{nuevaReceta.IdReceta}|{nuevaReceta.IdMedico}|{nuevaReceta.FechaUltimaModificacion}";
                    var textoEncriptado = EncryptionHelper.Encrypt(textoOriginal);
                    var qrCodeBase64 = QRGenerator.GenerarQR(textoEncriptado);
                    if (string.IsNullOrEmpty(qrCodeBase64))
                    {
                        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                        return ResponseFromService<Guid>.Failure(error);
                    }
                    var recetaQR = new RecetaQR
                    {
                        IdReceta = nuevaReceta.IdReceta,
                        IdPaciente = request.IdPaciente,
                        QRData = qrCodeBase64,
                        Estatus = true,
                        FechaCreacion = DateTime.Now,
                        FechaUltimaModificacion = DateTime.Now
                    };
                    _context.Set<RecetaQR>().Add(recetaQR);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // No se retorna error por problemas en la generación de QR, solo se registra el mensaje.
                    Console.WriteLine($"Error al generar QR: {ex.Message}");
                }
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<Guid>.Success(nuevaReceta.IdReceta, success);
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<Guid>.Exeption(ex, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<Guid>.Exeption(ex, error);
            }
        }

        private async Task<ResponseFromService<List<PrescriptionLineModel>>> GetMedicamentoActivoFromDatabase(Guid idPaciente)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_GetMedcamentoActivoAnalisis", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<List<PrescriptionLineModel>>.Failure(notificacion);

                // 2) Leer segundo conjunto: datos de medicamento activo
                var result = new List<PrescriptionLineModel>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Aquí delegamos al modelo la conversión del IDataRecord a PrescriptionLineModel
                        result.Add(PrescriptionLineModel.FromDataReader(reader));
                    }
                }

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

        public async Task<ResponseFromService<bool>> EliminarRecetaAsync(Guid idReceta)
        {
            try
            {
                var receta = await _context.RecetasSql.FirstOrDefaultAsync(r => r.IdReceta == idReceta);
                if (receta == null)
                {
                    var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NO_ENCONTRADO");
                    return ResponseFromService<bool>.Failure(notificacion);
                }
                receta.Estatus = 3; // 3 = Cancelada
                receta.FechaUltimaModificacion = DateTime.Now;
                await _context.SaveChangesAsync();
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<bool>.Success(true, success);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<RecetaGetRequest>> ConsultarRecetaAsync(Guid idReceta)
        {
            try
            {
                // 1) Abrir conexión y ejecutar SP
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_ConsultarReceta", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdReceta", idReceta);

                using var reader = await command.ExecuteReaderAsync();

                // 2) Leer código de notificación y obtener la notificación
                var codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 3) Si es ERROR, abortar con Failure
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<RecetaGetRequest>.Failure(notificacion);

                // 4) Avanzar al primer resultset y mapear RecetaGetSQL
                RecetaGetSQL recetaSql = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                    recetaSql = RecetaGetSQL.FromDataReader(reader);

                // 5) Avanzar al segundo resultset y mapear todos los DetalleRecetaGet
                var detalles = new List<DetalleRecetaGet>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                        detalles.Add(DetalleRecetaGet.FromDataReader(reader));
                }

                // 6) Transformar y devolver Success
                var receta = TransformRecetaSQLToRecetaGet(recetaSql);
                var payload = new RecetaGetRequest
                {
                    Receta = receta,
                    Detalles = detalles
                };

                return ResponseFromService<RecetaGetRequest>.Success(payload, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<RecetaGetRequest>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<RecetaGetRequest>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<RecetaGet>>> ObtenerRecetasPorMedicoAsync(Guid idMedico, Guid idSucursal)
        {
            try
            {
                // 1) Abrir conexión y ejecutar SP
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_RecetasPorMedico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdMedico", idMedico);
                command.Parameters.AddWithValue("@IdSucursal", idSucursal);

                using var reader = await command.ExecuteReaderAsync();

                // 2) Leer código de notificación y obtener la notificación
                var codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 3) Si es ERROR, abortar con Failure
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<RecetaGet>>.Failure(notificacion);

                // 4) Avanzar al segundo result‑set y mapear RecetaGetSQL → RecetaGet
                var lista = new List<RecetaGet>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var sqlModel = RecetaGetSQL.FromDataReader(reader);
                        lista.Add(TransformRecetaSQLToRecetaGet(sqlModel));
                    }
                }

                // 5) Devolver Success con la lista transformada
                return ResponseFromService<IEnumerable<RecetaGet>>.Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RecetaGet>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RecetaGet>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<bool>> GetTieneEventoSaludAsync(Guid idPaciente)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_PacienteGetEventoSalud", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                using var reader = await command.ExecuteReaderAsync();

                // 1) leo el código de notificación
                int codigoNotificacion = 0;
                if (await reader.ReadAsync())
                {
                    codigoNotificacion = reader.GetInt32(reader.GetOrdinal("CodigoNotificacion"));
                }

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<bool>.Failure(notificacion);
                }

                // 2) paso al segundo resultado y leo el booleano
                bool tieneEvento = false;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    tieneEvento = reader.GetBoolean(reader.GetOrdinal("Resultado"));
                }

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
                // 1) Abrimos conexión y ejecutamos el SP
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand("Consulta_PacienteGetEventoMedicamentoso", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                using var reader = await command.ExecuteReaderAsync();

                // 2) Leemos el código de notificación
                var codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // Si vino un ERROR, devolvemos directamente la failure
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<bool>.Failure(notificacion);
                }

                // 3) Saltamos al segundo conjunto y leemos el bit
                bool tieneEvento = false;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    tieneEvento = reader.GetBoolean(reader.GetOrdinal("Resultado"));
                }

                // 4) Retornamos el resultado junto con la notificación de éxito
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
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_SucursalesPorUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<SucursalModel>>.Failure(notificacion);
                }

                // 2) Leer segundo conjunto: sucursales
                var lista = new List<SucursalModel>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(SucursalModel.FromDataReader(reader));
                    }
                }

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

        // Helper Methods

        private async Task<ResponseFromService<bool>> ActualizarPacienteDesdeReceta(Guid idPaciente, string alergias, string molecules, string patologias, DateTime fechaUltimaModificacion)
        {
            try
            {
                var pacienteExistente = await _context.Pacientes
                    .Where(p => p.IdPaciente == idPaciente)
                    .FirstOrDefaultAsync();
                if (pacienteExistente != null)
                {
                    pacienteExistente.Alergias = string.IsNullOrEmpty(alergias) ? "" : alergias;
                    pacienteExistente.Molecules = string.IsNullOrEmpty(molecules) ? "" : molecules;
                    pacienteExistente.Patologias = string.IsNullOrEmpty(patologias) ? "" : patologias;
                    pacienteExistente.FechaUltimaModificacion = fechaUltimaModificacion;
                    await _context.SaveChangesAsync();
                }
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<bool>.Success(true, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        private static string ObtenerValorDesdeToken(string token, string claimType)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value ?? string.Empty;
        }

        private static string ConvertirListaAString(List<int> lista)
        {
            return string.Join(",", lista);
        }

        private static List<RequestSearchAllergy?> ParseAllergies(string allergiesString)
        {
            try
            {
                return allergiesString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a =>
                    {
                        var parts = a.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                            return null;
                        return new RequestSearchAllergy
                        {
                            IdAllergy = int.Parse(parts[0].Trim()),
                            NameAllergy = parts[1].Trim()
                        };
                    })
                    .Where(x => x != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing allergies: {ex}");
                return new List<RequestSearchAllergy?>();
            }
        }

        private static List<RequestSearchMolecules?> ParseMolecules(string moleculesString)
        {
            try
            {
                return moleculesString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(m =>
                    {
                        var parts = m.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                            return null;
                        return new RequestSearchMolecules
                        {
                            IdMolecule = int.Parse(parts[0].Trim()),
                            NameMolecule = parts[1].Trim()
                        };
                    })
                    .Where(x => x != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing molecules: {ex}");
                return new List<RequestSearchMolecules?>();
            }
        }

        private static List<ListIM10?> ParseCIM10(string cim10String)
        {
            try
            {
                return cim10String.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(c =>
                    {
                        var parts = c.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 3)
                            return null;
                        return new ListIM10
                        {
                            IdCIM10 = int.Parse(parts[0].Trim()),
                            NameCIM10 = parts[1].Trim(),
                            Code = parts[2].Trim()
                        };
                    })
                    .Where(x => x != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing CIM10: {ex}");
                return new List<ListIM10?>();
            }
        }

        private static RecetaGet TransformRecetaSQLToRecetaGet(RecetaGetSQL receta)
        {
            return new RecetaGet
            {
                IdReceta = receta.IdReceta,
                Folio = receta.Folio,
                IdMedico = receta.IdMedico,
                NombresMedico = receta.NombresMedico,
                PrimerApellidoMedico = receta.PrimerApellidoMedico,
                SegundoApellidoMedico = receta.SegundoApellidoMedico,
                Universidad = receta.Universidad,
                CedulaGeneral = receta.CedulaGeneral,
                Especialidad = receta.Especialidad,
                CedulaEspecialidad = receta.CedulaEspecialidad,
                IdPaciente = receta.IdPaciente,
                NombresPaciente = receta.NombresPaciente,
                PrimerApellidoPaciente = receta.PrimerApellidoPaciente,
                SegundoApellidoPaciente = receta.SegundoApellidoPaciente,
                IdTipoIdentificacion = receta.IdTipoIdentificacion,
                TipoIdentificacion = receta.TipoIdentificacion,
                NumeroIdentificacion = receta.NumeroIdentificacion,
                FechaNacimientoPaciente = receta.FechaNacimientoPaciente,
                PacPeso = receta.PacPeso,
                PacTalla = receta.PacTalla,
                PacEmbarazo = receta.PacEmbarazo,
                PacSemAmenorrea = receta.PacSemAmenorrea,
                PacLactancia = receta.PacLactancia,
                PacCreatinina = receta.PacCreatinina,
                Alergias = string.IsNullOrEmpty(receta.Alergias) ? new List<RequestSearchAllergy>() : ParseAllergies(receta.Alergias),
                Molecules = string.IsNullOrEmpty(receta.Molecules) ? new List<RequestSearchMolecules>() : ParseMolecules(receta.Molecules),
                Patologias = string.IsNullOrEmpty(receta.Patologias) ? new List<ListIM10>() : ParseCIM10(receta.Patologias),
                IdSucursal = receta.IdSucursal,
                Sucursal = receta.Sucursal,
                IdGEMP = receta.IdGEMP,
                GrupoEmpresarial = receta.GrupoEmpresarial,
                FechaCreacion = receta.FechaCreacion,
                FechaUltimaModificacion = receta.FechaUltimaModificacion,
                Estatus = receta.Estatus,
                Descripcion = receta.Descripcion,
                Timbrada = receta.Timbrada
            };
        }

        public async Task<ResponseFromService<Guid?>> ObtenerIdMedicoPorUsuarioAsync(Guid idUsuario)
        {
            try
            {
                var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.IdUsuario == idUsuario);
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<Guid?>.Success(medico?.IdMedico, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<Guid?>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<RecetaCreate?>> ObtenerRecetaPorIdAsync(Guid idReceta)
        {
            try
            {
                var receta = await _context.Recetas.FirstOrDefaultAsync(r => r.IdReceta == idReceta);
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<RecetaCreate?>.Success(receta, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<RecetaCreate?>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<DetalleRecetaResponse>>> GetReaccionMedicamentoPrevioAsync(Guid idPaciente)
        {
            try
            {
                await using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("DetalleReceta_GetReaccionesPaciente", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                await using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                if (!await reader.ReadAsync())
                    throw new InvalidOperationException("No se obtuvo código de notificación desde el SP.");

                var codigoNotificacion = reader.GetInt32(reader.GetOrdinal("CodigoNotificacion"));
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<DetalleRecetaResponse>>.Failure(notificacion);

                // 2) Leer segundo result set: detalles de reacciones
                var reaccionesPrevias = new List<DetalleRecetaResponse>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reaccionesPrevias.Add(new DetalleRecetaResponse
                        {
                            IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
                            IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                            IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                            MedicamentoId = reader.GetInt32(reader.GetOrdinal("MedicamentoId")),
                            MedicamentoType = reader.GetString(reader.GetOrdinal("MedicamentoType")),
                            Medicamento = reader.GetString(reader.GetOrdinal("Medicamento")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"))
                        });
                    }
                }

                return ResponseFromService<IEnumerable<DetalleRecetaResponse>>.Success(reaccionesPrevias, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<DetalleRecetaResponse>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<DetalleRecetaResponse>>.Exeption(ex, error);
            }
        }
    }
}



