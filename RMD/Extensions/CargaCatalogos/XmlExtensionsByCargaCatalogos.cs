using RMD.Shared.Models.CargaCatalogos;
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
using System.Xml.Linq;

namespace RMD.Extensions.CargaCatalogos
{
    public static class XmlExtensionsByCargaCatalogos
    {
        public static List<VMPApiModel> ParseVMPXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var vmpModelList = new List<VMPApiModel>();

                var vmpEntries = document.Descendants(atom + "entry");

                foreach (var entry in vmpEntries)
                {
                    var vmp = new VMPApiModel
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
                return new List<VMPApiModel>();
            }
        }

        public static List<ProductApiModel> ParseProductsXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);

                var productEntries = document.Descendants(atom + "entry").Select(entry => new ProductApiModel
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
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "ALLERGIES")?.Attribute("href")?.Value,
                    NATIONAL_VMPS = entry.Elements(atom + "link")
                                    .FirstOrDefault(l => (string)l.Attribute("title") == "NATIONAL_VMPS")?.Attribute("href")?.Value
                }).ToList();

                return productEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<ProductApiModel>();
            }
        }

        public static List<PackageApiModel> ParsePackagesXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);

                var packageEntries = document.Descendants(atom + "entry").Select(entry => new PackageApiModel
                {
                    IdPackage = int.Parse(((string)entry.Element(ns + "id")).Split('/').Last()),
                    Name = (string)entry.Element(ns + "name") ?? string.Empty,
                    Summary = (string)entry.Element(atom + "summary") ?? string.Empty,
                    ProductId = int.Parse(((string)entry.Element(ns + "productId")) ?? "0"),
                    MarketStatus = (string)entry.Element(ns + "marketStatus")?.Attribute("name") ?? string.Empty,
                    Otc = (bool?)entry.Element(ns + "otc") ?? false,
                    IsCeps = (bool?)entry.Element(ns + "isCeps") ?? false,
                    DrugId = int.Parse(((string)entry.Element(ns + "drugId")) ?? "0"),
                    Cip13 = (string)entry.Element(ns + "cip13") ?? string.Empty,
                    ShortLabel = (string)entry.Element(ns + "shortLabel") ?? string.Empty,
                    Tfr = (bool?)entry.Element(ns + "tfr") ?? false,
                    IdCompany = int.Parse(((string)entry.Element(ns + "company")?.Attribute("vidalId")) ?? "0"),
                    CompanyName = (string)entry.Element(ns + "company") ?? string.Empty,
                    NarcoticPrescription = (bool?)entry.Element(ns + "narcoticPrescription") ?? false,
                    SafetyAlert = (bool?)entry.Element(ns + "safetyAlert") ?? false,
                    WithoutPrescription = (bool?)entry.Element(ns + "withoutPrescription") ?? false,
                    IdGalenicForm = int.Parse(((string)entry.Element(ns + "galenicForm")?.Attribute("vidalId")) ?? "0"),
                    GalenicForm = (string)entry.Element(ns + "galenicForm") ?? string.Empty,
                    UcdCode13 = (string)entry.Element(ns + "ucd")?.Attribute("code13") ?? string.Empty,
                    UcdCode7 = (string)entry.Element(ns + "ucd")?.Attribute("code7") ?? string.Empty,
                    UcdId = int.Parse(((string)entry.Element(ns + "ucd")?.Attribute("vidalId")) ?? "0"),
                    VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated")),

                    LargerPacks = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "LARGER_PACKS")?.Attribute("href")?.Value ?? string.Empty,
                    AffiliationCenter = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "AFFILIATION_CENTER")?.Attribute("href")?.Value ?? string.Empty,
                    Pds = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "PDS")?.Attribute("href")?.Value ?? string.Empty,
                    PricingSchedule = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "PRICING_SCHEDULE")?.Attribute("href")?.Value ?? string.Empty,
                    Units = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "UNITS")?.Attribute("href")?.Value ?? string.Empty,
                    Routes = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "ROUTES")?.Attribute("href")?.Value ?? string.Empty,
                    Indicators = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "INDICATORS")?.Attribute("href")?.Value ?? string.Empty,
                    Indications = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "INDICATIONS")?.Attribute("href")?.Value ?? string.Empty,
                    SideEffects = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "SIDE_EFFECTS")?.Attribute("href")?.Value ?? string.Empty,
                    Alds = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "ALDS")?.Attribute("href")?.Value ?? string.Empty,
                    VatExcAffiliationCenter = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "VAT_EXC_AFFILIATION_CENTER")?.Attribute("href")?.Value ?? string.Empty,
                    RefundIndications = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "REFUND_INDICATIONS")?.Attribute("href")?.Value ?? string.Empty,
                    OptDocument = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "OPT_DOCUMENT")?.Attribute("href")?.Value ?? string.Empty,
                    Document = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "DOCUMENT")?.Attribute("href")?.Value ?? string.Empty,
                    Ucd = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "UCD")?.Attribute("href")?.Value ?? string.Empty,
                    NationalVmpps = entry.Elements(atom + "link").FirstOrDefault(l => (string)l.Attribute("title") == "NATIONAL_VMPPS")?.Attribute("href")?.Value ?? string.Empty

                }).ToList();

                return packageEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<PackageApiModel>();
            }
        }

        public static List<UnitApiModel> ParseUnitsXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var unitList = new List<UnitApiModel>();

                var unitEntries = document.Descendants(atom + "entry");

                foreach (var entry in unitEntries)
                {
                    var unit = new UnitApiModel
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
                return new List<UnitApiModel>();
            }
        }

        public static List<AllergyApiModel> ParseAllergiesXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var allergyList = new List<AllergyApiModel>();

                var allergyEntries = document.Descendants(atom + "entry");

                foreach (var entry in allergyEntries)
                {
                    var allergy = new AllergyApiModel
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
                return new List<AllergyApiModel>();
            }
        }

        public static List<MoleculeApiModel> ParseMoleculesXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var moleculeList = new List<MoleculeApiModel>();

                var moleculeEntries = document.Descendants(atom + "entry");

                foreach (var entry in moleculeEntries)
                {
                    var molecule = new MoleculeApiModel
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
                return new List<MoleculeApiModel>();
            }
        }

        public static List<RouteApiModel> ParseRoutesXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var routeList = new List<RouteApiModel>();

                var routeEntries = document.Descendants(atom + "entry");

                foreach (var entry in routeEntries)
                {
                    var route = new RouteApiModel
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
                return new List<RouteApiModel>();
            }
        }

        public static List<CIM10ApiModel> ParseCIM10XmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var cim10List = new List<CIM10ApiModel>();

                var cim10Entries = document.Descendants(atom + "entry");

                foreach (var entry in cim10Entries)
                {
                    var cim10 = new CIM10ApiModel
                    {
                        IdCIM10 = (int)entry.Element(ns + "id"), // ID de CIM10
                        //Name = (string)entry.Element(ns + "name"), // Nombre de CIM10
                        Code = (string)entry.Element(ns + "code"), // Código de CIM10
                        //Summary = (string)entry.Element(atom + "summary"), // Resumen de CIM10
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
                return new List<CIM10ApiModel>();
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
        public static List<UCDVApiModel> ParseUCDVXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var ucdvList = new List<UCDVApiModel>();

                var ucdvEntries = document.Descendants(atom + "entry");

                foreach (var entry in ucdvEntries)
                {
                    var ucdv = new UCDVApiModel
                    {
                        IdUCDV = int.Parse(entry.Element(ns + "id")?.Value ?? "0"),
                        Summary = (string)entry.Element(atom + "summary") ?? "",
                        Name = (string)entry.Element(ns + "name") ?? "",
                        IdConditioningUnit = int.Parse(entry.Element(ns + "conditioningUnit")?.Attribute("vidalId")?.Value ?? "0"),
                        ConditioningUnit = (string)entry.Element(ns + "conditioningUnit") ?? "",
                        Quantity = decimal.Parse(entry.Element(ns + "quantity")?.Value ?? "0"),
                        QuantityUnitId = int.Parse(entry.Element(ns + "quantityUnit")?.Attribute("vidalId")?.Value ?? "0"),
                        QuantityUnit = (string)entry.Element(ns + "quantityUnit") ?? "",
                        GalenicFormId = int.Parse(entry.Element(ns + "galenicForm")?.Attribute("vidalId")?.Value ?? "0"),
                        GalenicForm = (string)entry.Element(ns + "galenicForm") ?? "",
                        RoutesLink = entry.Elements(atom + "link")
                                          .FirstOrDefault(l => (string)l.Attribute("title") == "ROUTES")?.Attribute("href")?.Value ?? "",
                        UnitsLink = entry.Elements(atom + "link")
                                         .FirstOrDefault(l => (string)l.Attribute("title") == "UNITS")?.Attribute("href")?.Value ?? "",
                        MoleculesLink = entry.Elements(atom + "link")
                                             .FirstOrDefault(l => (string)l.Attribute("title") == "MOLECULES")?.Attribute("href")?.Value ?? "",
                        PackagesLink = entry.Elements(atom + "link")
                                            .FirstOrDefault(l => (string)l.Attribute("title") == "PACKAGES")?.Attribute("href")?.Value ?? "",
                        ProductsLink = entry.Elements(atom + "link")
                                            .FirstOrDefault(l => (string)l.Attribute("title") == "PRODUCTS")?.Attribute("href")?.Value ?? "",
                        PrescribablesLink = entry.Elements(atom + "link")
                                                 .FirstOrDefault(l => (string)l.Attribute("title") == "PRESCRIBABLES")?.Attribute("href")?.Value ?? "",
                        VmpLink = entry.Elements(atom + "link")
                                       .FirstOrDefault(l => (string)l.Attribute("title") == "VMP")?.Attribute("href")?.Value ?? "",
                        NationalVmppsLink = entry.Elements(atom + "link")
                                                 .FirstOrDefault(l => (string)l.Attribute("title") == "NATIONAL_VMPPS")?.Attribute("href")?.Value ?? "",
                        NationalVmpsLink = entry.Elements(atom + "link")
                                                .FirstOrDefault(l => (string)l.Attribute("title") == "NATIONAL_VMPS")?.Attribute("href")?.Value ?? ""
                    };

                    ucdvList.Add(ucdv);
                }

                return ucdvList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<UCDVApiModel>();
            }
        }

        public static List<UCDApiModel> ParseUCDXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var ucdList = new List<UCDApiModel>();

                var ucdEntries = document.Descendants(atom + "entry");

                foreach (var entry in ucdEntries)
                {
                    var ucd = new UCDApiModel
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
                return new List<UCDApiModel>();
            }
        }





        //public static List<IdBaseDestinoModel> ParseVidalIdsToModelList(this string xmlContent, int idDestino)
        //{
        //    try
        //    {
        //        XNamespace atom = "http://www.w3.org/2005/Atom";
        //        XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

        //        var idBaseList = new List<IdBaseDestinoModel>();
        //        var document = XDocument.Parse(xmlContent);
        //        var entries = document.Descendants(atom + "entry");

        //        foreach (var entry in entries)
        //        {
        //            // Extraer el valor de <id> dentro del espacio de nombres atom
        //            var idElement = entry.Element(atom + "id");
        //            if (idElement != null)
        //            {
        //                var idStr = idElement.Value;

        //                // Obtener el valor después de la última diagonal
        //                var idBaseStr = idStr.Split('/').Last();

        //                // Intentar convertir el valor extraído a entero
        //                if (int.TryParse(idBaseStr, out int idBase))
        //                {
        //                    var model = new IdBaseDestinoModel
        //                    {
        //                        IdBase = idBase,
        //                        IdDestino = idDestino
        //                    };

        //                    idBaseList.Add(model);
        //                }
        //            }
        //        }

        //        return idBaseList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al analizar el XML: {ex.Message}");
        //        return new List<IdBaseDestinoModel>();
        //    }
        //}



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
