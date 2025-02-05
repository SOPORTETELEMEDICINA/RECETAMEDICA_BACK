using RMD.Models.CargaCatalogos;
using System.Xml.Linq;

namespace RMD.Extensions.CargaCatalogos
{
    public static class XmlExtensionsByCargaCatalogos
    {
        public static List<VMPModel> ParseVMPXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var vmpModelList = new List<VMPModel>();

                var vmpEntries = document.Descendants(atom + "entry");

                foreach (var entry in vmpEntries)
                {
                    var vmp = new VMPModel
                    {
                        IdVMP = (int)entry.Element(ns + "id"),
                        Name = (string)entry.Element(ns + "name"),
                        ActivePrinciples = (string)entry.Element(ns + "activePrinciples"),
                        GalenicFormVidalId = (int)entry.Element(ns + "galenicForm").Attribute("vidalId"),
                        GalenicForm = (string)entry.Element(ns + "galenicForm"),
                        RegulatoryGenericPrescription = (bool)entry.Element(ns + "regulatoryGenericPrescription"),
                        IdVTM = entry.Elements(atom + "link")
                                     .Where(l => (string)l.Attribute("title") == "VTM")
                                     .Select(l => int.Parse(((string)l.Attribute("href")).Split('/').Last()))
                                     .FirstOrDefault(),
                        UpdatedDate = (DateTime)entry.Element(atom + "updated"),

                        // Asignar los links adicionales
                        VMP = entry.Elements(atom + "link")
                                    .Where(l => (string)l.Attribute("title") == "VMP")
                                    .Select(l => (string)l.Attribute("href"))
                                    .FirstOrDefault(),

                        PRODUCTS = entry.Elements(atom + "link")
                                        .Where(l => (string)l.Attribute("title") == "PRODUCTS")
                                        .Select(l => (string)l.Attribute("href"))
                                        .FirstOrDefault(),

                        ATC_CLASSIFICATION = entry.Elements(atom + "link")
                                                  .Where(l => (string)l.Attribute("title") == "ATC_CLASSIFICATION")
                                                  .Select(l => (string)l.Attribute("href"))
                                                  .FirstOrDefault(),

                        MOLECULES = entry.Elements(atom + "link")
                                         .Where(l => (string)l.Attribute("title") == "MOLECULES")
                                         .Select(l => (string)l.Attribute("href"))
                                         .FirstOrDefault(),

                        UNITS = entry.Elements(atom + "link")
                                     .Where(l => (string)l.Attribute("title") == "UNITS")
                                     .Select(l => (string)l.Attribute("href"))
                                     .FirstOrDefault(),

                        CONTRAINDICATION = entry.Elements(atom + "link")
                                                .Where(l => (string)l.Attribute("title") == "CONTRAINDICATION")
                                                .Select(l => (string)l.Attribute("href"))
                                                .FirstOrDefault(),

                        PHYSICO_CHEMICAL_INTERACTIONS = entry.Elements(atom + "link")
                                                            .Where(l => (string)l.Attribute("title") == "PHYSICO_CHEMICAL_INTERACTIONS")
                                                            .Select(l => (string)l.Attribute("href"))
                                                            .FirstOrDefault(),

                        ROUTES = entry.Elements(atom + "link")
                                      .Where(l => (string)l.Attribute("title") == "ROUTES")
                                      .Select(l => (string)l.Attribute("href"))
                                      .FirstOrDefault(),

                        INDICATORS = entry.Elements(atom + "link")
                                          .Where(l => (string)l.Attribute("title") == "INDICATORS")
                                          .Select(l => (string)l.Attribute("href"))
                                          .FirstOrDefault(),

                        INDICATIONS = entry.Elements(atom + "link")
                                           .Where(l => (string)l.Attribute("title") == "INDICATIONS")
                                           .Select(l => (string)l.Attribute("href"))
                                           .FirstOrDefault(),

                        SIDE_EFFECTS = entry.Elements(atom + "link")
                                            .Where(l => (string)l.Attribute("title") == "SIDE_EFFECTS")
                                            .Select(l => (string)l.Attribute("href"))
                                            .FirstOrDefault(),

                        ALDS = entry.Elements(atom + "link")
                                    .Where(l => (string)l.Attribute("title") == "ALDS")
                                    .Select(l => (string)l.Attribute("href"))
                                    .FirstOrDefault(),

                        UCDVS = entry.Elements(atom + "link")
                                     .Where(l => (string)l.Attribute("title") == "UCDVS")
                                     .Select(l => (string)l.Attribute("href"))
                                     .FirstOrDefault(),

                        UCDS = entry.Elements(atom + "link")
                                    .Where(l => (string)l.Attribute("title") == "UCDS")
                                    .Select(l => (string)l.Attribute("href"))
                                    .FirstOrDefault(),

                        PRESCRIBABLES = entry.Elements(atom + "link")
                                             .Where(l => (string)l.Attribute("title") == "PRESCRIBABLES")
                                             .Select(l => (string)l.Attribute("href"))
                                             .FirstOrDefault(),

                        ALLERGIES = entry.Elements(atom + "link")
                                         .Where(l => (string)l.Attribute("title") == "ALLERGIES")
                                         .Select(l => (string)l.Attribute("href"))
                                         .FirstOrDefault(),

                        OPT_DOCUMENT = entry.Elements(atom + "link")
                                            .Where(l => (string)l.Attribute("title") == "OPT_DOCUMENT")
                                            .Select(l => (string)l.Attribute("href"))
                                            .FirstOrDefault(),

                        DOCUMENTS = entry.Elements(atom + "link")
                                         .Where(l => (string)l.Attribute("title") == "DOCUMENTS")
                                         .Select(l => (string)l.Attribute("href"))
                                         .FirstOrDefault()
                    };

                    vmpModelList.Add(vmp);
                }

