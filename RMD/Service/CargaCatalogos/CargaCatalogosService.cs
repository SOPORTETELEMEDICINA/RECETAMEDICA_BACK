using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Extensions.CargaCatalogos;
using RMD.Interface.CargaCatalogos;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.Allergy;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.ATC;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.CIM10;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.Molecule;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.Package;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.Product;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.Route;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.UCD;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.UCDV;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.Unit;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.VMP;
using System.Data;
using System.Diagnostics;
//using RMD.Models.CargaCatalogos;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RMD.Service.CargaCatalogos
{
    public class CargaCatalogosService : ICargaCatalogosService
    {
        private readonly HttpClient _httpClient;
        private readonly VidalAPIDbContext _context;
        private readonly ICatalogoNotificacionService _catNoti;
        private readonly int _startPage, _pageSize;
        private readonly string _appId, _appKey;
        public CargaCatalogosService(
            IHttpClientFactory httpClientFactory,
            IConfiguration cfg,
            VidalAPIDbContext context,
            ICatalogoNotificacionService catNoti,
            byte[] commonKey)
        {
            _context = context;
            _catNoti = catNoti;

            var country = cfg["Country"] ?? "MX";
            var isES = country == "ES";

            var clientName = isES ? "VidalClientES" : "VidalClientMX";
            var prefix = isES ? "VidalApiES" : "VidalApi";

            _httpClient = httpClientFactory.CreateClient(clientName);
            _httpClient.Timeout = TimeSpan.FromMinutes(5);

            // Desencriptar AppId y AppKey
            var appIdEncrypted = cfg[$"{prefix}:AppId"]!;
            var appKeyEncrypted = cfg[$"{prefix}:AppKey"]!;
            _appId = EncryptionHelper.Decrypt(appIdEncrypted, commonKey);
            _appKey = EncryptionHelper.Decrypt(appKeyEncrypted, commonKey);
            _startPage = int.Parse(cfg[$"{prefix}:StartPage"]!);
            _pageSize = int.Parse(cfg[$"{prefix}:SizePage"]!);
        }
        public async Task<ResponseFromService<string>> ReloadCatalogs()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await GetAllVMPsAsync();
                await GetAllPackagesAsync();
                await GetAllUnitsAsync();
                await GetAllProductsAsync();
                await GetAllAllergiesAsync();
                await GetAllMoleculesAsync();
                await GetAllRoutesAsync();
                await GetAllCIM10Async();
                await GetAllUCDVsAsync();
                await GetAllUCDsAsync();
                await GetAllATCClassificationsAsync();


                //await GetAllSideEffectsAsync();
                //await GetAllVTMsAsync();
                sw.Stop();
                var msg = $"Carga terminada en {sw.Elapsed.Hours}h {sw.Elapsed.Minutes}m {sw.Elapsed.Seconds}s";
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                notif.Mensaje = msg;
                return ResponseFromService<string>.Success(msg, notif);
            }
            catch (Exception ex)
            {
                sw.Stop();
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Fallo tras {sw.Elapsed.Minutes}m{sw.Elapsed.Seconds}s: {ex.Message}";
                return ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllPackagesAsync()
        {
            try
            {
                var list = new List<PackageApiModel>();
                var page = _startPage; var total = 0;

                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"packages?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParsePackagesXmlToModelList();
                    list.AddRange(chunk);
                    if (total == 0) total = xml.GetOpenSearchValue<int>("totalResults");
                } while (list.Count < total);

                var json = JsonSerializer.Serialize(list);

                var param = new SqlParameter("@Json", SqlDbType.NVarChar)
                {
                    Value = json
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal.InsertPackagesVidalApi @Json", param);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("Paquetes cargados", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error Paquetes: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllVMPsAsync()
        {
            try
            {
                var list = new List<VMPApiModel>();
                var page = _startPage;
                var total = 0;
                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"vmps?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseVMPXmlToModelList();
                    list.AddRange(chunk);
                    // total stays 0 to fetch until list.Count<0 = false, break immediately
                } while (list.Count < total);

                var dt = list.ToDataTable();
                var p = new SqlParameter("@VMPTableType", SqlDbType.Structured)
                { TypeName = "dbo.Vidal_VMPTableType", Value = dt };
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal.InsertVMPVidalApi @VMPTableType", p);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("VMPs cargados", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error VMPs: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllUnitsAsync()
        {
            try
            {
                var list = new List<UnitApiModel>();
                var page = _startPage; var total = 0;
                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"units?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseUnitsXmlToModelList();
                    list.AddRange(chunk);
                } while (list.Count < total);

                var dt = list.ToDataTable();
                var p = new SqlParameter("@UnitTableType", SqlDbType.Structured)
                { TypeName = "dbo.Vidal_UnitType", Value = dt };
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal.InsertUnitsVidalApi @UnitTableType", p);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("Unidades cargadas", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error Unidades: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllProductsAsync()
        {
            try
            {
                var list = new List<ProductApiModel>();
                var page = _startPage;
                var total = 0;

                do
                {
                    var xml = await _httpClient.GetStringAsync($"products?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseProductsXmlToModelList();
                    list.AddRange(chunk);
                    if (total == 0) total = xml.GetOpenSearchValue<int>("totalResults");
                } while (list.Count < total);

                var json = JsonSerializer.Serialize(list);
                var param = new SqlParameter("@JsonProductos", SqlDbType.NVarChar)
                {
                    Value = json
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertProductosVidalApi] @JsonProductos", param);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("Productos cargados", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error Productos: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllAllergiesAsync()
        {
            try
            {
                var list = new List<AllergyApiModel>();
                var page = _startPage; var total = 0;
                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"allergies?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseAllergiesXmlToModelList();
                    list.AddRange(chunk);
                } while (list.Count < total);

                var dt = list.ToDataTable();
                var p = new SqlParameter("@AllergyTableType", SqlDbType.Structured)
                { TypeName = "dbo.Vidal_AllergyTableType", Value = dt };
                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertAllergiesVidalApi] @AllergyTableType", p);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("Alergias cargadas", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error Alergias: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllMoleculesAsync()
        {
            try
            {
                var list = new List<MoleculeApiModel>();
                var page = _startPage; var total = 0;
                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"molecules?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseMoleculesXmlToModelList();
                    list.AddRange(chunk);
                    if (total == 0) total = xml.GetOpenSearchValue<int>("totalResults");
                } while (list.Count < total);

                var dt = list.ToDataTable();
                var p = new SqlParameter("@MoleculeTableType", SqlDbType.Structured)
                { TypeName = "dbo.Vidal_MoleculeType", Value = dt };
                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertMoleculesVidalApi] @MoleculeTableType", p);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("Moléculas cargadas", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error Moléculas: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllRoutesAsync()
        {
            try
            {
                var list = new List<RouteApiModel>();
                var page = _startPage; var total = 0;
                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"routes?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseRoutesXmlToModelList();
                    list.AddRange(chunk);
                    if (total == 0) total = xml.GetOpenSearchValue<int>("totalResults");
                } while (list.Count < total);

                var dt = list.ToDataTable();
                var p = new SqlParameter("@RouteTableType", SqlDbType.Structured)
                { TypeName = "dbo.Vidal_RouteType", Value = dt };
                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertRoutesVidalApi] @RouteTableType", p);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("Rutas cargadas", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error Rutas: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllCIM10Async()
        {
            try
            {
                var list = new List<CIM10ApiModel>();
                var page = _startPage; var total = 0;
                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"cim10s?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseCIM10XmlToModelList();
                    list.AddRange(chunk);
                    if (total == 0) total = xml.GetOpenSearchValue<int>("totalResults");
                } while (list.Count < total);

                var json = JsonSerializer.Serialize(list);

                var p = new SqlParameter("@JsonRecords", SqlDbType.NVarChar)
                {
                    Value = json
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertCIM10VidalApi] @JsonRecords", p);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("CIM10 cargados", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error CIM10: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllUCDsAsync()
        {
            try
            {
                var list = new List<UCDApiModel>();
                var page = _startPage; var total = 0;
                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"ucds?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseUCDXmlToModelList();
                    list.AddRange(chunk);
                    if (total == 0) total = xml.GetOpenSearchValue<int>("totalResults");
                } while (list.Count < total);

                var dt = list.ToDataTable();
                var p = new SqlParameter("@UCDTableType", SqlDbType.Structured)
                { TypeName = "dbo.Vidal_UCDTableType", Value = dt };
                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertUCDVidalApi] @UCDTableType", p);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("UCDs cargados", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error UCDs: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllUCDVsAsync()
        {
            try
            {
                var list = new List<UCDVApiModel>();
                var page = _startPage; var total = 0;

                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"ucdvs?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");
                    var chunk = xml.ParseUCDVXmlToModelList();
                    list.AddRange(chunk);
                    if (total == 0) total = xml.GetOpenSearchValue<int>("totalResults");
                } while (list.Count < total);

                var json = JsonSerializer.Serialize(list);

                var param = new SqlParameter("@Json", SqlDbType.NVarChar)
                {
                    Value = json
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertUCDVVidalApi] @Json", param);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("UCDVs cargados", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error UCDV: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }
        private async Task GetAllATCClassificationsAsync()
        {
            try
            {
                var list = new List<ATCClassificationModel>();
                var page = _startPage;
                var total = 0;

                do
                {
                    var xml = await _httpClient
                        .GetStringAsync($"atc-classifications?start-page={page++}&page-size={_pageSize}&app_id={_appId}&app_key={_appKey}");

                    var chunk = xml.ParseATCClassificationXmlToModelList();
                    list.AddRange(chunk);

                    if (total == 0)
                        total = xml.GetOpenSearchValue<int>("totalResults");

                } while (list.Count < total);

                var dt = list.ToDataTable();

                var p = new SqlParameter("@ATCTableType", SqlDbType.Structured)
                {
                    TypeName = "dbo.Vidal_ATCClassificationType",
                    Value = dt
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertATCClassifications] @ATCTableType", p);

                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                ResponseFromService<string>.Success("ATC Classifications cargados", notif);
            }
            catch (Exception ex)
            {
                var notif = await _catNoti.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notif.Mensaje = $"Error ATC Classifications: {ex.Message}";
                ResponseFromService<string>.Failure(notif);
            }
        }

    }
}
