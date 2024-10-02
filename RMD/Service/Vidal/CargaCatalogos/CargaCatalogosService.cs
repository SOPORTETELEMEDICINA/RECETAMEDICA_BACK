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

        public async Task<ResponseFromService<string>> ReloadCatalogs()
        {

            var stopwatch = Stopwatch.StartNew();
            try
            {
                await GetAllVMPsAsync();
                await GetAllProductsAsync();
                await GetAllPackagesAsync(); // Nuevo método para Packages
                await GetAllUnitsAsync();
                await GetAllAllergiesAsync();
                await GetAllMoleculesAsync();
                await GetAllRoutesAsync();
                await GetAllCIM10Async();
                await GetAllVTMsAsync();
                await GetAllATCClassificationsAsync();
                await GetAllUCDVsAsync();
                await GetAllUCDsAsync();
                await GetAllSideEffectsAsync();
                // Detener el cronómetro
                stopwatch.Stop();
                // Calcular el tiempo en minutos y segundos
                TimeSpan ts = stopwatch.Elapsed;
                string formattedTime = $"{ts.Hours} Horas y {ts.Minutes} minutos y {ts.Seconds} segundos";
                // Loguear o mostrar el tiempo que tardó la ejecución
                // Retornar éxito
                return ResponseFromService<string>.Success($"Los datos de VMP se insertaron correctamente en{formattedTime}.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                // Detener el cronómetro si ocurre una excepción
                stopwatch.Stop();

                // Calcular el tiempo en minutos y segundos hasta el fallo
                TimeSpan ts = stopwatch.Elapsed;
                string formattedTime = $"{ts.Hours} Horas y {ts.Minutes} minutos y {ts.Seconds} segundos";
                // Retornar error
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar VMPs después de {formattedTime}: {ex.Message}");
            }
        }

        private async Task<ResponseFromService<string>> GetAllVMPsAsync()
        {
            // Crear un cronómetro para medir el tiempo de ejecución
            
          

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


                // Aquí llamamos a la función que se encargará de procesar los links
               // await ValidateAndFilterTitlesVMP(vmpModelLinkList);

                // Loguear o mostrar el tiempo que tardó la ejecución
                // Retornar éxito
                return ResponseFromService<string>.Success($"Los datos de VMP se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                // Retornar error
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar VMPs : {ex.Message}");
            }
        }

        private async Task<ResponseFromService<string>> GetAllProductsAsync()
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

        private async Task<ResponseFromService<string>> GetAllPackagesAsync()
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

        private async Task<ResponseFromService<string>> GetAllUnitsAsync()
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

        private async Task<ResponseFromService<string>> GetAllAllergiesAsync()
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

        private async Task<ResponseFromService<string>> GetAllMoleculesAsync()
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

        private async Task<ResponseFromService<string>> GetAllRoutesAsync()
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

        private async Task<ResponseFromService<string>> GetAllCIM10Async()
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

        private async Task<ResponseFromService<string>> GetAllVTMsAsync()
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

        private async Task<ResponseFromService<string>> GetAllATCClassificationsAsync()
        {
            var atcClassificationList = new List<ATCClassificationModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;
            var currentPage = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/atc-classifications?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var atcEntries = xmlContent.ParseATCClassificationXmlToModelList(); // Convertir el XML en la lista de ATCClassificationModel

                    if (atcEntries == null || !atcEntries.Any())
                    {
                        // Si no hay entradas, termina el ciclo
                        break;
                    }

                    atcClassificationList.AddRange(atcEntries);

                    // Obtener totalResults solo una vez
                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

                        // Si no se encuentra el total de resultados o es 0, termina el ciclo
                        if (totalResults == 0)
                        {
                            break;
                        }
                    }

                    currentPage++;
                    startPage++;

                } while (atcClassificationList.Count < totalResults && currentPage * pageSize < totalResults);

                // Convertir la lista en DataTable
                var atcDataTable = atcClassificationList.ToDataTable();

                // Llamar al procedimiento almacenado para insertar los datos usando el DbContext
                var atcParam = new SqlParameter
                {
                    ParameterName = "@ATCTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_ATCClassificationType", // Aquí especificas el nombre del TableType
                    Value = atcDataTable
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertATCClassifications @ATCTableType", atcParam);

                // Retornar éxito
                return ResponseFromService<string>.Success("Los datos de ATC Classification se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                // Retornar error
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar ATC Classification: {ex.Message}");
            }
        }

        private async Task<ResponseFromService<string>> GetAllUCDVsAsync()
        {
            var ucdvModelList = new List<UCDVModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/ucdvs?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var ucdvEntries = xmlContent.ParseUCDVXmlToModelList();

                    ucdvModelList.AddRange(ucdvEntries);

                    // Obtener totalResults solo una vez desde el XML
                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    }

                    startPage++;
                } while (ucdvModelList.Count < totalResults); // Verifica que el bucle termine cuando se alcanzan los resultados

                // Aquí conviertes a DataTable y haces el proceso para insertar en la base de datos
                var ucdvDataTable = ucdvModelList.ToDataTable();

                var ucdvParam = new SqlParameter
                {
                    ParameterName = "@UCDVTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_UCDVTableType", // Aquí especificas el nombre del TableType
                    Value = ucdvDataTable
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertUCDVs @UCDVTableType", ucdvParam);

                return ResponseFromService<string>.Success("Los datos de UCDV se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar UCDV: {ex.Message}");
            }
        }

        private async Task<ResponseFromService<string>> GetAllUCDsAsync()
        {
            var ucdModelList = new List<UCDModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/ucds?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var ucdEntries = xmlContent.ParseUCDXmlToModelList();

                    ucdModelList.AddRange(ucdEntries);

                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    }

                    startPage++;
                } while (ucdModelList.Count < totalResults);

                var ucdDataTable = ucdModelList.ToDataTable();

                var ucdParam = new SqlParameter
                {
                    ParameterName = "@UCDTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_UCDTableType",
                    Value = ucdDataTable
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertUCDs @UCDTableType", ucdParam);

                return ResponseFromService<string>.Success("Los datos de UCD se insertaron correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                return ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, $"Error al insertar UCDs: {ex.Message}");
            }
        }

        private async Task<ResponseFromService<string>> GetAllSideEffectsAsync()
        {
            var sideEffectList = new List<SideEffectModel>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/side-effects?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var sideEffectEntries = xmlContent.ParseSideEffectsXmlToModelList();

                    sideEffectList.AddRange(sideEffectEntries);

                    if (totalResults == 0)
                    {
                        totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    }

                    startPage++;
                } while (sideEffectList.Count < totalResults);

                var sideEffectDataTable = sideEffectList.ToDataTable();

                var sideEffectParam = new SqlParameter
                {
                    ParameterName = "@SideEffectTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_SideEffectTableType",
                    Value = sideEffectDataTable
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertSideEffects @SideEffectTableType", sideEffectParam);

                return ResponseFromService<string>.Success("Datos de SideEffects insertados correctamente.", "Proceso exitoso");
            }
            catch (Exception ex)
            {
                return ResponseFromService<string>.Failure(System.Net.HttpStatusCode.InternalServerError, $"Error al insertar SideEffects: {ex.Message}");
            }
        }


        private async Task ValidateAndFilterTitlesVMP(List<VMPModelLink> vmpModelLinkList)
        {
            var multipleEntryTitles = new HashSet<string>();
            var singleEntryTitles = new HashSet<string>();

            foreach (var vmpModelLink in vmpModelLinkList)
            {
                int idDestino = vmpModelLink.Vmp.IdVMP;

                foreach (var link in vmpModelLink.Links)
                {
                    if (link.Title == "UNITS")
                    {
                        Console.WriteLine("Hola");
                    }
                    // Generar el request URL
                    var requestUrl = $"{_basicUrl}{link.Href}";
                    if (char.IsDigit(link.Href.Last()))
                    {
                        requestUrl += $"&start-page=1&page-size=25&app_id={_appId}&app_key={_appKey}";
                    }
                    else
                    {
                        requestUrl += $"?start-page=1&page-size=25&app_id={_appId}&app_key={_appKey}";
                    }

                    // Hacer la solicitud HTTP
                    var response = await _httpClient.GetAsync(requestUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error: {response.StatusCode} for {link.Href}");
                        continue;
                    }

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(xmlContent))
                    {
                        Console.WriteLine($"Advertencia: El contenido del link {link.Href} está vacío.");
                        continue;
                    }

                    // Aquí enviamos el XML y el IdDestino al parser para obtener la lista de IdBaseDestinoModel
                    var idBaseDestinoList = xmlContent.ParseVidalIdsToModelList(idDestino);

                    // Convertir la lista de IdBaseDestinoModel a DataTable
                    var idBaseDestinoDataTable = idBaseDestinoList.ToDataTable();

                    // Llamar a la función para insertar los datos en la base de datos
                    await InsertIdBaseDestinoAsync(idBaseDestinoDataTable, link.Title, "VMP");
                }
            }
        }

        private async Task InsertIdBaseDestinoAsync(DataTable idBaseDestinoDataTable, string destination, string Relacion)
        {
            try
            {
                // Crear el parámetro de SQL para el TableType
                var tableParam = new SqlParameter
                {
                    ParameterName = "@IdBaseDestinoTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.Vidal_IdBaseDestinoTableType",
                    Value = idBaseDestinoDataTable
                };

                // Crear el parámetro de SQL para el Destino (link.Title)
                var destinoParam = new SqlParameter
                {
                    ParameterName = "@Destino",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    Value = destination
                };
                var relacionParam = new SqlParameter
                {
                    ParameterName = "@Relacion",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    Value = Relacion
                };

                // Ejecutar el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal_InsertIdBaseDestino @IdBaseDestinoTableType, @Destino, @Relacion", tableParam, destinoParam, relacionParam);

                Console.WriteLine($"Datos insertados correctamente para {destination}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar datos para {destination}: {ex.Message}");
            }
        }
    }
}