                return vmpModelList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<VMPModel>();
            }
        }

        public static List<ProductModel> ParseProductsXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);

                var productEntries = document.Descendants(atom + "entry").Select(entry => new ProductModel
                {
                    IdProduct = int.Parse(((string)entry.Element(ns + "id")).Split('/').Last()),
                    Summary = (string)entry.Element(atom + "summary"),
                    Name = (string)entry.Element(ns + "name"),
                    IdItemType = (string)entry.Element(ns + "itemType")?.Attribute("name"),
                    IdMarketStatus = (string)entry.Element(ns + "marketStatus")?.Attribute("name"),
                    HasPublishedDoc = (bool?)entry.Element(ns + "hasPublishedDoc") ?? false,
                    WithoutPrescription = (bool?)entry.Element(ns + "withoutPrescription") ?? false,
                    IdAmmType = int.Parse(((string)entry.Element(ns + "ammType")?.Attribute("vidalId")) ?? "0"),
                    BestDocType = (string)entry.Element(ns + "bestDocType")?.Attribute("name"),
                    SafetyAlert = (bool?)entry.Element(ns + "safetyAlert") ?? false,
                    IdCompany = int.Parse(((string)entry.Element(ns + "company")?.Attribute("vidalId")) ?? "0"),
                    CompanyName = (string)entry.Element(ns + "company"),
                    TypeCompany = (string)entry.Element(ns + "company")?.Attribute("type"),
                    IdVmp = int.Parse(((string)entry.Element(ns + "vmp")?.Attribute("vidalId")) ?? "0"),
                    IdGalenicForm = int.Parse(((string)entry.Element(ns + "galenicForm")?.Attribute("vidalId")) ?? "0"),
                    GalenicForm = (string)entry.Element(ns + "galenicForm"),
                    VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated")),
                    // Asignación de los enlaces
                    PACKAGES = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "PACKAGES")?.Attribute("href")?.Value,
                    MOLECULES = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "MOLECULES")?.Attribute("href")?.Value,
                    // Repite esto para cada enlace...
                    ACTIVE_EXCIPIENTS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "ACTIVE_EXCIPIENTS")?.Attribute("href")?.Value,
                    RECOS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "RECOS")?.Attribute("href")?.Value,
                    FOREIGN_PRODUCTS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "FOREIGN_PRODUCTS")?.Attribute("href")?.Value,
                    INDICATIONS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "INDICATIONS")?.Attribute("href")?.Value,
                    CONTRAINDICATION = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "CONTRAINDICATION")?.Attribute("href")?.Value,
                    RESTRICTED_PRESCRIPTIONS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "RESTRICTED_PRESCRIPTIONS")?.Attribute("href")?.Value,
                    PDS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "PDS")?.Attribute("href")?.Value,
                    UCDS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "UCDS")?.Attribute("href")?.Value,
                    UNITS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "UNITS")?.Attribute("href")?.Value,
                    FOOD_INTERACTIONS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "FOOD_INTERACTIONS")?.Attribute("href")?.Value,
                    PHYSICO_CHEMICAL_INTERACTIONS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "PHYSICO_CHEMICAL_INTERACTIONS")?.Attribute("href")?.Value,
                    ROUTES = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "ROUTES")?.Attribute("href")?.Value,
                    INDICATORS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "INDICATORS")?.Attribute("href")?.Value,
                    SIDE_EFFECTS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "SIDE_EFFECTS")?.Attribute("href")?.Value,
                    ALDS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "ALDS")?.Attribute("href")?.Value,
                    UCDVS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "UCDVS")?.Attribute("href")?.Value,
                    ALLERGIES = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "ALLERGIES")?.Attribute("href")?.Value
                }).ToList();

                return productEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<ProductModel>();
            }
        }

        public static List<PackageModel> ParsePackagesXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);

                var packageEntries = document.Descendants(atom + "entry").Select(entry => new PackageModel
                {
                    IdPackage = int.Parse(((string)entry.Element(ns + "id")).Split('/').Last()), // Parsear el vidal:id correctamente
                    Name = (string)entry.Element(ns + "name"), // Nombre del paquete
                    Summary = (string)entry.Element(atom + "summary"), // Resumen
                    ProductId = int.Parse(((string)entry.Element(ns + "productId")) ?? "0"), // ID del producto
                    MarketStatus = (string)entry.Element(ns + "marketStatus")?.Attribute("name"), // Estado del mercado
                    Otc = (bool?)entry.Element(ns + "otc") ?? false, // Medicamento OTC
                    IsCeps = (bool?)entry.Element(ns + "isCeps") ?? false, // Es CEPS
                    DrugId = int.Parse(((string)entry.Element(ns + "drugId")) ?? "0"), // Drug ID
                    Cip13 = (string)entry.Element(ns + "cip13"), // CIP13
                    ShortLabel = (string)entry.Element(ns + "shortLabel"), // Etiqueta corta
                    Tfr = (bool?)entry.Element(ns + "tfr") ?? false, // TFR
                    IdCompany = int.Parse(((string)entry.Element(ns + "company")?.Attribute("vidalId")) ?? "0"), // ID de la compañía
                    CompanyName = (string)entry.Element(ns + "company"), // Nombre de la compañía
                    NarcoticPrescription = (bool?)entry.Element(ns + "narcoticPrescription") ?? false, // Prescripción narcótica
                    SafetyAlert = (bool?)entry.Element(ns + "safetyAlert") ?? false, // Alerta de seguridad
                    WithoutPrescription = (bool?)entry.Element(ns + "withoutPrescription") ?? false, // Sin receta
                    IdGalenicForm = int.Parse(((string)entry.Element(ns + "galenicForm")?.Attribute("vidalId")) ?? "0"), // ID de la forma galénica
                    GalenicForm = (string)entry.Element(ns + "galenicForm"), // Descripción de la forma galénica
                    UcdCode13 = (string)entry.Element(ns + "ucd")?.Attribute("code13"), // UCD Code 13
                    UcdCode7 = (string)entry.Element(ns + "ucd")?.Attribute("code7"), // UCD Code 7
                    UcdId = int.Parse(((string)entry.Element(ns + "ucd")?.Attribute("vidalId")) ?? "0"), // UCD ID
                    VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated")), // Fecha de actualización

                    // Mapear los enlaces a las nuevas propiedades
                    LargerPacks = entry.Elements(atom + "link")
                                        .FirstOrDefault(l => (string)l.Attribute("title") == "LARGER_PACKS")?.Attribute("href")?.Value,
                    AffiliationCenter = entry.Elements(atom + "link")
                                             .FirstOrDefault(l => (string)l.Attribute("title") == "AFFILIATION_CENTER")?.Attribute("href")?.Value,
                    Pds = entry.Elements(atom + "link")
                               .FirstOrDefault(l => (string)l.Attribute("title") == "PDS")?.Attribute("href")?.Value,
                    PricingSchedule = entry.Elements(atom + "link")
                                           .FirstOrDefault(l => (string)l.Attribute("title") == "PRICING_SCHEDULE")?.Attribute("href")?.Value,
                    Units = entry.Elements(atom + "link")
                                 .FirstOrDefault(l => (string)l.Attribute("title") == "UNITS")?.Attribute("href")?.Value,
                    Routes = entry.Elements(atom + "link")
                                  .FirstOrDefault(l => (string)l.Attribute("title") == "ROUTES")?.Attribute("href")?.Value,
                    Indicators = entry.Elements(atom + "link")
                                      .FirstOrDefault(l => (string)l.Attribute("title") == "INDICATORS")?.Attribute("href")?.Value,
                    Indications = entry.Elements(atom + "link")
                                       .FirstOrDefault(l => (string)l.Attribute("title") == "INDICATIONS")?.Attribute("href")?.Value,
                    SideEffects = entry.Elements(atom + "link")
                                       .FirstOrDefault(l => (string)l.Attribute("title") == "SIDE_EFFECTS")?.Attribute("href")?.Value,
                    Alds = entry.Elements(atom + "link")
                                .FirstOrDefault(l => (string)l.Attribute("title") == "ALDS")?.Attribute("href")?.Value,
                    VatExcAffiliationCenter = entry.Elements(atom + "link")
                                                .FirstOrDefault(l => (string)l.Attribute("title") == "VAT_EXC_AFFILIATION_CENTER")?.Attribute("href")?.Value,
                    RefundIndications = entry.Elements(atom + "link")
                                             .FirstOrDefault(l => (string)l.Attribute("title") == "REFUND_INDICATIONS")?.Attribute("href")?.Value,
                    OptDocument = entry.Elements(atom + "link")
                                       .FirstOrDefault(l => (string)l.Attribute("title") == "OPT_DOCUMENT")?.Attribute("href")?.Value,
                    Document = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "DOCUMENT")?.Attribute("href")?.Value,
                    Ucd = entry.Elements(atom + "link")
                               .FirstOrDefault(l => (string)l.Attribute("title") == "UCD")?.Attribute("href")?.Value,
                }).ToList();

                return packageEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<PackageModel>();
            }
        }

        public static List<UnitModel> ParseUnitsXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var unitList = new List<UnitModel>();

                var unitEntries = document.Descendants(atom + "entry");

                foreach (var entry in unitEntries)
                {
                    var unit = new UnitModel
                    {
                        IdUnit = (int)entry.Element(ns + "unitId"), // Parseo del ID de la unidad
                        Name = (string)entry.Element(ns + "name"), // Nombre de la unidad
                        SingularName = (string)entry.Element(ns + "singularName"), // Nombre singular de la unidad
                        Conversion = (string)entry.Element(ns + "parentConversionRate"), // Cadena de conversión
                        Denominator = (int)entry.Element(ns + "parentConversionRate").Attribute("denominator"), // Denominador
                        Numerator = decimal.Parse((string)entry.Element(ns + "parentConversionRate").Attribute("numerator")), // Numerador como decimal
                        ParentUnitId = (int?)entry.Element(ns + "parentConversionRate").Attribute("unitId"), // ID de la unidad padre, puede ser nullable
                        VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated")) // Fecha de actualización
                    };

                    unitList.Add(unit);
                }

                return unitList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<UnitModel>();
            }
        }

        public static List<AllergyModel> ParseAllergiesXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var allergyList = new List<AllergyModel>();

                var allergyEntries = document.Descendants(atom + "entry");

                foreach (var entry in allergyEntries)
                {
                    var allergy = new AllergyModel
                    {
                        IdAllergy = (int)entry.Element(ns + "id"),
                        Name = (string)entry.Element(ns + "name"),
                        VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated")),
                        MoleculesLink = entry.Elements(atom + "link")
                                             .FirstOrDefault(l => (string)l.Attribute("title") == "MOLECULES")
                                             ?.Attribute("href")?.Value // Obtenemos el enlace de moléculas
                    };

                    allergyList.Add(allergy);
                }

                return allergyList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<AllergyModel>();
            }
        }

        public static List<MoleculeModel> ParseMoleculesXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var moleculeList = new List<MoleculeModel>();

                var moleculeEntries = document.Descendants(atom + "entry");

                foreach (var entry in moleculeEntries)
                {
                    var molecule = new MoleculeModel
                    {
                        IdMolecule = (int)entry.Element(ns + "id"),
                        Name = (string)entry.Element(ns + "name"),
                        SafetyAlert = (bool)entry.Element(ns + "safetyAlert"),
                        Homeopathy = (bool)entry.Element(ns + "homeopathy"),
                        Role = (string)entry.Element(ns + "role")?.Attribute("name"),
                        VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated"))
                    };

                    moleculeList.Add(molecule);
                }

                return moleculeList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<MoleculeModel>();
            }
        }

        public static List<RouteModel> ParseRoutesXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var routeList = new List<RouteModel>();

                var routeEntries = document.Descendants(atom + "entry");

                foreach (var entry in routeEntries)
                {
                    var route = new RouteModel
                    {
                        IdRoute = (int)entry.Element(ns + "routeId"),
                        Name = (string)entry.Element(ns + "name"),
                        Systemic = (bool)entry.Element(ns + "systemic"),
                        Topical = (bool)entry.Element(ns + "topical"),
                        ParentId = (int?)entry.Element(ns + "parentId"),
                        VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated"))
                    };

                    routeList.Add(route);
                }

                return routeList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<RouteModel>();
            }
        }

        public static List<CIM10Model> ParseCIM10XmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var cim10List = new List<CIM10Model>();

                var cim10Entries = document.Descendants(atom + "entry");

                foreach (var entry in cim10Entries)
                {
                    var cim10 = new CIM10Model
                    {
                        IdCIM10 = (int)entry.Element(ns + "id"), // ID de CIM10
                        Name = (string)entry.Element(ns + "name"), // Nombre de CIM10
                        Code = (string)entry.Element(ns + "code"), // Código de CIM10
                        Summary = (string)entry.Element(atom + "summary"), // Resumen de CIM10
                        UpdatedDate = (DateTime)entry.Element(atom + "updated"), // Fecha de actualización

                        // Nuevos campos para los links
                        ALDSLink = (string)entry.Elements(atom + "link")
                                   .Where(l => (string)l.Attribute("title") == "ALDS")
                                   .Select(l => (string)l.Attribute("href"))
                                   .FirstOrDefault(),
                        ChildrenLink = (string)entry.Elements(atom + "link")
                                       .Where(l => (string)l.Attribute("title") == "CHILDREN")
                                       .Select(l => (string)l.Attribute("href"))
                                       .FirstOrDefault()
                    };

                    cim10List.Add(cim10);
                }

                return cim10List;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<CIM10Model>();
            }
        }

        public static List<VTMModel> ParseVTMsXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var vtmList = new List<VTMModel>();

                var vtmEntries = document.Descendants(atom + "entry");

                foreach (var entry in vtmEntries)
                {
                    var vtm = new VTMModel
                    {
                        IdVTM = (int)entry.Element(ns + "id"),
                        Name = (string)entry.Element(ns + "name"),
                        Summary = (string)entry.Element(atom + "summary"),
                        UpdatedDate = (DateTime)entry.Element(atom + "updated")
                    };

                    vtmList.Add(vtm);
                }

                return vtmList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<VTMModel>();
            }
        }

        public static List<ATCClassificationModel> ParseATCClassificationXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var atcClassificationList = new List<ATCClassificationModel>();

                var atcEntries = document.Descendants(atom + "entry");

                foreach (var entry in atcEntries)
                {
                    var idElement = entry.Element(ns + "id");
                    var nameElement = entry.Element(ns + "name");
                    var codeElement = entry.Element(ns + "code");
                    var updatedElement = entry.Element(atom + "updated");

                    // Verificación de valores nulos o faltantes
                    if (idElement == null || nameElement == null || updatedElement == null)
                    {
                        // Si faltan estos elementos esenciales, salta al siguiente entry
                        continue;
                    }

                    var atc = new ATCClassificationModel
                    {
                        IdATC = int.TryParse(idElement.Value, out var idAtc) ? idAtc : 0, // Verificación segura de int
                        Name = nameElement.Value,
                        Code = codeElement?.Value ?? string.Empty, // Code puede ser opcional
                        UpdatedDate = DateTime.TryParse(updatedElement.Value, out var updatedDate) ? updatedDate : DateTime.MinValue,
                        // Para ParentId, obtenemos la última parte del href
                        ParentId = entry.Elements(atom + "link")
                                .Where(l => (string)l.Attribute("title") == "PARENT")
                                .Select(l => int.TryParse(((string)l.Attribute("href")).Split('/').Last(), out var parentId) ? parentId : 0)
                                .FirstOrDefault(),

                        // Para ChildId, obtenemos la penúltima parte del href antes de "children"
                        ChildId = entry.Elements(atom + "link")
                                .Where(l => (string)l.Attribute("title") == "CHILDREN")
                                .Select(l =>
                                {
                                    var hrefParts = ((string)l.Attribute("href"))?.Split('/');
                                    return hrefParts != null && hrefParts.Length > 1 && int.TryParse(hrefParts[^2], out var childId)
                                        ? childId
                                        : 0;
                                })
                                .FirstOrDefault()
                    };

                    atcClassificationList.Add(atc);
                }

                return atcClassificationList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML de ATC Classification: {ex.Message}");
                return new List<ATCClassificationModel>();
            }
        }

        public static List<UCDVModel> ParseUCDVXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var ucdvList = new List<UCDVModel>();

                var ucdvEntries = document.Descendants(atom + "entry");

                foreach (var entry in ucdvEntries)
                {
                    var ucdv = new UCDVModel
                    {
                        IdUCDV = int.Parse(entry.Element(ns + "id")?.Value ?? "0"),
                        Summary = (string)entry.Element(atom + "summary"),
                        Name = (string)entry.Element(ns + "name"),
                        IdConditioningUnit = int.Parse(entry.Element(ns + "conditioningUnit")?.Attribute("vidalId")?.Value ?? "0"),
                        ConditioningUnit = (string)entry.Element(ns + "conditioningUnit"),
                        Quantity = decimal.Parse(entry.Element(ns + "quantity")?.Value ?? "0"),
                        QuantityUnitId = int.Parse(entry.Element(ns + "quantityUnit")?.Attribute("vidalId")?.Value ?? "0"),
                        QuantityUnit = (string)entry.Element(ns + "quantityUnit"),
                        GalenicFormId = int.Parse(entry.Element(ns + "galenicForm")?.Attribute("vidalId")?.Value ?? "0"),
                        GalenicForm = (string)entry.Element(ns + "galenicForm"),
                        RoutesLink = entry.Elements(atom + "link")
                                          .FirstOrDefault(l => (string)l.Attribute("title") == "ROUTES")?.Attribute("href")?.Value,
                        UnitsLink = entry.Elements(atom + "link")
                                         .FirstOrDefault(l => (string)l.Attribute("title") == "UNITS")?.Attribute("href")?.Value,
                        MoleculesLink = entry.Elements(atom + "link")
                                             .FirstOrDefault(l => (string)l.Attribute("title") == "MOLECULES")?.Attribute("href")?.Value,
                        PackagesLink = entry.Elements(atom + "link")
                                            .FirstOrDefault(l => (string)l.Attribute("title") == "PACKAGES")?.Attribute("href")?.Value,
                        ProductsLink = entry.Elements(atom + "link")
                                            .FirstOrDefault(l => (string)l.Attribute("title") == "PRODUCTS")?.Attribute("href")?.Value,
                        PrescribablesLink = entry.Elements(atom + "link")
                                                 .FirstOrDefault(l => (string)l.Attribute("title") == "PRESCRIBABLES")?.Attribute("href")?.Value,
                        VmpLink = entry.Elements(atom + "link")
                                       .FirstOrDefault(l => (string)l.Attribute("title") == "VMP")?.Attribute("href")?.Value
                    };

                    ucdvList.Add(ucdv);
                }

                return ucdvList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<UCDVModel>();
            }
        }

        public static List<UCDModel> ParseUCDXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var ucdList = new List<UCDModel>();

                var ucdEntries = document.Descendants(atom + "entry");

                foreach (var entry in ucdEntries)
                {
                    var ucd = new UCDModel
                    {
                        IdUCD = int.Parse(entry.Element(ns + "id")?.Value ?? "0"),
                        Summary = (string)entry.Element(atom + "summary"),
                        Name = (string)entry.Element(ns + "name"),
                        MarketStatus = (string)entry.Element(ns + "marketStatus")?.Attribute("name"),
                        SafetyAlert = bool.Parse(entry.Element(ns + "safetyAlert")?.Value ?? "false"),
                        IdVmp = int.Parse(entry.Element(ns + "vmp")?.Attribute("vidalId")?.Value ?? "0"),
                        VmpDescription = (string)entry.Element(ns + "vmp"),
                        UnitsLink = entry.Elements(atom + "link")
                                         .FirstOrDefault(l => (string)l.Attribute("title") == "UNITS")?.Attribute("href")?.Value,
                        RoutesLink = entry.Elements(atom + "link")
                                          .FirstOrDefault(l => (string)l.Attribute("title") == "ROUTES")?.Attribute("href")?.Value,
                        IndicatorsLink = entry.Elements(atom + "link")
                                              .FirstOrDefault(l => (string)l.Attribute("title") == "INDICATORS")?.Attribute("href")?.Value,
                        PackagesLink = entry.Elements(atom + "link")
                                            .FirstOrDefault(l => (string)l.Attribute("title") == "PACKAGES")?.Attribute("href")?.Value,
                        ProductsLink = entry.Elements(atom + "link")
                                            .FirstOrDefault(l => (string)l.Attribute("title") == "PRODUCTS")?.Attribute("href")?.Value,
                        SideEffectsLink = entry.Elements(atom + "link")
                                               .FirstOrDefault(l => (string)l.Attribute("title") == "SIDE_EFFECTS")?.Attribute("href")?.Value,
                        PrescribablesLink = entry.Elements(atom + "link")
                                                 .FirstOrDefault(l => (string)l.Attribute("title") == "PRESCRIBABLES")?.Attribute("href")?.Value,
                        AtcClassificationLink = entry.Elements(atom + "link")
                                                      .FirstOrDefault(l => (string)l.Attribute("title") == "ATC_CLASSIFICATION")?.Attribute("href")?.Value,
                        ProductLink = entry.Elements(atom + "link")
                                           .FirstOrDefault(l => (string)l.Attribute("title") == "PRODUCT")?.Attribute("href")?.Value,
                        MoleculesLink = entry.Elements(atom + "link")
                                             .FirstOrDefault(l => (string)l.Attribute("title") == "MOLECULES")?.Attribute("href")?.Value,
                        ActiveExcipientsLink = entry.Elements(atom + "link")
                                                     .FirstOrDefault(l => (string)l.Attribute("title") == "ACTIVE_EXCIPIENTS")?.Attribute("href")?.Value,
                        VmpLink = entry.Elements(atom + "link")
                                       .FirstOrDefault(l => (string)l.Attribute("title") == "VMP")?.Attribute("href")?.Value
                    };

                    ucdList.Add(ucd);
                }

                return ucdList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<UCDModel>();
            }
        }





        public static List<IdBaseDestinoModel> ParseVidalIdsToModelList(this string xmlContent, int idDestino)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var idBaseList = new List<IdBaseDestinoModel>();
                var document = XDocument.Parse(xmlContent);
                var entries = document.Descendants(atom + "entry");

                foreach (var entry in entries)
                {
                    // Extraer el valor de <id> dentro del espacio de nombres atom
                    var idElement = entry.Element(atom + "id");
                    if (idElement != null)
                    {
                        var idStr = idElement.Value;

                        // Obtener el valor después de la última diagonal
                        var idBaseStr = idStr.Split('/').Last();

                        // Intentar convertir el valor extraído a entero
                        if (int.TryParse(idBaseStr, out int idBase))
                        {
                            var model = new IdBaseDestinoModel
                            {
                                IdBase = idBase,
                                IdDestino = idDestino
                            };

                            idBaseList.Add(model);
                        }
                    }
                }

                return idBaseList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<IdBaseDestinoModel>();
            }
        }

       

        public static List<SideEffectModel> ParseSideEffectsXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var sideEffectList = new List<SideEffectModel>();

                var entries = document.Descendants(atom + "entry");

                foreach (var entry in entries)
                {
                    var sideEffect = new SideEffectModel
                    {
                        IdSideEffect = (int)entry.Element(ns + "id"),
                        Name = (string)entry.Element(ns + "name"),
                        ApparatusName = (string)entry.Element(ns + "apparatus"),
                        ApparatusId = (int?)entry.Element(ns + "apparatus")?.Attribute("vidalId"),
                        Updated = (DateTime)entry.Element(atom + "updated")
                    };

                    sideEffectList.Add(sideEffect);
                }

                return sideEffectList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<SideEffectModel>();
            }
        }






    }
}
