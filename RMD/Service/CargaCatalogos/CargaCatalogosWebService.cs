using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.CargaCatalogos;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.ATC;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.CIM10;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.GalenicForm;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.Package;
using RMD.Shared.Models.CargaCatalogosAPI_DATA.Product;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using JsonSerializer = System.Text.Json.JsonSerializer;

//using RMD.Models.CargaCatalogosWebService;

namespace RMD.Service.CargaCatalogos
{
    public class CargaCatalogosWebService : ICargaCatalogosWebService
    {

        private readonly HttpClient _httpClient;
        private readonly string BaseUrlTemplate;
        private readonly CatalogoWebServiceDbContext _context;
        // Plantilla de URL para cualquier catálogo: {0} = tableName, {1} = page

        public CargaCatalogosWebService(HttpClient httpClient, IConfiguration configuration, CatalogoWebServiceDbContext context)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _context = context;
            // Obtén el valor de "Country" desde la configuración
            var country = configuration["Country"]?.ToLowerInvariant() ?? "mx";

            // Según el país, asigna la URL base correspondiente.
            if (country == "es")
            {
                BaseUrlTemplate = "https://wsdata.vademecum.es/vweb/json/webservices/getTableData_new?columnNameList=*&tableName={0}&orderBy=&c=es&tk=25b6c184df871245181320cb38d1dc34&nbpage=1000&page={1}";
            }
            else // Por defecto, usa "mx"
            {
                BaseUrlTemplate = "https://wsdata.vademecum.es/vweb/json/webservices/getTableData_new?columnNameList=*&tableName={0}&orderBy=&c=mx&tk=25b6c184df871245181320cb38d1dc34&nbpage=1000&page={1}";
            }
        }

