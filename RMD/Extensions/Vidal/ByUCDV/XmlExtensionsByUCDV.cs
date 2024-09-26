using RMD.Models.Vidal.ByUCDV;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByUCDV
{
    public static class XmlExtensionsByUCDV
    {
        public static List<UCDVS> ParseUcdvsXml(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom"; // Namespace Atom

            var ucdvEntries = doc.Descendants(atom + "entry").Select(entry => new UCDVS
            {
                Title = entry.Element(atom + "title")?.Value,
                Id = entry.Element(atom + "id")?.Value,
                Updated = DateTime.Parse(entry.Element(atom + "updated")?.Value),
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Name = entry.Element(vidal + "name")?.Value,
                ConditioningUnit = entry.Element(vidal + "conditioningUnit")?.Value,
                Quantity = entry.Element(vidal + "quantity")?.Value,
                QuantityUnit = entry.Element(vidal + "quantityUnit")?.Value,
                GalenicForm = entry.Element(vidal + "galenicForm")?.Value
            }).ToList();

            return ucdvEntries;
        }

        public static UCDV ParseUcdvXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom"; // Namespace Atom

            var entry = document.Descendants(atom + "entry").FirstOrDefault();

            if (entry == null) return null;

            var ucdv = new UCDV
            {
                Id = (string)entry.Element(atom + "id"),
                Updated = DateTime.Parse((string)entry.Element(atom + "updated")),
                Title = (string)entry.Element(atom + "title"),
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Name = (string)entry.Element(vidal + "name"),
                GalenicForm = (string)entry.Element(vidal + "galenicForm"),
                GalenicFormVidalId = int.Parse(entry.Element(vidal + "galenicForm")?.Attribute("vidalId")?.Value ?? "0")
            };

            return ucdv;
        }

        public static List<UCDVRoute> ParseUcdvRoutesXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom"; // Namespace Atom

            var document = XDocument.Parse(xmlContent);

            var routes = document.Descendants(atom + "entry").Select(entry => new UCDVRoute
            {
                Title = (string)entry.Element(atom + "title"),
                Id = (string)entry.Element(atom + "id"),
                Name = (string)entry.Element(vidal + "name"),
                RouteId = int.Parse(entry.Element(vidal + "routeId").Value),
                Ranking = int.Parse(entry.Element(vidal + "ranking").Value),
                OutOfSPC = bool.Parse(entry.Element(vidal + "outOfSPC").Value),
                ParentId = (string)entry.Element(vidal + "parentId")
            }).ToList();

            return routes;
        }

        public static List<UCDVUnit> ParseUcdvUnitsXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom"; // Namespace Atom

            var document = XDocument.Parse(xmlContent);

            var units = document.Descendants(atom + "entry")
                .Where(entry => entry.Attribute(vidal + "categories").Value.Contains("UNIT"))
                .Select(entry => new UCDVUnit
                {
                    Title = (string)entry.Element(atom + "title"),
                    Id = (string)entry.Element(atom + "id"),
                    Name = (string)entry.Element(vidal + "name"),
                    SingularName = (string)entry.Element(vidal + "singularName"),
                    UnitId = int.Parse(entry.Element(vidal + "unitId").Value),
                    DerivedByWeight = (string)entry.Element(vidal + "derivedByWeight"),
                    DerivedBySize = (string)entry.Element(vidal + "derivedBySize"),
                    Rank = int.Parse(entry.Element(vidal + "rank").Value),
                    ParentConversionRate = new ParentConversionRate
                    {
                        Denominator = int.Parse(entry.Element(vidal + "parentConversionRate").Attribute("denominator").Value),
                        Numerator = decimal.Parse(entry.Element(vidal + "parentConversionRate").Attribute("numerator").Value), // Cambiado a decimal.Parse
                        UnitId = int.Parse(entry.Element(vidal + "parentConversionRate").Attribute("unitId").Value),
                        Description = (string)entry.Element(vidal + "parentConversionRate")
                    }
                }).ToList();

            return units;
        }


        public static List<UCDVPackage> ParseUcdvPackagesXml(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom"; // Namespace Atom

            var packageEntries = doc.Descendants(atom + "entry").Select(entry => new UCDVPackage
            {
                Title = entry.Element(atom + "title")?.Value,
                Id = entry.Element(atom + "id")?.Value,
                Updated = DateTime.Parse(entry.Element(atom + "updated")?.Value),
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Name = entry.Element(vidal + "name")?.Value,
                ItemType = entry.Element(vidal + "itemType")?.Attribute("name")?.Value,
                ProductId = int.Parse(entry.Element(vidal + "productId")?.Value ?? "0"),
                MarketStatus = entry.Element(vidal + "marketStatus")?.Attribute("name")?.Value,
                Otc = bool.Parse(entry.Element(vidal + "otc")?.Value ?? "false"),
                IsCeps = bool.Parse(entry.Element(vidal + "isCeps")?.Value ?? "false"),
                DrugId = int.Parse(entry.Element(vidal + "drugId")?.Value ?? "0"),
                Cip13 = entry.Element(vidal + "cip13")?.Value,
                ShortLabel = entry.Element(vidal + "shortLabel")?.Value,
                Tfr = bool.Parse(entry.Element(vidal + "tfr")?.Value ?? "false"),
                NarcoticPrescription = bool.Parse(entry.Element(vidal + "narcoticPrescription")?.Value ?? "false"),
                SafetyAlert = bool.Parse(entry.Element(vidal + "safetyAlert")?.Value ?? "false"),
                WithoutPrescription = bool.Parse(entry.Element(vidal + "withoutPrescription")?.Value ?? "false"),
                Product = entry.Element(vidal + "product")?.Value,
                GalenicForm = entry.Element(vidal + "galenicForm")?.Value,
                Vmp = entry.Element(vidal + "vmp")?.Value,
                Ucd = entry.Element(vidal + "ucd")?.Value,
            }).ToList();

            return packageEntries;
        }

        public static List<UCDVProduct> ParseUcdvProductsXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom"; // Namespace Atom

            var document = XDocument.Parse(xmlContent);

            var products = document.Descendants(atom + "entry")
                .Select(entry => new UCDVProduct
                {
                    Title = (string)entry.Element(atom + "title"),
                    Id = (string)entry.Element(atom + "id"),
                    Name = (string)entry.Element(vidal + "name"),
                    ItemType = (string)entry.Element(vidal + "itemType")?.Attribute("name")?.Value,
                    MarketStatus = (string)entry.Element(vidal + "marketStatus")?.Attribute("name")?.Value,
                    HasPublishedDoc = bool.Parse(entry.Element(vidal + "hasPublishedDoc")?.Value ?? "false"),
                    WithoutPrescription = bool.Parse(entry.Element(vidal + "withoutPrescription")?.Value ?? "false"),
                    AmmTypeId = int.Parse(entry.Element(vidal + "ammType")?.Attribute("vidalId")?.Value ?? "0"),
                    BestDocType = (string)entry.Element(vidal + "bestDocType")?.Attribute("name")?.Value,
                    SafetyAlert = bool.Parse(entry.Element(vidal + "safetyAlert")?.Value ?? "false")
                }).ToList();

            return products;
        }

        public static List<UCDVMolecule> ParseUcdvMoleculesXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom"; // Namespace Atom

            var document = XDocument.Parse(xmlContent);

            var molecules = document.Descendants(atom + "entry")
                .Select(entry => new UCDVMolecule
                {
                    Title = (string)entry.Element(atom + "title"),
                    Id = (string)entry.Element(vidal + "id"),
                    Name = (string)entry.Element(vidal + "name"),
                    ItemType = (string)entry.Element(vidal + "itemType")?.Attribute("name")?.Value,
                    PerVolume = (string)entry.Element(vidal + "perVolume"),
                    PerVolumeValue = int.Parse((string)entry.Element(vidal + "perVolumeValue")?.Attribute("roundValue") ?? "0"),
                    PerVolumeUnit = (string)entry.Element(vidal + "perVolumeUnit"),
                    TotalQuantityValue = int.Parse((string)entry.Element(vidal + "totalQuantityValue")?.Attribute("roundValue") ?? "0"),
                    TotalQuantityUnit = (string)entry.Element(vidal + "totalQuantityUnitId")?.Value
                }).ToList();

            return molecules;
        }
    }
}
