using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Extensions.Vidal.CargaCatalogos;
using RMD.Interface.Vidal;
using RMD.Models.Responses;
using RMD.Models.Vidal.CargaCatalogos;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;

namespace RMD.Service.Vidal.CargaCatalogos
{
    public class CargaCatalogosService: ICargaCatalogosService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _basicUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;
        private readonly VidalDbContext _context;

        public CargaCatalogosService(HttpClient httpClient, IConfiguration configuration, VidalDbContext context)
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

        public async Task<ResponseFromService<string>> GetAllVMPsAsync()
        {
            var vmpModelLinkList = new List<VMPModelLink>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/vmps?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var vmpEntries = xmlContent.ParseVMPXmlToModelList(); // Convertir el XML en la lista de VMPModel

                    vmpModelLinkList.AddRange(vmpEntries);

                    startPage++;
                } while (vmpModelLinkList.Count < totalResults);

                // Aquí llamamos a la función que se encargará de procesar los links
                var validationResult = await ValidateAndFilterTitles(vmpModelLinkList);

                // OBTENER el primer VMPModelLink y mostrar sus datos
               


                // Convertir los VMPs a DataTable para guardar en la base de datos
                var vmpDataTable = vmpModelLinkList.Select(v => v.Vmp).ToList().ToDataTable();


                // Llamar al procedimiento almacenado para insertar los datos usando el DbContext
                var vmpParam = new SqlParameter
                {
                    ParameterName = "@VMPTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_VMPTableType", // Aquí especificas el nombre del TableType
                    Value = vmpDataTable
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertVMPs @VMPTableType", vmpParam);

                // Retornar éxito
                return ResponseFromService<string>.Success("Los datos de VMP se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                // Retornar error
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar VMPs: {ex.Message}");
            }
        }




        private async Task<ValidationResult> ValidateAndFilterTitles(List<VMPModelLink> vmpModelLinkList)
        {
            var multipleEntryTitles = new HashSet<string>();
            var singleEntryTitles = new HashSet<string>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;
            foreach (var vmpModelLink in vmpModelLinkList)
            {
                foreach (var link in vmpModelLink.Links)
                {
                    // Si el title ya está en la lista de múltiples entries, saltamos la validación de este link
                    if (multipleEntryTitles.Contains(link.Title)) continue;

                    // Generar el request URL
                    var requestUrl = $"{_basicUrl}{link.Href}";
                    // Corregir la validación si termina en dígito o barra
                    if (char.IsDigit(link.Href.Last()))
                    {
                        // Si termina en dígito, agregar la query con "&"
                        requestUrl += $"&start-page=1&page-size=25&app_id={_appId}&app_key={_appKey}";
                    }
                    else
                    {
                        // Si no termina en dígito, agregar la query con "?"
                        requestUrl += $"?start-page=1&page-size=25&app_id={_appId}&app_key={_appKey}";
                    }

                    // Hacer la solicitud HTTP
                    var response = await _httpClient.GetAsync(requestUrl);
                    // Validar si la respuesta es exitosa
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error: {response.StatusCode} for {link.Href}");
                        continue; // Saltar al siguiente link si la respuesta no es exitosa
                    }
                    var xmlContent = await response.Content.ReadAsStringAsync();
                    // Verificar si el contenido XML es vacío
                    if (string.IsNullOrWhiteSpace(xmlContent))
                    {
                        Console.WriteLine($"Advertencia: El contenido del link {link.Href} está vacío.");
                        continue; // Saltar al siguiente link si el contenido es vacío
                    }

                    // Verificar cuántos entries tiene el XML de la respuesta
                    XDocument document;
                    try
                    {
                        document = XDocument.Parse(xmlContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al analizar el XML del link {link.Href}: {ex.Message}");
                        continue; // Saltar al siguiente link si no se puede analizar el XML
                    }
                    var entries = document.Descendants().Where(x => x.Name.LocalName == "entry").ToList();

                    // Verificar si el link devuelve uno o más de un entry
                    if (entries.Count > 1)
                    {
                        // Si hay más de un entry, lo marcamos como "múltiples" y lo quitamos de la lista de "single-entry"
                        multipleEntryTitles.Add(link.Title);
                        singleEntryTitles.Remove(link.Title);
                    }
                    else if (!multipleEntryTitles.Contains(link.Title))
                    {
                        // Si hay solo un entry, lo agregamos a la lista de "single-entry"
                        singleEntryTitles.Add(link.Title);
                    }
                }
            }

            // Retornar los resultados
            return new ValidationResult
            {
                SingleEntryTitles = singleEntryTitles.ToList(),
                MultipleEntryTitles = multipleEntryTitles.ToList()
            };
        }

