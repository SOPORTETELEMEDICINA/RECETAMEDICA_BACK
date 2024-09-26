using RMD.Models.Vidal.ByUCD;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByUCD
{
    public static class XmlExtensionsByUCD
    {
        public static UCDByIdPackage ParseUCDXml(this string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return null; // Retorna nulo si el XML es vacío

            XDocument document = XDocument.Parse(xmlContent);
            XNamespace atomNamespace = "http://www.w3.org/2005/Atom";
            XNamespace vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entry = document.Descendants(atomNamespace + "entry").FirstOrDefault();
            if (entry == null) return null;

            var ucd = new UCDByIdPackage
            {
                Title = entry.Element(atomNamespace + "title")?.Value,
                Id = entry.Element(atomNamespace + "id")?.Value,
                Summary = entry.Element(atomNamespace + "summary")?.Value,
                VidalId = int.Parse(entry.Element(vidalNamespace + "id")?.Value ?? "0"),
                MarketStatus = entry.Element(vidalNamespace + "marketStatus")?.Value,
                SafetyAlert = bool.Parse(entry.Element(vidalNamespace + "safetyAlert")?.Value ?? "false"),
                RelatedLinks = entry.Elements(atomNamespace + "link")
                    .Select(link => link.Attribute("href")?.Value)
                    .ToList()
            };

            return ucd;
        }

        public static UCDById ParseUcdXml(this string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return null; // Retorna nulo si el XML es vacío

            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);
            var entry = document.Descendants(atom + "entry").FirstOrDefault();

            return entry == null
                ? null
                : new UCDById
                {
                    Id = (string)entry.Element(atom + "id") ?? string.Empty,
                    Updated = DateTime.Parse((string)entry.Element(atom + "updated") ?? DateTime.MinValue.ToString()),
                    Title = (string)entry.Element(atom + "title") ?? string.Empty,
                    VidalId = int.TryParse(entry.Element(vidal + "id")?.Value, out var vidalId) ? vidalId : 0,
                    Name = (string)entry.Element(vidal + "name") ?? string.Empty,
                    MarketStatus = (string)entry.Element(vidal + "marketStatus")?.Attribute("name") ?? string.Empty,
                    SafetyAlert = bool.TryParse(entry.Element(vidal + "safetyAlert")?.Value, out var safetyAlert) && safetyAlert
                };
        }

        public static List<UcdPackage> ParseUcdPackagesXml(this string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return new List<UcdPackage>(); // Lista vacía si el XML es vacío

            var document = XDocument.Parse(xmlContent);
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var packages = document.Descendants(atom + "entry")
                .Select(entry => new UcdPackage
                {
                    Id = (string)entry.Element(atom + "id") ?? string.Empty,
                    Updated = DateTime.Parse((string)entry.Element(atom + "updated") ?? DateTime.MinValue.ToString()),
                    Title = (string)entry.Element(atom + "title") ?? string.Empty,
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Name = (string)entry.Element(vidal + "name") ?? string.Empty,
                    ItemType = (string)entry.Element(vidal + "itemType")?.Attribute("name") ?? string.Empty,
                    ProductId = int.Parse(entry.Element(vidal + "productId")?.Value ?? "0"),
                    MarketStatus = (string)entry.Element(vidal + "marketStatus")?.Attribute("name") ?? string.Empty,
                    Otc = (bool?)entry.Element(vidal + "otc") ?? false,
                    IsCeps = (bool?)entry.Element(vidal + "isCeps") ?? false,
                    DrugId = int.Parse(entry.Element(vidal + "drugId")?.Value ?? "0"),
                    Cip13 = (string)entry.Element(vidal + "cip13") ?? string.Empty,
                    ShortLabel = (string)entry.Element(vidal + "shortLabel") ?? string.Empty,
                    Tfr = (bool?)entry.Element(vidal + "tfr") ?? false,
                    Company = (string)entry.Element(vidal + "company") ?? string.Empty,
                    NarcoticPrescription = (bool?)entry.Element(vidal + "narcoticPrescription") ?? false,
                    SafetyAlert = (bool?)entry.Element(vidal + "safetyAlert") ?? false,
                    WithoutPrescription = (bool?)entry.Element(vidal + "withoutPrescription") ?? false,
                    Product = (string)entry.Element(vidal + "product") ?? string.Empty,
                    Ucd = (string)entry.Element(vidal + "ucd") ?? string.Empty
                }).ToList();

            return packages;
        }

        public static List<UcdProduct> ParseUcdProductsXml(this string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return new List<UcdProduct>(); // Lista vacía si el XML es vacío

            var document = XDocument.Parse(xmlContent);
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var products = document.Descendants(atom + "entry")
                .Select(entry => new UcdProduct
                {
                    Id = (string)entry.Element(atom + "id"),
                    Updated = DateTime.Parse((string)entry.Element(atom + "updated")),
                    Title = (string)entry.Element(atom + "title"),
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Name = (string)entry.Element(vidal + "name"),
                    ItemType = (string)entry.Element(vidal + "itemType")?.Attribute("name")?.Value ?? string.Empty,
                    MarketStatus = (string)entry.Element(vidal + "marketStatus")?.Attribute("name")?.Value ?? string.Empty,
                    HasPublishedDoc = bool.TryParse(entry.Element(vidal + "hasPublishedDoc")?.Value, out bool hasPublishedDoc) && hasPublishedDoc,
                    WithoutPrescription = bool.TryParse(entry.Element(vidal + "withoutPrescription")?.Value, out bool withoutPrescription) && withoutPrescription,
                    AmmTypeId = int.TryParse(entry.Element(vidal + "ammType")?.Attribute("vidalId")?.Value, out int ammTypeId) ? ammTypeId : 0,
                    BestDocType = (string)entry.Element(vidal + "bestDocType")?.Attribute("name")?.Value ?? string.Empty,
                    SafetyAlert = bool.TryParse(entry.Element(vidal + "safetyAlert")?.Value, out bool safetyAlert) && safetyAlert,
                    ActivePrinciples = (string)entry.Element(vidal + "activePrinciples")?.Value ?? string.Empty,
                    Company = (string)entry.Element(vidal + "company")?.Value ?? string.Empty,
                    GalenicForm = (string)entry.Element(vidal + "galenicForm")?.Value ?? string.Empty
                }).ToList();

            return products;
        }


        public static List<UCDSideEffect> ParseUcdSideEffectsXml(this string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return new List<UCDSideEffect>(); // Lista vacía si el XML es vacío

            var document = XDocument.Parse(xmlContent);
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var sideEffects = document.Descendants(atom + "entry")
                .Select(entry => new UCDSideEffect
                {
                    Title = (string)entry.Element(atom + "title"),
                    Id = (string)entry.Element(vidal + "id"),
                    Name = (string)entry.Element(vidal + "name"),
                    Apparatus = (string)entry.Element(vidal + "apparatus"),
                    Frequency = (string)entry.Element(vidal + "frequency")?.Attribute("name")?.Value,
                    Order = int.Parse((string)entry.Element(vidal + "order") ?? "0")
                }).ToList();

            return sideEffects;
        }


        public static List<Ucd> ParseUcdsXml(this string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return new List<Ucd>(); // Lista vacía si el XML es vacío

            var document = XDocument.Parse(xmlContent);
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var ucdEntries = document.Descendants(atom + "entry")
                .Select(entry => new Ucd
                {
                    Title = (string)entry.Element(atom + "title"),
                    Id = (string)entry.Element(atom + "id"),
                    Updated = DateTime.Parse((string)entry.Element(atom + "updated")),
                    Summary = (string)entry.Element(atom + "summary"),
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Name = (string)entry.Element(vidal + "name"),
                    MarketStatus = (string)entry.Element(vidal + "marketStatus")?.Attribute("name")?.Value,
                    SafetyAlert = bool.Parse(entry.Element(vidal + "safetyAlert")?.Value ?? "false"),
                    VmpName = (string)entry.Element(vidal + "vmp"),
                    VmpVidalId = int.Parse(entry.Element(vidal + "vmp")?.Attribute("vidalId")?.Value ?? "0")
                }).ToList();

            return ucdEntries;
        }

    }
}
