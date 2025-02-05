using RMD.Data;
using RMD.Extensions;
using RMD.Extensions.Consulta;
using RMD.Interface.Consulta;
using RMD.Models.Consulta;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace RMD.Service.Consulta
{
    public class ConsultaService(HttpClient httpClient, IConfiguration configuration, ConsultaDbContext context) : IConsultaService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _baseUrl = configuration["VidalApi:BaseUrl"] ??
                throw new ArgumentNullException(nameof(configuration), "BaseUrl es nulo.");
        private readonly string _basicUrl = configuration["VidalApi:BasicUrl"] ??
                throw new ArgumentNullException(nameof(configuration), "BasicUrl es nulo.");
        private readonly string _appId = configuration["VidalApi:AppId"] ??
                throw new ArgumentNullException(nameof(configuration), "AppId es nulo.");
        private readonly string _appKey = configuration["VidalApi:AppKey"] ??
                throw new ArgumentNullException(nameof(configuration), "AppKey es nulo.");
        private readonly string _startPage = configuration["VidalApi:StartPage"] ??
                throw new ArgumentNullException(nameof(configuration), "StartPage es nulo.");
        private readonly string _pageSize = configuration["VidalApi:SizePage"] ??
                throw new ArgumentNullException(nameof(configuration), "SizePage es nulo.");
        private readonly ConsultaDbContext _context = context;
        // Diccionario para rastrear si el IdPaciente ya fue validado
        private readonly Dictionary<Guid, bool> _validacionMedicamentos = new();
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

        //public async Task<IEnumerable<RequestSearchVMP>> GetVMPByNameAsync(string name)
        //{
        //    var nameParam = new SqlParameter("@name", name);

        //    try
        //    {
        //        var vmps = await _context.VMPS
        //            .FromSqlRaw("EXEC Consulta_GetVMPByName @name", nameParam)
        //            .ToListAsync();

        //        if (vmps == null || vmps.Count == 0)
        //        {
        //            return new List<RequestSearchVMP>(); // Devuelve una lista vacía si no se encuentran resultados
        //        }

        //        return vmps;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
        //    }
        //}

        //public async Task<IEnumerable<RequestSearchProducts>> GetProductsByNameAsync(string name)
        //{
        //    var nameParam = new SqlParameter("@name", name);

        //    try
        //    {
        //        var products = await _context.Products
        //            .FromSqlRaw("EXEC Consulta_GetProductsByName @name", nameParam)
        //            .ToListAsync();

        //        if (products == null || products.Count == 0)
        //        {
        //            return new List<RequestSearchProducts>(); // Devuelve una lista vacía si no se encuentran resultados
        //        }

        //        return products;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
        //    }
        //}

        //public async Task<IEnumerable<RequestSearchPackage>> GetPackagesByNameAsync(string name)
        //{
        //    var nameParam = new SqlParameter("@name", name);

        //    try
        //    {
        //        var packages = await _context.Packages
        //            .FromSqlRaw("EXEC Consulta_GetPackagesByName @name", nameParam)
        //            .ToListAsync();

        //        if (packages == null || packages.Count == 0)
        //        {
        //            return new List<RequestSearchPackage>(); // Devuelve una lista vacía si no se encuentran resultados
        //        }

        //        return packages;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error al obtener las alergias por nombre: {ex.Message}");
        //    }
        //}

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

        public async Task<string> GetIdsFromLink(int id, string IdType, string RelacionType)
        {

            var idList = new List<int>();
            var indicationList = new List<IndicationModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;
            var currentPage = 0;
            string link = await GetLink( id, IdType, RelacionType);


            try
            {
                if (RelacionType != "INDICATIONS")
                {
                    do
                    {
                        // Construir la URL con los parámetros de autenticación
                        var requestUrl = $"{_basicUrl}{link}?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";

                        // Hacer la solicitud HTTP
                        var response = await _httpClient.GetAsync(requestUrl);
                        response.EnsureSuccessStatusCode();

                        // Obtener el contenido XML de la respuesta
                        var xmlContent = await response.Content.ReadAsStringAsync();

                        // Parsear el contenido XML para obtener los IDs (solo pasamos el XML)
                        var ids = xmlContent.ParseXmlForIds(); // Enviamos el XML al método de parsing

                        if (ids == null || !ids.Any())
                        {
                            break; // Si no hay IDs, detenemos el ciclo
                        }

                        idList.AddRange(ids);

                        // Solo obtenemos el totalResults una vez
                        if (totalResults == 0)
                        {
                            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                            if (totalResults == 0)
                            {
                                break;
                            }
                        }

                        currentPage++;
                        startPage++;

                    } while (idList.Count < totalResults && currentPage * pageSize < totalResults);

                    // Si la lista de IDs está vacía, retornar un JSON vacío
                    if (!idList.Any())
                    {
                        return "{}";  // JSON vacío
                    }

                    // Si hay IDs en la lista, llamamos al SP Vidal_Relaciones
                    using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                    await connection.OpenAsync();

                    var command = new SqlCommand("Vidal_Relaciones", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetro para Id
                    command.Parameters.AddWithValue("@Id", id);

                    // Parámetro para IdType
                    // command.Parameters.AddWithValue("@IdType", idType);

                    command.Parameters.AddWithValue("@RelacionType", RelacionType);
                    // Parámetro para la lista de IDs, convertimos la lista en un DataTable
                    var idTable = new DataTable();
                    idTable.Columns.Add("Id", typeof(int));
                    foreach (var idItem in idList)
                    {
                        idTable.Rows.Add(idItem);
                    }

                    var idsParam = new SqlParameter
                    {
                        ParameterName = "@Ids",
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "dbo.IdTableType", // Asegúrate de crear el Table Type en SQL
                        Value = idTable
                    };
                    command.Parameters.Add(idsParam);


                    // Leer los resultados del SP y convertirlos a JSON
                    using var reader = await command.ExecuteReaderAsync();
                    var resultList = new List<Dictionary<string, object>>();
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // Usar el nombre de la columna en lugar de confiar en el orden
                            var columnName = reader.GetName(i);
                            row[columnName] = reader.GetValue(i);
                        }
                        resultList.Add(row);
                    }
                    // Convertir la lista a JSON
                    return JsonSerializer.Serialize(resultList, new JsonSerializerOptions { WriteIndented = true });
                }
                else
                {
                    do
                    {
                        // Construir la URL con los parámetros de autenticación
                        var requestUrl = $"{_basicUrl}{link}?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";

                        // Hacer la solicitud HTTP
                        var response = await _httpClient.GetAsync(requestUrl);
                        response.EnsureSuccessStatusCode();

                        // Obtener el contenido XML de la respuesta
                        var xmlContent = await response.Content.ReadAsStringAsync();

                        // Parsear el contenido XML para obtener los IDs (solo pasamos el XML)
                        var indication = xmlContent.ParseXmlForIndications(); // Enviamos el XML al método de parsing

                        if (indication == null || !indication.Any())
                        {
                            break; // Si no hay IDs, detenemos el ciclo
                        }

                        indicationList.AddRange(indication);

                        // Solo obtenemos el totalResults una vez
                        if (totalResults == 0)
                        {
                            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                            if (totalResults == 0)
                            {
                                break;
                            }
                        }

                        currentPage++;
                        startPage++;

                    } while (indicationList.Count < totalResults && currentPage * pageSize < totalResults);

                    // Si la lista de IDs está vacía, retornar un JSON vacío
                    if (!indicationList.Any())
                    {
                        return "{}";  // JSON vacío
                    }
                    // Convertir la lista a JSON
                    return JsonSerializer.Serialize(indicationList, new JsonSerializerOptions { WriteIndented = true });
                }               
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los IDs desde el enlace: {ex.Message}");
            }
           
        }

        private async Task<string> GetLink(int id, string idType, string relacionType)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_GetLink", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetro para Id
                command.Parameters.AddWithValue("@Id", id);

                // Parámetro para IdType
                command.Parameters.AddWithValue("@IdType", idType);

                // Parámetro para RelacionType
                command.Parameters.AddWithValue("@RelacionType", relacionType);

                // Ejecutar el SP y obtener el resultado (string)
                var result = await command.ExecuteScalarAsync();

                // Retornar el resultado como string
                return result?.ToString();
            }
            catch (SqlException sqlEx)
            {
                // Manejo específico de excepciones SQL
                throw new Exception("Error en la ejecución de la consulta SQL", sqlEx);
            }
            catch (Exception ex)
            {
                // Manejo genérico de excepciones
                throw new Exception("Error en la obtención del link", ex);
            }
        }

        //public async Task<List<PackageEntry>> GetPackagesByName(string name)
        //{
        //    var packageEntries = new List<PackageEntry>();
        //    int startPage = int.Parse(_startPage);
        //    int pageSize = int.Parse(_pageSize);
        //    var totalResults = 0;

        //    try
        //    {
        //        do
        //        {
        //            var requestUrl = $"{_baseUrl}/packages?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
        //            var response = await _httpClient.GetAsync(requestUrl);

        //            response.EnsureSuccessStatusCode();

        //            var xmlContent = await response.Content.ReadAsStringAsync();
        //            var packagePage = xmlContent.ParsePackageConsultaXml();

        //            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
        //            packageEntries.AddRange(packagePage);

        //            startPage++;
        //        } while (packageEntries.Count < totalResults);

        //        return packageEntries;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error inesperado: {ex.Message}");
        //        throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
        //    }
        //}


        //public async Task<List<Cim10Entry>> GetCim10ByName(string name)
        //{
        //    var cim10Entries = new List<Cim10Entry>();
        //    int startPage = int.Parse(_startPage);
        //    int pageSize = int.Parse(_pageSize);
        //    var totalResults = 0;

        //    try
        //    {
        //        do
        //        {
        //            var requestUrl = $"{_baseUrl}/pathologies?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
        //            var response = await _httpClient.GetAsync(requestUrl);

        //            response.EnsureSuccessStatusCode();

        //            var xmlContent = await response.Content.ReadAsStringAsync();
        //            var cim10Page = xmlContent.ParseCim10Xml();

        //            // Obtener información de paginación
        //            totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

        //            cim10Entries.AddRange(cim10Page);

        //            startPage++;
        //        } while (cim10Entries.Count < totalResults);

        //        Debug.WriteLine("Procesamiento de CIM10 completo.");
        //        return cim10Entries;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error inesperado: {ex.Message}");
        //        throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
        //    }
        //}       
        public async Task<PrescriptionResponseHTML> ProcessPrescriptionRequest(PrescriptionModel request)
        {
            try
            {
                var idPaciente = request.Patient.IdPaciente; // Obtener IdPaciente del modelo

                // Verificar si el paciente ya fue validado
                if (!_validacionMedicamentos.ContainsKey(idPaciente) || !_validacionMedicamentos[idPaciente])
                {
                    if (request.MedicamentoActivo == null || !request.MedicamentoActivo.Any())
                    {
                        // Consultar medicamentos activos desde la base de datos
                        request.MedicamentoActivo = await GetMedicamentoActivoFromDatabase(idPaciente);

                        // Marcar al paciente como validado
                        _validacionMedicamentos[idPaciente] = true;
                    }
                }

                // Convertir el modelo a XML utilizando la función de extensión
                var xmlContent = request.ParseToXml();

                // Validar el contenido del XML
                if (!ValidateXmlContent(xmlContent))
                {
                    throw new Exception("El XML generado no es válido.");
                }

                // Enviar el XML al endpoint externo
                var htmlResponse = await SendXmlToExternalApi(xmlContent);

                // Eliminar la clave del diccionario al finalizar
                _validacionMedicamentos.Remove(idPaciente);

                // Retornar la respuesta y MedicamentoActivo
                return new PrescriptionResponseHTML
                {
                    HtmlResponse = htmlResponse,
                    MedicamentoActivo = request.MedicamentoActivo
                };
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.", ex);
            }
        }

        //public async Task<PrescriptionResponseHTML> ProcessPrescriptionRequest(PrescriptionModel request)
        //{
        //    try
        //    {

        //        var idPaciente = request.Patient.IdPaciente; // Obtener IdPaciente del modelo

        //        // Verificar si el paciente ya fue validado
        //        if (!_validacionMedicamentos.ContainsKey(idPaciente) || !_validacionMedicamentos[idPaciente])
        //        {
        //            if (request.MedicamentoActivo == null || !request.MedicamentoActivo.Any())
        //            {
        //                // Consultar medicamentos activos desde la base de datos
        //                request.MedicamentoActivo = await GetMedicamentoActivoFromDatabase(idPaciente);

        //                // Marcar al paciente como validado
        //                _validacionMedicamentos[idPaciente] = true;
        //            }
        //        }

        //        // Convertir el modelo a XML utilizando la función de extensión
        //        var xmlContent = request.ParseToXml();

        //        // Validar el contenido del XML
        //        if (!ValidateXmlContent(xmlContent))
        //        {
        //            throw new Exception("El XML generado no es válido.");
        //        }

        //        // Enviar el XML al endpoint externo

        //        // Enviar el XML al endpoint externo
        //        var htmlResponse = await SendXmlToExternalApi(xmlContent);

        //        // Retornar la respuesta y MedicamentoActivo
        //        return new PrescriptionResponseHTML
        //        {
        //            HtmlResponse = htmlResponse,
        //            MedicamentoActivo = request.MedicamentoActivo
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejo de errores
        //        throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.", ex);
        //    }
        //}

        public async Task<PrescriptionResponseXML> ProcessPrescriptionXMLRequest(PrescriptionModel request)
        {
            try
            {
                var idPaciente = request.Patient.IdPaciente; // Obtener IdPaciente del modelo

                // Verificar si el paciente ya fue validado
                if (!_validacionMedicamentos.ContainsKey(idPaciente) || !_validacionMedicamentos[idPaciente])
                {
                    if (request.MedicamentoActivo == null || !request.MedicamentoActivo.Any())
                    {
                        // Consultar medicamentos activos desde la base de datos
                        request.MedicamentoActivo = await GetMedicamentoActivoFromDatabase(idPaciente);

                        // Marcar al paciente como validado
                        _validacionMedicamentos[idPaciente] = true;
                    }
                }


                // Convertir el modelo a XML utilizando la función de extensión
                var xmlContent = request.ParseToXml();

                // Validar el contenido del XML
                if (!ValidateXmlContent(xmlContent))
                {
                    throw new Exception("El XML generado no es válido.");
                }

                // Enviar el XML al endpoint externo
                var xmlResponse = await SendXmlToExternalApiReturnXML(xmlContent);
                return new PrescriptionResponseXML
                {
                    XMLResponse = xmlResponse,
                    MedicamentoActivo = request.MedicamentoActivo
                };
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.", ex);
            }
        }

        public async Task<IEnumerable<Medicamentos>> GetMedicamentoByNameAsync(string name)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_GetMedicamentoByName", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetro para nombre
                command.Parameters.AddWithValue("@ByName", name);

                // Ejecutar el SP y leer los resultados
                using var reader = await command.ExecuteReaderAsync();
                var medicamentos = new List<Medicamentos>();

                while (await reader.ReadAsync())
                {
                    var medicamento = new Medicamentos
                    {
                        IdType = reader["IdType"].ToString(),
                        Id = Convert.ToInt32(reader["Id"]),
                        Summary = reader["Summary"].ToString(),
                        Nombre = reader["Nombre"].ToString()
                    };

                    medicamentos.Add(medicamento);
                }

                return medicamentos;
            }
            catch (SqlException sqlEx)
            {
                // Manejo de excepciones específicas de SQL
                throw new Exception("Error en la ejecución del procedimiento almacenado.", sqlEx);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones genéricas
                throw new Exception("Se produjo un error al obtener los medicamentos.", ex);
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


        private async Task<XDocument> SendXmlToExternalApiReturnXML(string xmlContent)
        {
            try
            {
                var requestUrl = $"{_baseUrl}/alerts/full";

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                requestMessage.Headers.Add("app_id", _appId);
                requestMessage.Headers.Add("app_key", _appKey);
                requestMessage.Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml");
                Console.WriteLine(xmlContent); // Para depuración, puedes eliminarlo en producción

                var response = await _httpClient.SendAsync(requestMessage);

                // Asegúrate de que la respuesta sea exitosa
                response.EnsureSuccessStatusCode();

                // Leer la respuesta del servidor como string
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parsear la respuesta a XDocument
                var xmlResponse = XDocument.Parse(responseContent);

                return xmlResponse;
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

        public async Task<Guid> RegistrarRecetaAsync(RecetaRequestModel request, string token)
        {
            // Extraer los valores del token
            var idGEMP = ObtenerValorDesdeToken(token, "GEMP");
            var idSucursal = ObtenerValorDesdeToken(token, "IdSucursal");
            var idUsuario = Guid.Parse(ObtenerValorDesdeToken(token, "IdUsuario"));

            if (string.IsNullOrEmpty(idGEMP) || string.IsNullOrEmpty(idSucursal))
            {
                throw new Exception("El token no contiene los valores necesarios.");
            }
            // Buscar el IdMedico correspondiente al IdUsuario
            var idMedico = await _context.Medicos
                .Where(m => m.IdUsuario == idUsuario)
                .Select(m => m.IdMedico)
                .FirstOrDefaultAsync();

            if (idMedico == Guid.Empty)
            {
                throw new Exception($"No se encontró un médico con el IdUsuario '{idUsuario}'.");
            }
            // Validar datos obligatorios y rangos
            if (request.Paciente.Peso < 0 || request.Paciente.Peso > 999.99m)
                throw new Exception("El peso del paciente está fuera de rango.");

            if (request.Paciente.Talla < 0 || request.Paciente.Talla > 999.99m)
                throw new Exception("La talla del paciente está fuera de rango.");

            if (request.Paciente.Creatinina.HasValue &&
                (request.Paciente.Creatinina < 0 || request.Paciente.Creatinina > 999.99m))
                throw new Exception("El valor de creatinina está fuera de rango.");

            var nuevaReceta = new RecetaCreate
            {
                IdReceta = Guid.NewGuid(),
                IdMedico = idMedico, // ID del médico desde el token
                IdPaciente = request.IdPaciente,
                PacPeso = request.Paciente.Peso,
                PacTalla = request.Paciente.Talla,
                PacEmbarazo = request.Paciente.Embarazo,
                PacSemAmenorrea = request.Paciente.SemanasAmenorrea,
                PacLactancia = request.Paciente.Lactancia,
                PacCreatinina = request.Paciente.Creatinina,
                Alergias = ConvertirListaAString(request.Paciente.Alergias),
                Molecules = ConvertirListaAString(request.Paciente.Moleculas),
                Patologias = ConvertirListaAString(request.Paciente.Patologias),
                IdSucursal = Guid.Parse(idSucursal),
                IdGEMP = Guid.Parse(idGEMP),
                FechaCreacion = DateTime.Now,
                FechaUltimaModificacion = DateTime.Now
            };
            await ActualizarPacienteDesdeReceta(
                request.IdPaciente,
                ConvertirListaAString(request.AlergiasCronicas).Replace(",", ";"),
                ConvertirListaAString(request.MoleculasCronicas).Replace(",", ";"),
                ConvertirListaAString(request.PatologiasCronicas).Replace(",", ";"),
                DateTime.Now
            );
            try
            {
                // Guardar receta principal
                _context.Recetas.Add(nuevaReceta);
                await _context.SaveChangesAsync();

                // Guardar detalles de receta
                foreach (var detalle in request.PrescriptionLines)
                {
                    // Validar PeriodoInicio y PeriodoTerminacion
                    if (detalle.PeriodoInicio > detalle.PeriodoTerminacion)
                    {
                        throw new Exception($"La fecha de inicio ({detalle.PeriodoInicio}) no puede ser mayor a la de terminación ({detalle.PeriodoTerminacion}).");
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
                        Indicacion = detalle.Indicacion,
                        IndicacionNombre = detalle.IndicacionNombre, // Nuevo campo
                        Frecuencia = detalle.Frecuencia,           // Nuevo campo
                        Observaciones = detalle.Observaciones,         // Nuevo campo
                        Duracion = detalle.Duracion,
                        UnidadDuracion = detalle.UnidadDuracion,
                        PeriodoInicio = detalle.PeriodoInicio,
                        PeriodoTerminacion = detalle.PeriodoTerminacion
                    };


                    _context.DetalleRecetas.Add(nuevoDetalle);
                }
                await _context.SaveChangesAsync();

                // Generar QR
                try
                {
                    var textoEncriptado = EncryptionHelper.Encrypt($"{nuevaReceta.IdReceta}|{nuevaReceta.IdMedico}|{nuevaReceta.FechaUltimaModificacion}");
                    var qrCodeBase64 = QRGenerator.GenerarQR(textoEncriptado);
                    Console.WriteLine($"Texto encriptado: {textoEncriptado}");
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
                    Console.WriteLine($"Error al encriptar: {ex.Message}");
                }
                return nuevaReceta.IdReceta; // Retornar el IdReceta generado
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Error al guardar la receta en la base de datos: {innerException}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado: {ex.Message}", ex);
            }
        }


        private async Task<List<PrescriptionLineModel>> GetMedicamentoActivoFromDatabase(Guid idPaciente)
        {
            try
            {
                // Conexión a la base de datos utilizando _context
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                // Configurar el comando para ejecutar el SP
                using var command = new SqlCommand("Consulta_GetMedcamentoActivoAnalisis", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Pasar el parámetro IdPaciente
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                // Lista para almacenar los resultados
                var result = new List<PrescriptionLineModel>();

                // Ejecutar el lector de datos
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new PrescriptionLineModel
                    {
                        Drug = reader.GetInt32(reader.GetOrdinal("Drug")),
                        DrugType = reader.GetString(reader.GetOrdinal("DrugType")),
                        Dose = reader.GetInt32(reader.GetOrdinal("Dose")),
                        UnitId = reader.GetInt32(reader.GetOrdinal("UnitId")),
                        Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                        DurationType = reader.GetString(reader.GetOrdinal("DurationType")),
                        Route = reader.GetInt32(reader.GetOrdinal("Route")),
                        Indication = reader.IsDBNull(reader.GetOrdinal("Indication"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("Indication")),
                        FrequencyType = reader.GetString(reader.GetOrdinal("Frecuency"))
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw new Exception("Error al consultar MedicamentoActivo desde la base de datos.", ex);
            }
        }


        public async Task ActualizarRecetaAsync(RecetaGetRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Consulta_ActualizarReceta", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Crear el JSON para la receta
                var recetaJson = JsonSerializer.Serialize(new
                {
                    IdReceta = request.Receta.IdReceta,
                    IdMedico = request.Receta.IdMedico,
                    IdPaciente = request.Receta.IdPaciente,
                    PacPeso = request.Receta.PacPeso,
                    PacTalla = request.Receta.PacTalla,
                    PacEmbarazo = request.Receta.PacEmbarazo,
                    PacSemAmenorrea = request.Receta.PacSemAmenorrea,
                    PacLactancia = request.Receta.PacLactancia,
                    PacCreatinina = request.Receta.PacCreatinina,
                    Alergias = string.Join(",", request.Receta.Alergias?.Select(a => a.NameAllergy) ?? new List<string>()),
                    Molecules = string.Join(",", request.Receta.Molecules?.Select(m => m.NameMolecule) ?? new List<string>()),
                    Patologias = string.Join(",", request.Receta.Patologias?.Select(p => p.NameCIM10) ?? new List<string>()),
                    IdSucursal = request.Receta.IdSucursal,
                    IdGEMP = request.Receta.IdGEMP
                });

                // Crear el JSON para los detalles de la receta
                var detallesJson = JsonSerializer.Serialize(request.Detalles.Select(detalle => new
                {
                    MedicamentoId = detalle.MedicamentoId,
                    MedicamentoType = detalle.MedicamentoType,
                    CantidadDiaria = detalle.CantidadDiaria,
                    UnidadDispensacionId = detalle.UnidadDispensacionId,
                    RutaAdministracionId = detalle.RutaAdministracionId,
                    Indicacion = detalle.Indicacion,
                    IndicacionNombre = detalle.IndicacionNombre, // Nuevo campo
                    Frecuencia = detalle.Frecuencia,           // Nuevo campo
                    Observaciones = detalle.Observaciones,         // Nuevo campo
                    Duracion = detalle.Duracion,
                    UnidadDuracion = detalle.UnidadDuracion,
                    PeriodoInicio = detalle.PeriodoInicio,
                    PeriodoTerminacion = detalle.PeriodoTerminacion,
                    Surtido = false, // Por defecto no surtido
                    FechaSurtido = (DateTime?)null // No hay fecha de surtido al crear o actualizar
                }));


                // Asignar los parámetros al comando
                command.Parameters.Add(new SqlParameter("@RecetaJson", SqlDbType.NVarChar)
                {
                    Value = recetaJson
                });

                command.Parameters.Add(new SqlParameter("@DetallesJson", SqlDbType.NVarChar)
                {
                    Value = detallesJson
                });

                // Ejecutar el procedimiento almacenado
                await command.ExecuteNonQueryAsync();
                //await ActualizarPacienteDesdeReceta(
                //     request.Receta.IdPaciente,
                //     ConvertirListaAString(request.Receta.Alergias.Select(a => a.IdAllergy).ToList()),
                //     ConvertirListaAString(request.Receta.Molecules.Select(m => m.IdMolecule).ToList()),
                //     ConvertirListaAString(request.Receta.Patologias.Select(p => p.IdCIM10).ToList()),
                //     DateTime.Now
                // );
                // **Generar y Actualizar QR**
                var textoEncriptado = EncryptionHelper.Encrypt($"{request.Receta.IdReceta}|{request.Receta.IdMedico}|{DateTime.Now}");
                var qrCodeBase64 = QRGenerator.GenerarQR(textoEncriptado);

                var recetaQR = await _context.Set<RecetaQR>()
                    .FirstOrDefaultAsync(qr => qr.IdReceta == request.Receta.IdReceta);

                if (recetaQR != null)
                {
                    recetaQR.QRData = qrCodeBase64;
                    recetaQR.FechaUltimaModificacion = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar la receta: {ex.Message}");
            }
        }

        public async Task<Guid?> ObtenerIdMedicoPorUsuarioAsync(Guid idUsuario)
        {
            var medico = await _context.Medicos
                .FirstOrDefaultAsync(m => m.IdUsuario == idUsuario);

            return medico?.IdMedico;
        }

        public async Task<RecetaCreate?> ObtenerRecetaPorIdAsync(Guid idReceta)
        {
            return await _context.Recetas.FirstOrDefaultAsync(r => r.IdReceta == idReceta);
        }

        public async Task EliminarRecetaAsync(Guid idReceta)
        {
            var receta = await ObtenerRecetaPorIdAsync(idReceta);

            if (receta != null)
            {
                _context.Recetas.Remove(receta);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RecetaGetRequest?> ConsultarRecetaAsync(Guid idReceta, Guid? idMedico)
        {
            try
            {
                var parametros = new List<SqlParameter>
                {
                    new("@IdReceta", idReceta)
                };

                if (idMedico.HasValue)
                {
                    parametros.Add(new SqlParameter("@IdMedico", idMedico.Value));
                }
                else
                {
                    parametros.Add(new SqlParameter("@IdMedico", DBNull.Value));
                }

                // Ejecutar SP y mapear directamente a un modelo compatible
                var recetaSQL = await _context.Set<RecetaGetSQL>()
                    .FromSqlRaw("EXEC Consulta_ConsultarReceta @IdReceta, @IdMedico", parametros.ToArray())
                    .ToListAsync();
                // Procesar las cadenas de alergias, molecules y patologías
  
                if (!recetaSQL.Any())
                {
                    return null; // Si no se encontró la receta, devolver null
                }
                // Transformar RecetaGetSQL en RecetaGet
                var receta = recetaSQL.First();
                var recetaTransformada = new RecetaGet
                {
                    IdReceta = receta.IdReceta,
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
                    IdGEMP = receta.IdGEMP
                };

                // Leer detalles de la receta
                var detalles = await _context.Set<DetalleRecetaGet>()
                    .FromSqlRaw("EXEC Consulta_ConsultarRecetaDetalles @IdReceta", new SqlParameter("@IdReceta", idReceta))
                    .ToListAsync();

                return new RecetaGetRequest
                {
                    Receta = recetaTransformada,
                    Detalles = detalles
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar la receta: {ex.Message}");
            }
            //    try
            //    {

            //        using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
            //        await connection.OpenAsync();

            //        using var command = new SqlCommand("Consulta_ConsultarReceta", connection);
            //        command.CommandType = CommandType.StoredProcedure;
            //        command.Parameters.AddWithValue("@IdReceta", idReceta);

            //        if (idMedico.HasValue)
            //        {
            //            command.Parameters.AddWithValue("@IdMedico", idMedico.Value);
            //        }
            //        else
            //        {
            //            command.Parameters.AddWithValue("@IdMedico", DBNull.Value);
            //        }

            //        using var reader = await command.ExecuteReaderAsync();
            //        var recetaGetRequest = new RecetaGetRequest();
            //        recetaGetRequest.Detalles = new List<DetalleRecetaGet>();

            //        // Leer la primera tabla (Receta)
            //        if (await reader.ReadAsync())
            //        {
            //            recetaGetRequest.Receta = new RecetaGet
            //            {
            //                IdReceta = (Guid)reader["IdReceta"],
            //                IdMedico = (Guid)reader["IdMedico"],
            //                NombresMedico = reader["Nombres"].ToString(),
            //                PrimerApellidoMedico = reader["PrimerApellido"].ToString(),
            //                SegundoApellidoMedico = reader["SegundoApellido"].ToString(),
            //                Universidad = reader["Universidad"].ToString(),
            //                CedulaGeneral = reader["CedulaGeneral"].ToString(),
            //                Especialidad = reader["Especialidad"].ToString(),
            //                CedulaEspecialidad = reader["CedulaEspecialidad"].ToString(),
            //                IdPaciente = (Guid)reader["IdPaciente"],
            //                NombresPaciente = reader["Nombres"].ToString(),
            //                PrimerApellidoPaciente = reader["PrimerApellido"].ToString(),
            //                SegundoApellidoPaciente = reader["SegundoApellido"].ToString(),
            //                FechaNacimientoPaciente = (DateTime)reader["FechaNacimiento"],
            //                PacPeso = (decimal)reader["PacPeso"],
            //                PacTalla = (decimal)reader["PacTalla"],
            //                PacEmbarazo = (bool)reader["PacEmbarazo"],
            //                PacSemAmenorrea = (int)reader["PacSemAmenorrea"],
            //                PacLactancia = (bool)reader["PacLactancia"],
            //                PacCreatinina = (decimal)reader["PacCreatinina"],
            //                Alergias = ParseAllergies(reader["Alergias"].ToString()),
            //                Molecules = ParseMolecules(reader["Molecules"].ToString()),
            //                Patologias = ParseCIM10(reader["Patologias"].ToString()),
            //                IdSucursal = (Guid)reader["IdSucursal"],
            //                IdGEMP = (Guid)reader["IdGEMP"]
            //            };
            //        }

            //        // Leer la segunda tabla (Detalles)
            //        if (await reader.NextResultAsync())
            //        {
            //            while (await reader.ReadAsync())
            //            {
            //                var detalle = new DetalleRecetaGet
            //                {
            //                    IdDetalleReceta = (Guid)reader["IdDetalleReceta"],
            //                    IdReceta = (Guid)reader["IdReceta"],
            //                    MedicamentoType = reader["MedicamentoType"].ToString(),
            //                    MedicamentoId = (int)reader["MedicamentoId"],
            //                    Medicamento = reader["Medicamento"].ToString(),
            //                    CantidadDiaria = (decimal)reader["CantidadDiaria"],
            //                    UnidadDispensacionId = (int)reader["UnidadDispensacionId"],
            //                    UnidadDispensacion = reader["UnidadDispensacion"].ToString(),
            //                    RutaAdministracionId = (int)reader["RutaAdministracionId"],
            //                    RutaAdministracion = reader["RutaAdministracion"].ToString(),
            //                    Indicacion = reader["Indicacion"].ToString(),
            //                    Duracion = (int)reader["Duracion"],
            //                    UnidadDuracion = reader["UnidadDuracion"].ToString(),
            //                    PeriodoInicio = (DateTime)reader["PeriodoInicio"],
            //                    PeriodoTerminacion = reader["PeriodoTerminacion"] as DateTime?,
            //                    Surtido = (bool)reader["Surtido"]
            //                };
            //                recetaGetRequest.Detalles.Add(detalle);
            //            }
            //        }

            //        return recetaGetRequest;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception($"Error al consultar la receta: {ex.Message}");
            //    }
        }

       

        public async Task<IEnumerable<RecetaPacienteModel>> ObtenerRecetasPorMedicoAsync(Guid idMedico, Guid idSucursal)
        {
            var medicoParam = new SqlParameter("@IdMedico", idMedico);
            var sucursalParam = new SqlParameter("@IdSucursal", idSucursal);

            try
            {
                var recetas = await _context.Set<RecetaPacienteModel>()
                    .FromSqlRaw("EXEC Consulta_RecetasPorMedico @IdMedico, @IdSucursal", medicoParam, sucursalParam)
                    .ToListAsync();

                return recetas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las recetas: {ex.Message}");
            }
        }

        public async Task<IEnumerable<SucursalModel>> ObtenerSucursalesPorUsuarioAsync(Guid idUsuario)
        {
            var usuarioParam = new SqlParameter("@IdUsuario", idUsuario);

            try
            {
                var sucursales = await _context.Set<SucursalModel>()
                    .FromSqlRaw("EXEC Consulta_SucursalesPorUsuario @IdUsuario", usuarioParam)
                    .ToListAsync();

                return sucursales;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las sucursales: {ex.Message}");
            }
        }

        private RecetaUpdate ConvertirRecetaGetToRecetaUpdate(RecetaGet recetaGet)
        {
            return new RecetaUpdate
            {
                IdReceta = recetaGet.IdReceta,
                IdMedico = recetaGet.IdMedico,
                PacPeso = recetaGet.PacPeso,
                PacTalla = recetaGet.PacTalla,
                PacEmbarazo = recetaGet.PacEmbarazo,
                PacSemAmenorrea = recetaGet.PacSemAmenorrea,
                PacLactancia = recetaGet.PacLactancia,
                PacCreatinina = recetaGet.PacCreatinina,
                Alergias = ConvertirListaAString(recetaGet.Alergias.Select(a => a.IdAllergy).ToList()),
                Molecules = ConvertirListaAString(recetaGet.Molecules.Select(m => m.IdMolecule).ToList()),
                Patologias = ConvertirListaAString(recetaGet.Patologias.Select(c => c.IdCIM10).ToList())
            };
        }

        public async Task<IEnumerable<DetalleRecetaResponse>> GetReaccionMedicamentoPrevioAsync(Guid idPaciente)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DetalleReceta_GetReaccionesPaciente", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetro para IdPaciente
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);
                command.Parameters.Add("@OutMessage", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output;

                // Ejecutar el SP y leer los resultados
                using var reader = await command.ExecuteReaderAsync();
                var reaccionesPrevias = new List<DetalleRecetaResponse>();

                while (await reader.ReadAsync())
                {
                    var reaccion = new DetalleRecetaResponse
                    {
                        IdDetalleReceta = (Guid)reader["IdDetalleReceta"],
                        IdReceta = (Guid)reader["IdReceta"],
                        IdPaciente = (Guid)reader["IdPaciente"],
                        MedicamentoId = (int)reader["MedicamentoId"],
                        MedicamentoType = reader["MedicamentoType"].ToString(),
                        Medicamento = reader["Medicamento"].ToString(),
                        Descripcion = reader["Descripcion"].ToString()
                    };

                    reaccionesPrevias.Add(reaccion);
                }

                return reaccionesPrevias;
            }
            catch (SqlException sqlEx)
            {
                // Manejo de excepciones específicas de SQL
                throw new Exception("Error en la ejecución del procedimiento almacenado.", sqlEx);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones genéricas
                throw new Exception("Se produjo un error al obtener las reacciones previas del paciente.", ex);
            }
        }


        private async Task ActualizarPacienteDesdeReceta(
            Guid idPaciente,
            string alergias,
            string molecules,
            string patologias,
            DateTime fechaUltimaModificacion)
        {
            var pacienteExistente = await _context.Pacientes.FindAsync(idPaciente);
            if (pacienteExistente != null)
            {
                pacienteExistente.Alergias = string.IsNullOrEmpty(alergias) ? "" : alergias;
                pacienteExistente.Molecules = string.IsNullOrEmpty(molecules) ? "" : molecules;
                pacienteExistente.Patologias = string.IsNullOrEmpty(patologias) ? "" : patologias;
                pacienteExistente.FechaUltimaModificacion = fechaUltimaModificacion;

                await _context.SaveChangesAsync();
            }
        }



        private string ObtenerValorDesdeToken(string token, string claimType)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);

            return claim?.Value ?? string.Empty;
        }

        private string ConvertirListaAString(List<int> lista)
        {
            return string.Join(",", lista);
        }
        private List<RequestSearchAllergy> ParseAllergies(string allergiesString)
        {
            return allergiesString.Split(',')
                .Select(a =>
                {
                    var parts = a.Split('-');
                    return new RequestSearchAllergy
                    {
                        IdAllergy = int.Parse(parts[0].Trim()),
                        NameAllergy = parts[1].Trim()
                    };
                }).ToList();
        }

        private List<RequestSearchMolecules> ParseMolecules(string moleculesString)
        {
            return moleculesString.Split(',')
                .Select(m =>
                {
                    var parts = m.Split('-');
                    return new RequestSearchMolecules
                    {
                        IdMolecule = int.Parse(parts[0].Trim()),
                        NameMolecule = parts[1].Trim()
                    };
                }).ToList();
        }

        private List<ListIM10> ParseCIM10(string cim10String)
        {
            return cim10String.Split(',')
                .Select(c =>
                {
                    var parts = c.Split('-');
                    return new ListIM10
                    {
                        IdCIM10 = int.Parse(parts[0].Trim()),
                        NameCIM10 = parts[1].Trim(),
                        Code = parts[2].Trim() // Ahora se guarda como string
                    };
                }).ToList();
        }

    }
}