        // Clase para manejar los resultados de la validación
        private class ValidationResult
        {
            public List<string> SingleEntryTitles { get; set; }
            public List<string> MultipleEntryTitles { get; set; }
        }













        public async Task<ResponseFromService<string>> GetAllProductsAsync()
        {
            var productList = new List<ProductModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/products?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var productEntries = xmlContent.ParseProductsXmlToModelList();  // Aquí obtenemos la lista de productos

                    productList.AddRange(productEntries);

                    // Aquí agregamos el cálculo de totalResults para detener el bucle
                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    }

                    startPage++;
                } while (productList.Count < totalResults);

                // Convertir la lista en DataTable
                var productDataTable = productList.ToDataTable();

                // Llamar al procedimiento almacenado para insertar los datos usando el DbContext
                var productParam = new SqlParameter
                {
                    ParameterName = "@ProductoTableType", // Aquí debes corregir el nombre a @ProductoTableType
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_ProductoType", // Aquí especificas el nombre del TableType
                    Value = productDataTable
                };

                // Ejecutar el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertProductos @ProductoTableType", productParam);

                // Retornar éxito
                return ResponseFromService<string>.Success("Los datos de Productos se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                // Retornar error
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar Productos: {ex.Message}");
            }
        }

        public async Task<ResponseFromService<string>> GetAllPackagesAsync()
        {
            var packageList = new List<PackageModel>();
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
                    var packageEntries = xmlContent.ParsePackagesXmlToModelList();  // Aquí obtenemos la lista de paquetes

                    packageList.AddRange(packageEntries);

                    // Aquí agregamos el cálculo de totalResults para detener el bucle
                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    }

                    startPage++;
                } while (packageList.Count < totalResults);

                // Convertir la lista en DataTable
                var packageDataTable = packageList.ToDataTable();

                // Llamar al procedimiento almacenado para insertar los datos usando el DbContext
                var packageParam = new SqlParameter
                {
                    ParameterName = "@PackageTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_PackageType", // Aquí especificas el nombre del TableType
                    Value = packageDataTable
                };

                // Ejecutar el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertPackages @PackageTableType", packageParam);

                // Retornar éxito
                return ResponseFromService<string>.Success("Los datos de Paquetes se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                // Retornar error
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar Paquetes: {ex.Message}");
            }
        }

        public async Task<ResponseFromService<string>> GetAllUnitsAsync()
        {
            var unitList = new List<UnitModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/units?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var unitEntries = xmlContent.ParseUnitsXmlToModelList(); // Convertir el XML en la lista de UnitModel

                    unitList.AddRange(unitEntries);

                    startPage++;
                } while (unitList.Count < totalResults);

                // Convertir la lista en DataTable
                var unitDataTable = unitList.ToDataTable();

                // Llamar al procedimiento almacenado para insertar los datos usando el DbContext
                var unitParam = new SqlParameter
                {
                    ParameterName = "@UnitTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_UnitType", // Aquí especificas el nombre del TableType
                    Value = unitDataTable
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertUnits @UnitTableType", unitParam);

                // Retornar éxito
                return ResponseFromService<string>.Success("Los datos de las Unidades se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                // Retornar error
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar Unidades: {ex.Message}");
            }
        }

        public async Task<ResponseFromService<string>> GetAllAllergiesAsync()
        {
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;
            var allergyList = new List<AllergyModel>();

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/allergies?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var allergyEntries = xmlContent.ParseAllergiesXmlToModelList();

                    allergyList.AddRange(allergyEntries);

                    startPage++;
                } while (allergyList.Count < totalResults);
                            
                var allergyDataTable = allergyList.ToDataTable();

                var allergyParam = new SqlParameter
                {
                    ParameterName = "@AllergyTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_AllergyType",
                    Value = allergyDataTable
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertAllergies @AllergyTableType", allergyParam);

                return ResponseFromService<string>.Success("Datos de alergias insertados correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar alergias: {ex.Message}");
            }
        }

        public async Task<ResponseFromService<string>> GetAllMoleculesAsync()
        {
            var moleculeList = new List<MoleculeModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/molecules?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var moleculeEntries = xmlContent.ParseMoleculesXmlToModelList();

                    moleculeList.AddRange(moleculeEntries);

                    // Aquí agregamos el cálculo de totalResults para detener el bucle
                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    }

                    startPage++;
                } while (moleculeList.Count < totalResults);

                // Convertir la lista en DataTable
                var moleculeDataTable = moleculeList.ToDataTable();

                var moleculeParam = new SqlParameter
                {
                    ParameterName = "@MoleculeTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_MoleculeType",
                    Value = moleculeDataTable
                };

                // Ejecutar el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertMolecules @MoleculeTableType", moleculeParam);

                return ResponseFromService<string>.Success("Los datos de las moléculas se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar moléculas: {ex.Message}");
            }
        }

        public async Task<ResponseFromService<string>> GetAllRoutesAsync()
        {
            var routeList = new List<RouteModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/routes?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var routeEntries = xmlContent.ParseRoutesXmlToModelList();

                    routeList.AddRange(routeEntries);

                    // Aquí agregamos el cálculo de totalResults para detener el bucle
                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    }

                    startPage++;
                } while (routeList.Count < totalResults);

                // Convertir la lista en DataTable
                var routeDataTable = routeList.ToDataTable();

                var routeParam = new SqlParameter
                {
                    ParameterName = "@RouteTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_RouteType",
                    Value = routeDataTable
                };

                // Ejecutar el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertRoutes @RouteTableType", routeParam);

                return ResponseFromService<string>.Success("Los datos de las rutas se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar rutas: {ex.Message}");
            }
        }

        public async Task<ResponseFromService<string>> GetAllCIM10Async()
        {
            var cim10List = new List<CIM10Model>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/cim10s?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var cim10Entries = xmlContent.ParseCIM10XmlToModelList(); // Parsear el XML a la lista de CIM10

                    cim10List.AddRange(cim10Entries);

                    // Aquí agregamos el cálculo de totalResults para detener el bucle
                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    }

                    startPage++;
                } while (cim10List.Count < totalResults);

                // Convertir la lista en DataTable
                var cim10DataTable = cim10List.ToDataTable();

                var cim10Param = new SqlParameter
                {
                    ParameterName = "@CIM10TableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_CIM10Type",
                    Value = cim10DataTable
                };

                // Ejecutar el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertCIM10 @CIM10TableType", cim10Param);

                return ResponseFromService<string>.Success("Los datos de CIM10 se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar CIM10: {ex.Message}");
            }
        }

        public async Task<ResponseFromService<string>> GetAllVTMsAsync()
        {
            var vtmList = new List<VTMModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/vtms?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var vtmEntries = xmlContent.ParseVTMsXmlToModelList();

                    vtmList.AddRange(vtmEntries);

                    startPage++;
                } while (vtmList.Count < totalResults);

                var vtmDataTable = vtmList.ToDataTable();

                var vtmParam = new SqlParameter
                {
                    ParameterName = "@VTMTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_VTMType",
                    Value = vtmDataTable
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertVTM @VTMTableType", vtmParam);

                return ResponseFromService<string>.Success("Los datos de VTM se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar VTM: {ex.Message}");
            }
        }

    }
}
