//using RMD.Models.Vidal.CargaCatalogos;

//namespace RMD.Extensions.Vidal.CargaCatalogos
//{
//    public static class LinkProcessingExtensions
//    {
//        public static async Task ProcessLinksAsync(this List<VMPModelLink> modelLinkList, HttpClient httpClient, string basicUrl, string appId, string appKey, string startPage, string pageSize)
//        {
//            foreach (var modelLink in modelLinkList)
//            {
//                foreach (var link in modelLink.Links)
//                {
//                    // Construir la URL
//                    string requestUrl = BuildRequestUrl(basicUrl, link.Href, startPage, pageSize, appId, appKey);

//                    var response = await httpClient.GetAsync(requestUrl);
//                    response.EnsureSuccessStatusCode();

//                    var xmlContent = await response.Content.ReadAsStringAsync();

//                    // Llamar al método para procesar el link por el título
//                    await ProcessLinkByTitle(link.Title.ToUpper(), xmlContent);
//                }
//            }
//        }

//        private static string BuildRequestUrl(string basicUrl, string href, string startPage, string pageSize, string appId, string appKey)
//        {
//            if (char.IsDigit(href.Last()))
//            {
//                return $"{basicUrl}{href}&start-page={startPage}&page-size={pageSize}&app_id={appId}&app_key={appKey}";
//            }
//            return $"{basicUrl}{href}?start-page={startPage}&page-size={pageSize}&app_id={appId}&app_key={appKey}";
//        }

//        private static async Task ProcessLinkByTitle(string title, string xmlContent)
//        {
//            switch (title)
//            {
//                case "PRODUCTS":
//                    var products = xmlContent.ParseProductXmlToModelList();
//                    await SaveProductsAsync(products);
//                    break;

//                case "ATC_CLASSIFICATION":
//                    var atcClassifications = xmlContent.ParseAtcClassificationXmlToModelList();
//                    await SaveAtcClassificationsAsync(atcClassifications);
//                    break;

//                case "MOLECULES":
//                    var molecules = xmlContent.ParseMoleculeXmlToModelList();
//                    await SaveMoleculesAsync(molecules);
//                    break;

//                case "UNITS":
//                    var units = xmlContent.ParseUnitsXmlToModelList();
//                    await SaveUnitsAsync(units);
//                    break;

//                case "CONTRAINDICATION":
//                    var contraindications = xmlContent.ParseContraindicationXmlToModelList();
//                    await SaveContraindicationsAsync(contraindications);
//                    break;

//                case "PHYSICO_CHEMICAL_INTERACTIONS":
//                    var physicoChemicalInteractions = xmlContent.ParsePhysicoChemicalInteractionsXmlToModelList();
//                    await SavePhysicoChemicalInteractionsAsync(physicoChemicalInteractions);
//                    break;

//                case "ROUTES":
//                    var routes = xmlContent.ParseRoutesXmlToModelList();
//                    await SaveRoutesAsync(routes);
//                    break;

//                case "INDICATORS":
//                    var indicators = xmlContent.ParseIndicatorsXmlToModelList();
//                    await SaveIndicatorsAsync(indicators);
//                    break;

//                case "INDICATIONS":
//                    var indications = xmlContent.ParseIndicationsXmlToModelList();
//                    await SaveIndicationsAsync(indications);
//                    break;

//                case "SIDE_EFFECTS":
//                    var sideEffects = xmlContent.ParseSideEffectsXmlToModelList();
//                    await SaveSideEffectsAsync(sideEffects);
//                    break;

//                case "ALDS":
//                    var alds = xmlContent.ParseAldsXmlToModelList();
//                    await SaveAldsAsync(alds);
//                    break;

//                case "UCDVS":
//                    var ucdvs = xmlContent.ParseUcdvsXmlToModelList();
//                    await SaveUcdvsAsync(ucdvs);
//                    break;

//                case "UCDS":
//                    var ucds = xmlContent.ParseUcdsXmlToModelList();
//                    await SaveUcdsAsync(ucds);
//                    break;

//                case "PRESCRIBABLES":
//                    var prescribables = xmlContent.ParsePrescribablesXmlToModelList();
//                    await SavePrescribablesAsync(prescribables);
//                    break;

//                case "ALLERGIES":
//                    var allergies = xmlContent.ParseAllergiesXmlToModelList();
//                    await SaveAllergiesAsync(allergies);
//                    break;

//                default:
//                    Console.WriteLine($"No se ha implementado manejo específico para el link: {title}");
//                    break;
//            }
//        }

//        private static Task SaveProductsAsync(List<ProductModel> products) => Task.CompletedTask;
//        private static Task SaveAtcClassificationsAsync(List<AtcClassificationModel> atcClassifications) => Task.CompletedTask;
//        private static Task SaveMoleculesAsync(List<MoleculeModel> molecules) => Task.CompletedTask;
//        private static Task SaveUnitsAsync(List<UnitModel> units) => Task.CompletedTask;
//        private static Task SaveContraindicationsAsync(List<ContraindicationModel> contraindications) => Task.CompletedTask;
//        private static Task SavePhysicoChemicalInteractionsAsync(List<PhysicoChemicalInteractionModel> physicoChemicalInteractions) => Task.CompletedTask;
//        private static Task SaveRoutesAsync(List<RouteModel> routes) => Task.CompletedTask;
//        private static Task SaveIndicatorsAsync(List<IndicatorModel> indicators) => Task.CompletedTask;
//        private static Task SaveIndicationsAsync(List<IndicationModel> indications) => Task.CompletedTask;
//        private static Task SaveSideEffectsAsync(List<SideEffectModel> sideEffects) => Task.CompletedTask;
//        private static Task SaveAldsAsync(List<AldModel> alds) => Task.CompletedTask;
//        private static Task SaveUcdvsAsync(List<UcdvModel> ucdvs) => Task.CompletedTask;
//        private static Task SaveUcdsAsync(List<UcdModel> ucds) => Task.CompletedTask;
//        private static Task SavePrescribablesAsync(List<PrescribableModel> prescribables) => Task.CompletedTask;
//        private static Task SaveAllergiesAsync(List<AllergyModel> allergies) => Task.CompletedTask;
//    }

//}