        // Única función pública definida en el interface
        public async Task<bool> CargarCatalogosWebService()
        {
            try
            {

                // Capturamos el tiempo de inicio
                
                var unused4 = await CargarGalenicFormAsync("galenicform");
                var unused = await CargarPackageAsync("package");
                var unused1 = await CargarProductAsync("product");
                var unused2 = await CargarCim10CatalogAsync("cim10");
                var unused5 = await CargarCommonNameGroupAtcAsync("commonnamegroup_atc");
                var unused3 = await CargarContainerContentAsync("containercontent");


                // var responseContra = await CargarCim10ContraindicationAsync("cim10_contraindication");
                // var responseIndicationGroup = await CargarCim10IndicationGroupAsync("cim10_indicationgroup");
                // var responseAllergy = await CargarAllergyAsync("allergy");
                // var responseAllergyCross = await CargarAllergyCrossAsync("allergy_cross");
                // var responseAllergyMolecule = await CargarAllergyMoleculeAsync("allergy_molecule");
                // var responseAllergySimplified = await CargarAllergySimplifiedAsync("allergy_simplified");
                // var responseClassdc = await CargarClassdcAsync("classdc");
                // var responseCommonNameGroup = await CargarCommonNameGroupAsync("commonnamegroup");
                // var responseATC = await CargarATCClassAsync("atcclass");
                // var responseComposition = await CargarCommonNameGroupCompositionAsync("commonnamegroup_composition");
                // var responseDrugIntClass = await CargarCommonNameGroupDrugIntClassAsync("commonnamegroup_drugintclass");
                // var responseCommonNameGroupIndication = await CargarCommonNameGroupIndicationAsync("commonnamegroup_indication");
                // var responseCommonNameGroupIndicator = await CargarCommonNameGroupIndicatorAsync("commonnamegroup_indicator");
                // var responseRedunMolecule = await CargarCommonNameGroupRedunMoleculeAsync("commonnamegroup_redun_molecule");
                // var responseCommonNameGroupRoute = await CargarCommonNameGroupRouteAsync("commonnamegroup_route");
                // var responseContraIndicacion = await CargarContraindicationAsync("contraindication");
                // var responseIndGroup = await CargarIndicationGroupAsync("indicationgroup");
                // var responseIndication = await CargarIndicationAsync("indication");
                // var responseDrugInteractionClass = await CargarDrugInteractionClassAsync("druginteractionclass");
                // var responseDrugEntityComposition = await CargarDrugEntityCompositionAsync("drugentity_composition");
                // var responseDrugEntity = await CargarDrugEntityAsync("drugentity");
                // var responseIndGroupIndication = await CargarIndicationGroupIndicationAsync("indicationgroup_indication");
                // var responseMolecule = await CargarMoleculeAsync("molecule");
                //// var responsePackageDrugEntity = await CargarPackageDrugEntityAsync("package_drugentity");

                // var responseProductComposition = await CargarProductCompositionAsync("product_composition");
                // var responseProductForm = await CargarProductFormAsync("product_form");
                // var responseProductRoute = await CargarProductRouteAsync("product_route");
                // var responseRoute = await CargarRouteAsync("route");
                // var responseUCDV = await CargarUCDVAsync("ucdv");
                // var responseUnit = await CargarUnitAsync("unit");

                // if (country == "es")
                // {
                //     var responsePackageDcpf = await CargarPackageDcpfAsync("package_dcpf");
                //     var responseDangerousDrugs = await CargarDangerousDrugsAsync("dangerous_drugs");
                //     var responsePackageIndicator = await CargarPackageIndicatorAsync("package_indicator");
                //     var responseProductIndicator = await CargarProductIndicatorAsync("product_indicator");
                //     var responseDcp = await CargarDcpAsync("dcp");
                //     var responseDcpf = await CargarDcpAsync("dcpf");
                // }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private async Task<bool> CargarPackageAsync(string tableName)
        {
            try
            {
                int page = 1;
                var allRecords = new List<PackageWSDATAModel>();
                var url = string.Format(BaseUrlTemplate, tableName, page);
                var firstResponse = await _httpClient.GetAsync(url);
                firstResponse.EnsureSuccessStatusCode();

                var jsonString = await firstResponse.Content.ReadAsStringAsync();
                jsonString = LimpiarJson(jsonString); // <-- 🔧 limpieza

                var responseObj = JsonSerializer.Deserialize<PackageResponseWSdataModel>(
                    jsonString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                int totalCount = int.Parse(responseObj.Result.Count.First().Total);
                int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
                allRecords.AddRange(responseObj.Result.Table);

                for (page = 2; page <= totalPages; page++)
                {
                    url = string.Format(BaseUrlTemplate, tableName, page);
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    jsonString = await response.Content.ReadAsStringAsync();
                    jsonString = LimpiarJson(jsonString); // <-- 🔧 limpieza

                    responseObj = JsonSerializer.Deserialize<PackageResponseWSdataModel>(
                        jsonString,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    allRecords.AddRange(responseObj.Result.Table);
                }

                var jsonRecords = JsonSerializer.Serialize(allRecords);

                var param = new SqlParameter("@JsonRecords", jsonRecords);
                await _context.Database.ExecuteSqlRawAsync("EXEC Vidal.InsertPackagesWebServiceData @JsonRecords", param);

                return true;
            }
            catch (Exception ex)
            {
                // puedes loguear ex.Message si quieres
                return false;
            }
        }

        private async Task<bool> CargarProductAsync(string tableName)
        {
            try
            {
                int page = 1;
                var allRecords = new List<ProductWSdataModel>();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new SafeNullableIntConverter() }
                };

                var url = string.Format(BaseUrlTemplate, tableName, page);
                var firstResponse = await _httpClient.GetAsync(url);
                firstResponse.EnsureSuccessStatusCode();
                var jsonString = LimpiarJson(await firstResponse.Content.ReadAsStringAsync());
                var responseObj = JsonSerializer.Deserialize<ProductResponseWSdataModel>(jsonString, options);

                int totalCount = int.Parse(responseObj.Result.Count.First().Total);
                int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
                allRecords.AddRange(responseObj.Result.Table.Where(t => t.ProductId != "0"));

                for (page = 2; page <= totalPages; page++)
                {
                    url = string.Format(BaseUrlTemplate, tableName, page);
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    jsonString = LimpiarJson(await response.Content.ReadAsStringAsync());
                    responseObj = JsonSerializer.Deserialize<ProductResponseWSdataModel>(jsonString, options);

                    allRecords.AddRange(responseObj.Result.Table.Where(t => t.ProductId != "0"));
                }

                var jsonRecords = JsonSerializer.Serialize(allRecords);
                var param = new SqlParameter("@JsonRecords", jsonRecords);
                await _context.Database.ExecuteSqlRawAsync("EXEC [VIDAL].[InsertProductosWebServiceData] @JsonRecords", param);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> CargarCim10CatalogAsync(string tableName)
        {
            try
            {
                int page = 1;
                var allRecords = new List<Cim10CatalogWSdataModel>();
                var url = string.Format(BaseUrlTemplate, tableName, page);

                var firstResponse = await _httpClient.GetAsync(url);
                firstResponse.EnsureSuccessStatusCode();
                var jsonString = LimpiarJson(await firstResponse.Content.ReadAsStringAsync());
                var catalogResponse = JsonSerializer.Deserialize<Cim10CatalogResponseWSdataModel>(jsonString);

                int totalCount = int.Parse(catalogResponse.Result.Count.First().Total);
                int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

                allRecords.AddRange(catalogResponse.Result.Table.Where(t => t.Cim10Id != "0"));

                for (page = 2; page <= totalPages; page++)
                {
                    url = string.Format(BaseUrlTemplate, tableName, page);
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    jsonString = LimpiarJson(await response.Content.ReadAsStringAsync());
                    catalogResponse = JsonSerializer.Deserialize<Cim10CatalogResponseWSdataModel>(jsonString);
                    allRecords.AddRange(catalogResponse.Result.Table.Where(t => t.Cim10Id != "0"));
                }

                var jsonRecords = JsonSerializer.Serialize(allRecords);
                var param = new SqlParameter("@JsonRecords", jsonRecords);
                await _context.Database.ExecuteSqlRawAsync("EXEC [Vidal].[InsertCIM10WebServiceData] @JsonRecords", param);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> CargarContainerContentAsync(string tableName)
        {
            try
            {
                int page = 1;
                var allRecords = new List<ContainerContent>();

                var url = string.Format(BaseUrlTemplate, tableName, page);
                var firstResponse = await _httpClient.GetAsync(url);
                firstResponse.EnsureSuccessStatusCode();
                var jsonString = LimpiarJson(await firstResponse.Content.ReadAsStringAsync());
                var containerResponse = JsonSerializer.Deserialize<ContainerContentResponse>(jsonString);

                int totalCount = int.Parse(containerResponse.Result.Count.First().Total);
                int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

                allRecords.AddRange(containerResponse.Result.Table);

                for (page = 2; page <= totalPages; page++)
                {
                    url = string.Format(BaseUrlTemplate, tableName, page);
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    jsonString = LimpiarJson(await response.Content.ReadAsStringAsync());
                    containerResponse = JsonSerializer.Deserialize<ContainerContentResponse>(jsonString);
                    allRecords.AddRange(containerResponse.Result.Table);
                }

                var jsonRecords = JsonSerializer.Serialize(allRecords);
                var param = new SqlParameter("@JsonRecords", jsonRecords);
                await _context.Database.ExecuteSqlRawAsync("EXEC [VIDAL].[InsertContainerContentWebServiceData] @JsonRecords", param);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> CargarCommonNameGroupAtcAsync(string tableName)
        {
            try
            {
                int page = 1;
                var allRecords = new List<VmpAtcWSdataModel>();
                var url = string.Format(BaseUrlTemplate, tableName, page);

                var firstResponse = await _httpClient.GetAsync(url);
                firstResponse.EnsureSuccessStatusCode();

                var jsonString = LimpiarJson(await firstResponse.Content.ReadAsStringAsync());
                var response = JsonSerializer.Deserialize<VmpAtcResponseWSdataModel>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (response?.Result?.Count == null || !response.Result.Count.Any())
                    return false;

                if (!int.TryParse(response.Result.Count.First().Total, out int totalCount))
                    return false;

                int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

                if (response.Result.Table != null)
                    allRecords.AddRange(response.Result.Table);

                for (page = 2; page <= totalPages; page++)
                {
                    url = string.Format(BaseUrlTemplate, tableName, page);
                    var resp = await _httpClient.GetAsync(url);
                    resp.EnsureSuccessStatusCode();

                    jsonString = LimpiarJson(await resp.Content.ReadAsStringAsync());
                    response = JsonSerializer.Deserialize<VmpAtcResponseWSdataModel>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (response?.Result?.Table != null)
                        allRecords.AddRange(response.Result.Table);
                }

                var jsonRecords = JsonSerializer.Serialize(allRecords);
                var param = new SqlParameter("@JsonRecords", jsonRecords);
                await _context.Database.ExecuteSqlRawAsync("EXEC [VIDAL].[InsertVMP_ATCWebServiceData] @JsonRecords", param);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> CargarGalenicFormAsync(string tableName)
        {
            try
            {
                int page = 1;
                var allRecords = new List<GalenicForm>();

                var url = string.Format(BaseUrlTemplate, tableName, page);
                var firstResponse = await _httpClient.GetAsync(url);
                firstResponse.EnsureSuccessStatusCode();
                var jsonString = LimpiarJson(await firstResponse.Content.ReadAsStringAsync());
                var responseObj = JsonSerializer.Deserialize<GalenicFormResponse>(jsonString);

                int totalCount = int.Parse(responseObj.Result.Count.First().Total);
                int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
                allRecords.AddRange(responseObj.Result.Table);

                for (page = 2; page <= totalPages; page++)
                {
                    url = string.Format(BaseUrlTemplate, tableName, page);
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    jsonString = LimpiarJson(await response.Content.ReadAsStringAsync());
                    responseObj = JsonSerializer.Deserialize<GalenicFormResponse>(jsonString);
                    allRecords.AddRange(responseObj.Result.Table);
                }

                var jsonRecords = JsonSerializer.Serialize(allRecords);
                var param = new SqlParameter("@JsonRecords", jsonRecords);
                await _context.Database.ExecuteSqlRawAsync("EXEC [VIDAL].[InsertGalenicFormWebServiceData] @JsonRecords", param);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string LimpiarJson(string json)
        {
            return Regex.Replace(json, @"[\u0000-\u001F]", " "); // Limpia caracteres de control: tabs, nulls, etc.
        }
        private class SafeNullableIntConverter : JsonConverter<int?>
        {
            public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                if (reader.TokenType == JsonTokenType.String)
                {
                    var str = reader.GetString();
                    return int.TryParse(str, out int val) ? val : null;
                }

                if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int result))
                    return result;

                return null;
            }

            public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                    writer.WriteNumberValue(value.Value);
                else
                    writer.WriteNullValue();
            }
        }

        //private async Task<bool> CargarCim10ContraindicationAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Cim10Contraindication>();

        //        // Construir la URL usando el tableName
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var contraindicationResponse = JsonConvert.DeserializeObject<Cim10ContraindicationResponse>(jsonString);

        //        int totalCount = int.Parse(contraindicationResponse.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

        //        // Agrega registros de la primera página
        //        allRecords.AddRange(contraindicationResponse.Result.Table);

        //        // Iterar desde la 2da página hasta la última
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            contraindicationResponse = JsonConvert.DeserializeObject<Cim10ContraindicationResponse>(jsonString);
        //            allRecords.AddRange(contraindicationResponse.Result.Table);
        //        }

        //        // Serializa la lista en JSON
        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);

        //        // Llama al SP correspondiente
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaCim10Contraindication @JsonRecords", param);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //private async Task<bool> CargarCim10IndicationGroupAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Cim10IndicationGroup>();

        //        // Construir la URL usando el tableName
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var indicationResponse = JsonConvert.DeserializeObject<Cim10IndicationGroupResponse>(jsonString);

        //        int totalCount = int.Parse(indicationResponse.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

        //        // Agrega registros de la primera página
        //        allRecords.AddRange(indicationResponse.Result.Table);

        //        // Recorrer las páginas restantes
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            indicationResponse = JsonConvert.DeserializeObject<Cim10IndicationGroupResponse>(jsonString);
        //            allRecords.AddRange(indicationResponse.Result.Table);
        //        }

        //        // Serializa la lista en JSON
        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaCim10IndicationGroup @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //private async Task<bool> CargarClassdcAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Classdc>();

        //        // Construir la URL usando el tableName
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var classdcResponse = JsonConvert.DeserializeObject<ClassdcResponse>(jsonString);

        //        int totalCount = int.Parse(classdcResponse.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

        //        // Agregar registros de la primera página
        //        allRecords.AddRange(classdcResponse.Result.Table);

        //        // Recorrer las páginas restantes
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            classdcResponse = JsonConvert.DeserializeObject<ClassdcResponse>(jsonString);
        //            allRecords.AddRange(classdcResponse.Result.Table);
        //        }

        //        // Serializar la lista a JSON
        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaClassdc @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarATCClassAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<ATCClass>();

        //        // Construir la URL usando el tableName
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var atcResponse = JsonConvert.DeserializeObject<ATCClassResponse>(jsonString);

        //        int totalCount = int.Parse(atcResponse.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

        //        // Agregar registros de la primera página
        //        allRecords.AddRange(atcResponse.Result.Table);

        //        // Recorrer las páginas restantes
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            atcResponse = JsonConvert.DeserializeObject<ATCClassResponse>(jsonString);
        //            allRecords.AddRange(atcResponse.Result.Table);
        //        }


        //        // Serializar la lista a JSON
        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaATCClass @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //private async Task<bool> CargarContraindicationAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Contraindication>();

        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var contraindicationResponse = JsonConvert.DeserializeObject<ContraindicationResponse>(jsonString);

        //        int totalCount = int.Parse(contraindicationResponse.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

        //        // Agregar registros de la primera página
        //        allRecords.AddRange(contraindicationResponse.Result.Table);

        //        // Recorrer las páginas restantes
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            contraindicationResponse = JsonConvert.DeserializeObject<ContraindicationResponse>(jsonString);
        //            allRecords.AddRange(contraindicationResponse.Result.Table);
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaContraindication @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarDangerousDrugsAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<DangerousDrugs>();

        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var drugsResponse = JsonConvert.DeserializeObject<DangerousDrugsResponse>(jsonString);

        //        int totalCount = int.Parse(drugsResponse.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);

        //        allRecords.AddRange(drugsResponse.Result.Table);

        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            drugsResponse = JsonConvert.DeserializeObject<DangerousDrugsResponse>(jsonString);
        //            allRecords.AddRange(drugsResponse.Result.Table);
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaDangerousDrugs @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarIndicationGroupAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<IndicationGroup>();

        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<IndicationGroupResponse>(jsonString);

        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table);

        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<IndicationGroupResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table);
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaIndicationGroup @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarIndicationAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Indication>();

        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<IndicationResponse>(jsonString);

        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table);

        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<IndicationResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table);
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaIndication @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //private async Task<bool> CargarDrugInteractionClassAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<DrugInteractionClass>();

        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<DrugInteractionClassResponse>(jsonString);

        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table);

        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<DrugInteractionClassResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table);
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaDrugInteractionClass @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarDrugEntityCompositionAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<DrugEntityComposition>();

        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<DrugEntityCompositionResponse>(jsonString);

        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table);

        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<DrugEntityCompositionResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table);
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaDrugEntity_Composition @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarDrugEntityAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<DrugEntity>();

        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<DrugEntityResponse>(jsonString);

        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table.Where(t => t.DrugEntityId != "0"));

        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<DrugEntityResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table.Where(t => t.DrugEntityId != "0"));
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaDrugEntity @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarIndicationGroupIndicationAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<IndicationGroupIndication>();
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<IndicationGroupIndicationResponse>(jsonString);

        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table);

        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<IndicationGroupIndicationResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table);
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaIndicationGroup_Indication @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarMoleculeAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Molecule>();
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<MoleculeResponse>(jsonString);

        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table);

        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<MoleculeResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table);
        //        }

        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaMolecule @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}      

        //private async Task<bool> CargarRouteAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Routes>();
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<RouteResponse>(jsonString);
        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table);
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<RouteResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table);
        //        }
        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaRoute @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarUCDVAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<UCDV>();
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var responseObj = JsonConvert.DeserializeObject<UCDVResponse>(jsonString);
        //        int totalCount = int.Parse(responseObj.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(responseObj.Result.Table);
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            responseObj = JsonConvert.DeserializeObject<UCDVResponse>(jsonString);
        //            allRecords.AddRange(responseObj.Result.Table);
        //        }
        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaUCDV @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarDcpAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Dcp>();

        //        // Construir la URL (tableName = "dcp")
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var dcpResponse = JsonConvert.DeserializeObject<DcpResponse>(jsonString);

        //        int totalCount = int.Parse(dcpResponse.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(dcpResponse.Result.Table);

        //        // Itera las páginas restantes
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            dcpResponse = JsonConvert.DeserializeObject<DcpResponse>(jsonString);
        //            allRecords.AddRange(dcpResponse.Result.Table);
        //        }

        //        // Serializa y ejecuta el SP
        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaDcp @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private async Task<bool> CargarDcpfAsync(string tableName)
        //{
        //    try
        //    {
        //        int page = 1;
        //        var allRecords = new List<Dcpf>();

        //        // Construir la URL para "dcpf"
        //        var url = string.Format(BaseUrlTemplate, tableName, page);
        //        var firstResponse = await _httpClient.GetAsync(url);
        //        firstResponse.EnsureSuccessStatusCode();
        //        var jsonString = await firstResponse.Content.ReadAsStringAsync();
        //        var dcpfResponse = JsonConvert.DeserializeObject<DcpfResponse>(jsonString);

        //        int totalCount = int.Parse(dcpfResponse.Result.Count.First().Total);
        //        int totalPages = (int)Math.Ceiling(totalCount / 1000.0);
        //        allRecords.AddRange(dcpfResponse.Result.Table);

        //        // Recorrer las páginas restantes
        //        for (page = 2; page <= totalPages; page++)
        //        {
        //            url = string.Format(BaseUrlTemplate, tableName, page);
        //            var response = await _httpClient.GetAsync(url);
        //            response.EnsureSuccessStatusCode();
        //            jsonString = await response.Content.ReadAsStringAsync();
        //            dcpfResponse = JsonConvert.DeserializeObject<DcpfResponse>(jsonString);
        //            allRecords.AddRange(dcpfResponse.Result.Table);
        //        }

        //        // Serializar y ejecutar el SP
        //        var jsonRecords = JsonConvert.SerializeObject(allRecords);
        //        var param = new SqlParameter("@JsonRecords", jsonRecords);
        //        await _context.Database.ExecuteSqlRawAsync("EXEC VIDAL.CargaDcpf @JsonRecords", param);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

    }
}
