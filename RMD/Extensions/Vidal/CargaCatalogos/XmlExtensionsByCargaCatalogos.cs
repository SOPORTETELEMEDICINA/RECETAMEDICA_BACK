using RMD.Models.Vidal.CargaCatalogos;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.CargaCatalogos
{
    public static class XmlExtensionsByCargaCatalogos
    {
        public static List<VMPModelLink> ParseVMPXmlToModelList(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);
                var vmpModelLinkList = new List<VMPModelLink>();

                var vmpEntries = document.Descendants(atom + "entry");

                foreach (var entry in vmpEntries)
                {
                    var vmp = new VMPModel
                    {
                        IdVMP = (int)entry.Element(ns + "id"),
                        Name = (string)entry.Element(ns + "name"),
                        ActivePrinciples = (string)entry.Element(ns + "activePrinciples"),
                        IdRoute = (int)entry.Element(ns + "route").Attribute("id"),
                        RouteName = (string)entry.Element(ns + "route"),
                        GalenicFormVidalId = (int)entry.Element(ns + "galenicForm").Attribute("vidalId"),
                        GalenicForm = (string)entry.Element(ns + "galenicForm"),
                        RegulatoryGenericPrescription = (bool)entry.Element(ns + "regulatoryGenericPrescription"),
                        IdVTM = entry.Elements(atom + "link")
                                     .Where(l => (string)l.Attribute("title") == "VTM")
                                     .Select(l => int.Parse(((string)l.Attribute("href")).Split('/').Last()))
                                     .FirstOrDefault(),
                        UpdatedDate = (DateTime)entry.Element(atom + "updated")
                    };

                    // Crear la lista de links
                    var links = entry.Elements(atom + "link")
                                     .Select(l => new LinkModel
                                     {
                                         Href = (string)l.Attribute("href"),
                                         Title = (string)l.Attribute("title")
                                     })
                                     .Where(link => link.Title != "DOCUMENTS" && link.Title != "OPT_DOCUMENT" // Excluir los "DOCUMENTS" y "OPT_DOCUMENT"
                                                 && (link.Title != "VTM" || vmp.IdVTM == 0) // Si ya tenemos el VTM en el modelo, no agregarlo a los links
                                                 && (link.Title != "ROUTE" || vmp.IdRoute == 0)
                                                 && (link.Title != "VMP" || vmp.IdRoute == 0)) // Si ya tenemos la Route en el modelo, no agregarlo a los links
                                     .ToList();

                    // Crear el VMPModelLink que encapsula el VMPModel y los links
                    var vmpModelLink = new VMPModelLink
                    {
                        Vmp = vmp,
                        Links = links
                    };

                    vmpModelLinkList.Add(vmpModelLink);
                }

                return vmpModelLinkList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<VMPModelLink>();
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
                    IdProduct = int.Parse(((string)entry.Element(ns + "id")).Split('/').Last()), // Parsear el vidal:id correctamente
                    Summary = (string)entry.Element(atom + "summary"), // Resumen
                    Name = (string)entry.Element(ns + "name"), // Nombre del producto
                    IdItemType = (string)entry.Element(ns + "itemType")?.Attribute("name"), // Tipo de ítem
                    IdMarketStatus = (string)entry.Element(ns + "marketStatus")?.Attribute("name"), // Estado de mercado
                    HasPublishedDoc = (bool?)entry.Element(ns + "hasPublishedDoc") ?? false, // Si tiene documentos publicados
                    WithoutPrescription = (bool?)entry.Element(ns + "withoutPrescription") ?? false, // Sin receta
                    IdAmmType = (int?)entry.Element(ns + "ammType")?.Attribute("vidalId") ?? 0, // Tipo de autorización
                    BestDocType = (string)entry.Element(ns + "bestDocType")?.Attribute("name"), // Mejor tipo de documento
                    SafetyAlert = (bool?)entry.Element(ns + "safetyAlert") ?? false, // Alerta de seguridad
                    IdCompany = (int?)entry.Element(ns + "company")?.Attribute("vidalId") ?? 0, // ID de la compañía
                    CompanyName = (string)entry.Element(ns + "company"), // Nombre de la compañía
                    TypeCompany = (string)entry.Element(ns + "company")?.Attribute("type"), // Tipo de compañía
                    IdVmp = int.Parse(((string)entry.Element(ns + "vmp")?.Attribute("vidalId")) ?? "0"), // ID del VMP
                    IdGalenicForm = int.Parse(((string)entry.Element(ns + "galenicForm")?.Attribute("vidalId")) ?? "0"), // ID de la forma galénica
                    GalenicForm = (string)entry.Element(ns + "galenicForm"), // Descripción de la forma galénica
                    VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated")) // Fecha de actualización de Vidal
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
                    VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated")) // Fecha de actualización
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
                        VidalUpdateDate = DateTime.Parse((string)entry.Element(atom + "updated"))
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
                        IdCIM10 = (int)entry.Element(ns + "id"),
                        Name = (string)entry.Element(ns + "name"),
                        Code = (string)entry.Element(ns + "code"),
                        Summary = (string)entry.Element(atom + "summary"),
                        UpdatedDate = (DateTime)entry.Element(atom + "updated")
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

    }
}
