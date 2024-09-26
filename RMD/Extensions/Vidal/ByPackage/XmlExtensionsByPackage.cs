using RMD.Models.Consulta;
using RMD.Models.Vidal.ByPackage;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByPackage
{
    public static class XmlExtensionsByPackage
    {
        public static PackageById ParsePackageXml(this string xml)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xml);

            var entry = document.Descendants(atom + "entry").FirstOrDefault();

            if (entry == null) return null;

            return new PackageById
            {
                Title = entry.Element(atom + "title")?.Value,
                VidalId = entry.Element(vidal + "id")?.Value,
                Name = entry.Element(vidal + "name")?.Value,
                ItemType = entry.Element(vidal + "itemType")?.Attribute("name")?.Value,
                ProductId = int.Parse(entry.Element(vidal + "productId")?.Value ?? "0"),
                MarketStatus = entry.Element(vidal + "marketStatus")?.Attribute("name")?.Value,
                OTC = bool.Parse(entry.Element(vidal + "otc")?.Value ?? "false"),
                IsCeps = bool.Parse(entry.Element(vidal + "isCeps")?.Value ?? "false"),
                DrugId = int.Parse(entry.Element(vidal + "drugId")?.Value ?? "0"),
                Cip13 = entry.Element(vidal + "cip13")?.Value,
                ShortLabel = entry.Element(vidal + "shortLabel")?.Value,
                TFR = bool.Parse(entry.Element(vidal + "tfr")?.Value ?? "false"),
                Company = entry.Element(vidal + "company")?.Value,
                NarcoticPrescription = bool.Parse(entry.Element(vidal + "narcoticPrescription")?.Value ?? "false"),
                SafetyAlert = bool.Parse(entry.Element(vidal + "safetyAlert")?.Value ?? "false"),
                WithoutPrescription = bool.Parse(entry.Element(vidal + "withoutPrescription")?.Value ?? "false"),
                Product = entry.Element(vidal + "product")?.Value,
                GalenicForm = entry.Element(vidal + "galenicForm")?.Value,
                UCD = entry.Element(vidal + "ucd")?.Value,
            };
        }

        public static List<Package> ParsePackagesXml(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);

            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var packages = doc.Descendants(ns + "entry").Select(entry => new Package
            {
                Id = (int)entry.Element(vidalNs + "id"),
                Name = (string)entry.Element(vidalNs + "name"),
                ProductId = (int)entry.Element(vidalNs + "productId"),
                MarketStatus = (string)entry.Element(vidalNs + "marketStatus"),
                Otc = (bool)entry.Element(vidalNs + "otc"),
                IsCeps = (bool)entry.Element(vidalNs + "isCeps"),
                DrugId = (int)entry.Element(vidalNs + "drugId"),
                Cip13 = (string)entry.Element(vidalNs + "cip13"),
                ShortLabel = (string)entry.Element(vidalNs + "shortLabel"),
                Tfr = (bool)entry.Element(vidalNs + "tfr"),
                Company = (string)entry.Element(vidalNs + "company"),
                NarcoticPrescription = (bool)entry.Element(vidalNs + "narcoticPrescription"),
                SafetyAlert = (bool)entry.Element(vidalNs + "safetyAlert"),
                WithoutPrescription = (bool)entry.Element(vidalNs + "withoutPrescription"),
                Product = (string)entry.Element(vidalNs + "product"),
                GalenicForm = (string)entry.Element(vidalNs + "galenicForm"),
                Ucd = (string)entry.Element(vidalNs + "ucd")
            }).ToList();

            return packages;
        }

        public static List<PackageRoute> ParsePackageRoutesXml(this string xmlContent)
        {
            XDocument document = XDocument.Parse(xmlContent);
            XNamespace atomNamespace = "http://www.w3.org/2005/Atom";
            XNamespace vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var routes = document.Descendants(atomNamespace + "entry").Select(entry => new PackageRoute
            {
                Title = entry.Element(atomNamespace + "title")?.Value,
                Link = entry.Element(atomNamespace + "link")?.Attribute("href")?.Value,
                Id = (int)(entry.Element(vidalNamespace + "id") ?? new XElement("id", 0)),
                Name = entry.Element(vidalNamespace + "name")?.Value,
                RouteId = (int)(entry.Element(vidalNamespace + "routeId") ?? new XElement("routeId", 0)),
                Ranking = (int)(entry.Element(vidalNamespace + "ranking") ?? new XElement("ranking", 0)),
                OutOfSPC = bool.Parse(entry.Element(vidalNamespace + "outOfSPC")?.Value ?? "false"),
                ParentId = (int)(entry.Element(vidalNamespace + "parentId") ?? new XElement("parentId", 0))
            }).ToList();

            return routes;
        }

        public static List<PackageIndicator> ParsePackageIndicatorsXml(this string xmlContent)
        {
            XDocument document = XDocument.Parse(xmlContent);
            XNamespace atomNamespace = "http://www.w3.org/2005/Atom";
            XNamespace vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var indicators = document.Descendants(atomNamespace + "entry").Select(entry => new PackageIndicator
            {
                Title = entry.Element(atomNamespace + "title")?.Value,
                Link = entry.Element(atomNamespace + "link")?.Attribute("href")?.Value,
                Id = entry.Element(atomNamespace + "id")?.Value,
                Indicator = entry.Element(vidalNamespace + "indicator")?.Value,
                VidalId = int.Parse(entry.Element(vidalNamespace + "indicator")?.Attribute("vidalId")?.Value ?? "0")
            }).ToList();

            return indicators;
        }

  
            public static List<PackageUnit> PackageUnitXmlParser(this string xmlContent)
            {
                XDocument doc = XDocument.Parse(xmlContent);
                XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var units = doc.Descendants("entry").Select(entry => new PackageUnit
                {
                    Title = entry.Element("title")?.Value,
                    Id = entry.Element("id")?.Value,
                    Updated = DateTime.Parse(entry.Element("updated")?.Value),
                    VidalCategories = entry.Attribute(vidal + "categories")?.Value,
                    UnitId = int.Parse(entry.Element(vidal + "unitId")?.Value ?? "0"),
                    Name = entry.Element(vidal + "name")?.Value,
                    SingularName = entry.Element(vidal + "singularName")?.Value,
                    ParentConversionRate = entry.Element(vidal + "parentConversionRate")?.Value,
                    DerivedByWeight = entry.Element(vidal + "derivedByWeight")?.Value,
                    DerivedBySize = entry.Element(vidal + "derivedBySize")?.Value,
                    Rank = int.Parse(entry.Element(vidal + "rank")?.Value ?? "0"),
                    RefUnit = int.TryParse(entry.Element(vidal + "ref_unit")?.Value, out int refUnit) ? refUnit : (int?)null,
                    Unit = int.TryParse(entry.Element(vidal + "unit")?.Value, out int unit) ? unit : (int?)null,
                    Rate = entry.Element(vidal + "rate")?.Value,
                    Coeff = double.TryParse(entry.Element(vidal + "coeff")?.Value, out double coeff) ? coeff : (double?)null
                }).ToList();

                return units;
            }
     

        public static List<PackageIndication> PackageIndicationXmlParser(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var indications = doc.Descendants("entry").Select(entry => new PackageIndication
            {
                Title = entry.Element("title")?.Value,
                Id = entry.Element("id")?.Value,
                Updated = DateTime.Parse(entry.Element("updated")?.Value),
                VidalCategories = entry.Attribute(vidal + "categories")?.Value,
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Name = entry.Element(vidal + "name")?.Value,
                Type = entry.Element(vidal + "type")?.Attribute("name")?.Value
            }).ToList();

            return indications;
        }

        public static List<PackageSideEffect> PackageSideEffectsXmlParser(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var sideEffects = doc.Descendants("entry").Select(entry => new PackageSideEffect
            {
                Title = entry.Element("title")?.Value,
                Id = entry.Element("id")?.Value,
                Updated = DateTime.Parse(entry.Element("updated")?.Value),
                VidalCategories = entry.Attribute(vidal + "categories")?.Value,
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Name = entry.Element(vidal + "name")?.Value,
                Apparatus = entry.Element(vidal + "apparatus")?.Value,
                Frequency = entry.Element(vidal + "frequency")?.Attribute("name")?.Value
            }).ToList();

            return sideEffects;
        }

        public static List<PackageAtcClassification> PackageAtcClassificationXmlParser(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var atcClassifications = doc.Descendants("entry").Select(entry => new PackageAtcClassification
            {
                Title = entry.Element("title")?.Value,
                Id = entry.Element("id")?.Value,
                Updated = DateTime.Parse(entry.Element("updated")?.Value),
                VidalCategories = entry.Attribute(vidal + "categories")?.Value,
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Name = entry.Element(vidal + "name")?.Value,
                Code = entry.Element(vidal + "code")?.Value
            }).ToList();

            return atcClassifications;
        }

        public static PackageVmp ParsePackageVmpXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var entry = document.Descendants("entry").FirstOrDefault();
            if (entry == null)
                return null;

            return new PackageVmp
            {
                Id = entry.Element(vidal + "id")?.Value,
                Name = entry.Element(vidal + "name")?.Value,
                MarketStatus = entry.Element(vidal + "marketStatus")?.Value,
                Vmp = entry.Element(vidal + "vmp")?.Value,
                GalenicForm = entry.Element(vidal + "galenicForm")?.Value,
                AtcCode = entry.Element(vidal + "atc")?.Attribute("name")?.Value
            };
        }

        public static PackageProduct ParsePackageProductXml(this string xmlString)
        {
            XDocument xmlDoc = XDocument.Parse(xmlString);

            var entry = xmlDoc.Root.Element("{http://www.w3.org/2005/Atom}entry");

            var packageProduct = new PackageProduct
            {
                Id = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}id")?.Value,
                Title = entry.Element("{http://www.w3.org/2005/Atom}title")?.Value,
                Summary = entry.Element("{http://www.w3.org/2005/Atom}summary")?.Value,
                ProductId = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}productId")?.Value,
                MarketStatus = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}marketStatus")?.Value,
                OTC = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}otc")?.Value,
                IsCeps = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}isCeps")?.Value,
                DrugId = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}drugId")?.Value,
                Cip13 = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}cip13")?.Value,
                ShortLabel = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}shortLabel")?.Value,
                TFR = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}tfr")?.Value,
                Company = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}company")?.Value,
                NarcoticPrescription = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}narcoticPrescription")?.Value,
                SafetyAlert = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}safetyAlert")?.Value,
                WithoutPrescription = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}withoutPrescription")?.Value,
                ProductName = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}product")?.Value,
                GalenicForm = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}galenicForm")?.Value,
                ATCName = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}atc")?.Value,
                VMP = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}vmp")?.Value,
                UCD = entry.Element("{http://api.vidal.net/-/spec/vidal-api/1.0/}ucd")?.Value
            };

            var links = entry.Elements("{http://www.w3.org/2005/Atom}link")
                              .Where(e => e.Attribute("rel")?.Value == "related")
                              .Select(e => e.Attribute("href")?.Value);

            packageProduct.RelatedLinks.AddRange(links);

            return packageProduct;
        }

        public static List<PackageUnit> PackageUnitByLinkXmlParser(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var units = doc.Descendants(atom + "entry").Select(entry => new PackageUnit
            {
                Title = entry.Element(atom + "title")?.Value,
                Id = entry.Element(atom + "id")?.Value,
                Updated = DateTime.Parse(entry.Element(atom + "updated")?.Value),
                VidalCategories = entry.Attribute(vidal + "categories")?.Value,
                UnitId = int.Parse(entry.Element(vidal + "unitId")?.Value ?? "0"),
                Name = entry.Element(vidal + "name")?.Value,
                SingularName = entry.Element(vidal + "singularName")?.Value,
                ParentConversionRate = entry.Element(vidal + "parentConversionRate")?.Value,
                DerivedByWeight = entry.Element(vidal + "derivedByWeight")?.Value,
                DerivedBySize = entry.Element(vidal + "derivedBySize")?.Value,
                Rank = int.Parse(entry.Element(vidal + "rank")?.Value ?? "0"),
                RefUnit = int.TryParse(entry.Element(vidal + "ref_unit")?.Value, out int refUnit) ? refUnit : (int?)null,
                Unit = int.TryParse(entry.Element(vidal + "unit")?.Value, out int unit) ? unit : (int?)null,
                Rate = entry.Element(vidal + "rate")?.Value,
                Coeff = double.TryParse(entry.Element(vidal + "coeff")?.Value, out double coeff) ? coeff : (double?)null
            }).ToList();

            return units;
        }


        public static List<PackageEntry> ParsePackageConsultaXml(this string xmlContent)
        {
            var packageEntries = new List<PackageEntry>();
            var xmlDoc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            foreach (var entry in xmlDoc.Descendants(atomNs + "entry"))
            {
                var packageEntry = new PackageEntry
                {
                    Title = entry.Element(atomNs + "title")?.Value,
                    Id = entry.Element(atomNs + "id")?.Value,
                    VidalId = entry.Element(ns + "id")?.Value,
                    Name = entry.Element(ns + "name")?.Value,
                    ProductId = entry.Element(ns + "productId")?.Value,
                    MarketStatus = entry.Element(ns + "marketStatus")?.Value,
                    Otc = bool.Parse(entry.Element(ns + "otc")?.Value ?? "false"),
                    IsCeps = bool.Parse(entry.Element(ns + "isCeps")?.Value ?? "false"),
                    DrugId = entry.Element(ns + "drugId")?.Value,
                    Cip13 = entry.Element(ns + "cip13")?.Value,
                    ShortLabel = entry.Element(ns + "shortLabel")?.Value,
                    Tfr = bool.Parse(entry.Element(ns + "tfr")?.Value ?? "false"),
                    Company = entry.Element(ns + "company")?.Value,
                    NarcoticPrescription = bool.Parse(entry.Element(ns + "narcoticPrescription")?.Value ?? "false"),
                    SafetyAlert = bool.Parse(entry.Element(ns + "safetyAlert")?.Value ?? "false"),
                    WithoutPrescription = bool.Parse(entry.Element(ns + "withoutPrescription")?.Value ?? "false"),
                    Product = entry.Element(ns + "product")?.Value,
                    GalenicForm = entry.Element(ns + "galenicForm")?.Value,
                    Vmp = entry.Element(ns + "vmp")?.Value,
                    Ucd = entry.Element(ns + "ucd")?.Value,
                    Summary = entry.Element(atomNs + "summary")?.Value
                };

                foreach (var link in entry.Elements(atomNs + "link"))
                {
                    packageEntry.RelatedLinks.Add(new RelatedLink
                    {
                        Relation = link.Attribute("rel")?.Value,
                        Type = link.Attribute("type")?.Value,
                        Href = link.Attribute("href")?.Value,
                        Title = link.Attribute("title")?.Value
                    });
                }

                packageEntries.Add(packageEntry);
            }

            return packageEntries;
        }
    }
}
