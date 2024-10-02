using RMD.Models.Vidal.ByVMP;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByVMP
{
    public static class XmlExtensionsByVMP
    {
       

        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        public static List<VMPEntry> ParseVMPXml(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);

                var vmpEntries = document.Descendants(atom + "entry").Select(entry => new VMPEntry
                {
                    IdVmp = (int)entry.Element(ns + "id"),
                    Summary = (string)entry.Element(atom + "summary"),
                    Name = (string)entry.Element(ns + "name"),
                    ActivePrinciples = (string)entry.Element(ns + "activePrinciples"),
                    RouteId = (int)entry.Element(ns + "route").Attribute("id"),
                    RouteValue = (string)entry.Element(ns + "route"),
                    GalenicFormId = (int)entry.Element(ns + "galenicForm").Attribute("vidalId"),
                    GalenicFormValue = (string)entry.Element(ns + "galenicForm"),
                    RegulatoryGenericPrescription = (bool)entry.Element(ns + "regulatoryGenericPrescription"),
                    IdVTM = entry.Elements(atom + "link")
                                 .Where(l => (string)l.Attribute("title") == "VTM")
                                 .Select(l => int.Parse(((string)l.Attribute("href")).Split('/').Last()))
                                 .FirstOrDefault()
                }).ToList();

                return vmpEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return [];
            }
        }

        public static VMPEntry ParseVMPByIdXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var entryElement = document.Root.Element("{http://www.w3.org/2005/Atom}entry");

            if (entryElement == null)
            {
                return null;
            }

            var vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            var vmpEntry = new VMPEntry
            {
                IdVmp = (int)entryElement.Element(XName.Get("id", vidalNamespace)),
                Name = (string)entryElement.Element(XName.Get("name", vidalNamespace)),
                ActivePrinciples = (string)entryElement.Element(XName.Get("activePrinciples", vidalNamespace)),
                RouteId = (int)entryElement.Element(XName.Get("route", vidalNamespace)).Attribute("id"),
                RouteValue = (string)entryElement.Element(XName.Get("route", vidalNamespace)),
                GalenicFormId = (int)entryElement.Element(XName.Get("galenicForm", vidalNamespace)).Attribute("vidalId"),
                GalenicFormValue = (string)entryElement.Element(XName.Get("galenicForm", vidalNamespace)),
                RegulatoryGenericPrescription = bool.Parse((string)entryElement.Element(XName.Get("regulatoryGenericPrescription", vidalNamespace)))
            };

            // Buscar en los enlaces para encontrar IdVTM
            foreach (var link in entryElement.Elements("{http://www.w3.org/2005/Atom}link"))
            {
                var href = (string)link.Attribute("href");
                if (href.Contains("/vtm/"))
                {
                    var idStr = href.Split('/').Last();
                    vmpEntry.IdVTM = int.Parse(idStr);
                    break;
                }
            }

            return vmpEntry;
        }

        public static List<VMPProductEntry> ParseVMPProductXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace openSearchNs = "http://a9.com/-/spec/opensearch/1.1/";

            var entries = document.Root.Elements(ns + "entry")
                .Select(entry => new VMPProductEntry
                {
                    IdProduct = (int?)entry.Element(vidalNs + "id") ?? 0, // Manejo de nulos
                    Name = (string)entry.Element(vidalNs + "name") ?? string.Empty,
                    ActivePrinciples = (string)entry.Element(vidalNs + "activePrinciples") ?? string.Empty,
                    MarketStatus = (string)entry.Element(vidalNs + "marketStatus")?.Attribute("name")?.Value ?? string.Empty,
                    HasPublishedDoc = bool.TryParse((string)entry.Element(vidalNs + "hasPublishedDoc"), out var hasPublishedDoc) && hasPublishedDoc,
                    WithoutPrescription = bool.TryParse((string)entry.Element(vidalNs + "withoutPrescription"), out var withoutPrescription) && withoutPrescription,
                    SafetyAlert = bool.TryParse((string)entry.Element(vidalNs + "safetyAlert"), out var safetyAlert) && safetyAlert,
                    IdVMP = (int?)entry.Element(vidalNs + "vmp")?.Attribute("vidalId") ?? 0,
                    IdCompany = (int?)entry.Element(vidalNs + "company")?.Attribute("vidalId") ?? 0,
                    CompanyName = (string)entry.Element(vidalNs + "company") ?? string.Empty,
                    IdGalenicForm = (int?)entry.Element(vidalNs + "galenicForm")?.Attribute("vidalId") ?? 0,
                    GalenicForm = (string)entry.Element(vidalNs + "galenicForm") ?? string.Empty
                })
                .ToList();

            // Extraer información de paginación usando el espacio de nombres opensearch (opcional)
            var itemsPerPage = (int?)document.Root.Element(openSearchNs + "itemsPerPage") ?? 0;
            var totalResults = (int?)document.Root.Element(openSearchNs + "totalResults") ?? 0;
            var startIndex = (int?)document.Root.Element(openSearchNs + "startIndex") ?? 0;

            return entries;
        }

        public static List<VMPAtcClassificationEntry> ParseVMPAtcClassificationXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .Select(entry =>
                {
                    var parentLink = entry.Elements(XName.Get("link", ns))
                        .FirstOrDefault(link => (string)link.Attribute("title") == "PARENT");

                    var idParent = parentLink != null
                        ? ExtractIdFromHref((string)parentLink.Attribute("href"), 2)
                        : (int?)null;

                    return new VMPAtcClassificationEntry
                    {
                        IdAtcClassification = (int)entry.Element(XName.Get("id", vidalNs)),
                        Name = (string)entry.Element(XName.Get("name", vidalNs)),
                        Code = (string)entry.Element(XName.Get("code", vidalNs)),
                        IdParent = idParent
                    };
                })
                .ToList();

            return entries;
        }
        public static List<VMPMoleculeEntry> ParseVMPMoleculeXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .Select(entry => new VMPMoleculeEntry
                {
                    IdMolecule = (int)entry.Element(XName.Get("id", vidalNs)),
                    Name = (string)entry.Element(XName.Get("name", vidalNs)),
                    ItemType = (string)entry.Element(XName.Get("itemType", vidalNs)).Attribute("name"),
                    PerVolumeValue = double.Parse((string)entry.Element(XName.Get("perVolumeValue", vidalNs))),
                    PerVolumeUnit = (string)entry.Element(XName.Get("perVolumeUnit", vidalNs))
                })
                .ToList();

            return entries;
        }

        public static List<VMPUnitEntry> ParseVMPUnitXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(ns + "entry")
                .Select(entry =>
                {
                    var parentLink = entry.Elements(ns + "link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "PARENT");

                    var idParent = parentLink != null
                        ? ExtractIdFromHref((string)parentLink.Attribute("href"), 1)
                        : (int?)null;

                    return new VMPUnitEntry
                    {
                        IdUnit = (int?)entry.Element(vidalNs + "unitId") ?? 0,  // Manejo de nulos
                        Name = (string)entry.Element(vidalNs + "name") ?? string.Empty,
                        SingularName = (string)entry.Element(vidalNs + "singularName") ?? string.Empty,
                        ParentConversionRate = (string)entry.Element(vidalNs + "parentConversionRate") ?? string.Empty,
                        IdParent = idParent,
                        DerivedByWeight = (string)entry.Element(vidalNs + "derivedByWeight")?.Attribute("name") ?? string.Empty,
                        DerivedBySize = (string)entry.Element(vidalNs + "derivedBySize")?.Attribute("name") ?? string.Empty,
                        Rank = (int?)entry.Element(vidalNs + "rank") ?? 0
                    };
                })
                .ToList();

            return entries;
        }

        public static List<VMPContraindicationEntry> ParseVMPContraindicationXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(ns + "entry")
                .Select(entry => new VMPContraindicationEntry
                {
                    IdContraindication = int.Parse(((string)entry.Element(vidalNs + "id"))?.Split('/').Last() ?? "0"),
                    Title = (string)entry.Element(ns + "title") ?? "Unknown title",
                    Relativity = (string)entry.Element(vidalNs + "relativity") ?? "Unknown relativity"
                })
                .ToList();

            return entries;
        }

        public static List<VMPPhysicoChemicalInteractionEntry> ParseVMPPhysicoChemicalInteractionXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .Select(entry => new VMPPhysicoChemicalInteractionEntry
                {
                    IdInteraction = (int)entry.Element(XName.Get("id", vidalNs)),
                    Title = (string)entry.Element(XName.Get("title")),
                    Name = (string)entry.Element(XName.Get("name", vidalNs))
                })
                .ToList();

            return entries;
        }

        public static List<VMPRouteEntry> ParseVMPRouteXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .Select(entry => new VMPRouteEntry
                {
                    IdRoute = (int)entry.Element(XName.Get("id", vidalNs)),
                    Name = (string)entry.Element(XName.Get("name", vidalNs)),
                    RouteId = (int)entry.Element(XName.Get("routeId", vidalNs)),
                    Ranking = (int)entry.Element(XName.Get("ranking", vidalNs)),
                    OutOfSPC = bool.Parse((string)entry.Element(XName.Get("outOfSPC", vidalNs))),
                    ParentId = (int)entry.Element(XName.Get("parentId", vidalNs))
                })
                .ToList();

            return entries;
        }

        public static List<VMPIndicatorEntry> ParseVMPIndicatorXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .SelectMany(entry => entry.Elements(XName.Get("indicator", vidalNs))
                .Select(indicator => new VMPIndicatorEntry
                {
                    IndicatorId = (int)indicator.Attribute("vidalId"),
                    Name = (string)indicator
                }))
                .ToList();

            return entries;
        }

        public static List<VMPIndicationEntry> ParseVMPIndicationXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .Select(entry => new VMPIndicationEntry
                {
                    Id = (int)entry.Element(XName.Get("id", vidalNs)),
                    Name = (string)entry.Element(XName.Get("name", vidalNs)),
                    Type = entry.Element(XName.Get("type", vidalNs))?.Attribute("name")?.Value ?? string.Empty,
                    Category = (string)entry.Attribute(XName.Get("categories", vidalNs))
                })
                .ToList();

            return entries;
        }

        public static List<VMPSideEffectEntry> ParseVMPSideEffectXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .Select(entry => new VMPSideEffectEntry
                {
                    IdSideEffect = (int)entry.Element(XName.Get("id", vidalNs)),
                    Name = (string)entry.Element(XName.Get("name", vidalNs)),
                    IdApparatus = (int)entry.Element(XName.Get("apparatus", vidalNs)).Attribute("vidalId"),
                    ApparatusName = (string)entry.Element(XName.Get("apparatus", vidalNs)),
                    Frequency = entry.Element(XName.Get("frequency", vidalNs))?.Attribute("name")?.Value ?? string.Empty
                })
                .ToList();

            return entries;
        }

        public static List<VMPUcdvEntry> ParseVMPUcdvXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(ns + "entry")
                .Select(entry => new VMPUcdvEntry
                {
                    IdUcdv = int.Parse(((string)entry.Element(vidalNs + "id"))?.Split('/').Last() ?? "0"),
                    Name = (string)entry.Element(vidalNs + "name") ?? string.Empty,
                    IdConditioningUnit = (int?)entry.Element(vidalNs + "conditioningUnit")?.Attribute("vidalId") ?? 0,
                    ConditioningUnitName = (string)entry.Element(vidalNs + "conditioningUnit") ?? string.Empty,
                    // Para Quantity y QuantityUnit, verificamos si están presentes, de lo contrario, asignamos valores por defecto
                    Quantity = (double?)entry.Element(vidalNs + "quantity") ?? 0,
                    IdQuantityUnit = (int?)entry.Element(vidalNs + "quantityUnit")?.Attribute("vidalId") ?? 0,
                    QuantityUnitName = (string)entry.Element(vidalNs + "quantityUnit") ?? string.Empty,
                    IdGalenicForm = (int?)entry.Element(vidalNs + "galenicForm")?.Attribute("vidalId") ?? 0,
                    GalenicFormName = (string)entry.Element(vidalNs + "galenicForm") ?? string.Empty
                })
                .ToList();

            return entries;
        }

        public static List<VMPUcdEntry> ParseVMPUcdXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .Select(entry => new VMPUcdEntry
                {
                    IdUcd = (int)entry.Element(XName.Get("id", vidalNs)),
                    Name = (string)entry.Element(XName.Get("name", vidalNs)),
                    MarketStatus = (string)entry.Element(XName.Get("marketStatus", vidalNs)).Attribute("name"),
                    SafetyAlert = (bool)entry.Element(XName.Get("safetyAlert", vidalNs))
                })
                .ToList();

            return entries;
        }

        public static List<VMPAllergyEntry> ParseVMPAllergyXml(this string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
            {
                return new List<VMPAllergyEntry>(); // Si el XML está vacío, devolvemos una lista vacía
            }

            var document = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(ns + "entry")
                .Select(entry => new VMPAllergyEntry
                {
                    IdAllergy = (int?)entry.Element(vidalNs + "id") ?? 0,
                    Name = (string)entry.Element(vidalNs + "name") ?? string.Empty
                })
                .ToList();

            return entries.Any() ? entries : new List<VMPAllergyEntry>(); // Si no hay elementos, retornamos lista vacía
        }

        public static List<VMPDocumentEntry> ParseVMPDocumentXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = document.Root.Elements(XName.Get("entry", ns))
                .Select(entry => new VMPDocumentEntry
                {
                    IdDocument = (int)entry.Element(XName.Get("id", vidalNs)),
                    Name = (string)entry.Element(XName.Get("name", vidalNs)),
                    Link = (string)entry.Elements(XName.Get("link", ns))
                                .FirstOrDefault(l => (string)l.Attribute("type") == "application/xhtml+xml" &&
                                                      (string)l.Attribute("title") == "VMPFR")
                                ?.Attribute("href")
                })
                .ToList();

            return entries;
        }

        private static int ExtractIdFromHref(string href, int part)
        {
            // Extrae el ID de la URL
            var parts = href.Split('/');
            if (parts.Length > 3 && int.TryParse(parts[^part], out int id))
            {
                return id;
            }
            return 0;
        }
    }
}
