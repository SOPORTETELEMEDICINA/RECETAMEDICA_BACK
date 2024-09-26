//using RMD.Models.Vidal;
//using System.Xml.Linq;

//namespace RMD.Extensions
//{
//    public static class XmlParserExtensions
//    {
//        public static List<ProductEntry> ParseProductsXml(this string xmlContent)
//        {
//            XDocument doc = XDocument.Parse(xmlContent);
//            XNamespace ns = "http://www.w3.org/2005/Atom";
//            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

//            var products = doc.Descendants(ns + "entry").Select(entry => new ProductEntry
//            {
//                Title = entry.Element(ns + "title")?.Value ?? string.Empty,
//                ProductLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "PRODUCT")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                PackagesLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "PACKAGES")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                MoleculesLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "MOLECULES")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                ActiveExcipientsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "ACTIVE_EXCIPIENTS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                RecosLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "RECOS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                ForeignProductsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "FOREIGN_PRODUCTS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                IndicationsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "INDICATIONS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                ContraindicationsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "CONTRAINDICATION")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                RestrictedPrescriptionsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "RESTRICTED_PRESCRIPTIONS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                PdsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "PDS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                UcdsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "UCDS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                UnitsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "UNITS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                FoodInteractionsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "FOOD_INTERACTIONS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                PhysicoChemicalInteractionsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "PHYSICO_CHEMICAL_INTERACTIONS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                RoutesLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "ROUTES")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                IndicatorsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "INDICATORS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                SideEffectsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "SIDE_EFFECTS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                AldsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "ALDS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                UcdvsLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "UCDVS")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                AllergiesLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "ALLERGIES")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                AtcClassificationLink = (entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "ATC_CLASSIFICATION")?.Attribute("href")?.Value ?? string.Empty).Replace("/rest/api/product", ""),
//                CategoryTerm = entry.Element(ns + "category")?.Attribute("term")?.Value ?? string.Empty,
//                AuthorName = entry.Element(ns + "author")?.Element(ns + "name")?.Value ?? string.Empty,
//                ProductId = entry.Element(ns + "id")?.Value.Split('/').LastOrDefault() ?? string.Empty,
//                Updated = DateTime.TryParse(entry.Element(ns + "updated")?.Value, out DateTime updatedDate) ? updatedDate : (DateTime?)null,
//                Summary = entry.Element(ns + "summary")?.Value ?? string.Empty,
//                Name = entry.Element(vidalNs + "name")?.Value ?? string.Empty,
//                ItemType = entry.Element(vidalNs + "itemType")?.Attribute("name")?.Value ?? string.Empty,
//                MarketStatus = entry.Element(vidalNs + "marketStatus")?.Attribute("name")?.Value ?? string.Empty,
//                HasPublishedDoc = bool.TryParse(entry.Element(vidalNs + "hasPublishedDoc")?.Value, out bool hasPublishedDoc) && hasPublishedDoc,
//                WithoutPrescription = bool.TryParse(entry.Element(vidalNs + "withoutPrescription")?.Value, out bool withoutPrescription) && withoutPrescription,
//                AmmType = entry.Element(vidalNs + "ammType")?.Value ?? string.Empty,
//                BestDocType = entry.Element(vidalNs + "bestDocType")?.Value ?? string.Empty,
//                SafetyAlert = bool.TryParse(entry.Element(vidalNs + "safetyAlert")?.Value, out bool safetyAlert) && safetyAlert,
//                Company = entry.Element(vidalNs + "company")?.Value ?? string.Empty,
//                Vmp = entry.Element(vidalNs + "vmp")?.Value ?? string.Empty,
//                GalenicForm = entry.Element(vidalNs + "galenicForm")?.Value ?? string.Empty
//            }).ToList();

//            return products;
//        }

//        public static ProductDetail ParseProductDetailXml(this string xmlContent)
//        {
//            XDocument doc = XDocument.Parse(xmlContent);
//            XNamespace ns = "http://www.w3.org/2005/Atom";
//            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

//            var entry = doc.Descendants(ns + "entry").FirstOrDefault();
//            if (entry == null) return null;

//            var productDetail = new ProductDetail
//            {
//                Title = entry.Element(ns + "title")?.Value ?? string.Empty,
//                ProductId = entry.Element(vidalNs + "id")?.Value.Split('/').Last() ?? string.Empty,
//                Updated = DateTime.TryParse(entry.Element(ns + "updated")?.Value, out DateTime updatedDate) ? updatedDate : default,
//                Summary = entry.Element(ns + "summary")?.Value ?? string.Empty,
//                Name = entry.Element(vidalNs + "name")?.Value ?? string.Empty,
//                ItemType = entry.Element(vidalNs + "itemType")?.Value ?? string.Empty,
//                MarketStatus = entry.Element(vidalNs + "marketStatus")?.Attribute("name")?.Value ?? string.Empty,
//                HasPublishedDoc = bool.TryParse(entry.Element(vidalNs + "hasPublishedDoc")?.Value, out bool hasPublishedDoc) && hasPublishedDoc,
//                WithoutPrescription = bool.TryParse(entry.Element(vidalNs + "withoutPrescription")?.Value, out bool withoutPrescription) && withoutPrescription,
//                AmmType = entry.Element(vidalNs + "ammType")?.Value ?? string.Empty,
//                BestDocType = entry.Element(vidalNs + "bestDocType")?.Value ?? string.Empty,
//                SafetyAlert = bool.TryParse(entry.Element(vidalNs + "safetyAlert")?.Value, out bool safetyAlert) && safetyAlert,
//                ActivePrinciples = entry.Element(vidalNs + "activePrinciples")?.Value ?? string.Empty,
//                Company = entry.Element(vidalNs + "company")?.Value ?? string.Empty,
//                GalenicForm = entry.Element(vidalNs + "galenicForm")?.Value ?? string.Empty
//            };

//            return productDetail;
//        }

//        public static List<Package> ParsePackagesXml(this string xmlContent)
//        {
//            XDocument doc = XDocument.Parse(xmlContent);
//            XNamespace ns = "http://www.w3.org/2005/Atom";
//            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

//            var packages = doc.Descendants(ns + "entry").Select(entry => new Package
//            {
//                Title = entry.Element(ns + "title")?.Value ?? string.Empty,
//                PackageLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "PACKAGE")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                LargerPacksLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "LARGER_PACKS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                AffiliationCentersLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "AFFILIATION_CENTER")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                PdsLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "PDS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                PricingScheduleLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "PRICING_SCHEDULE")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                UnitsLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "UNITS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                RoutesLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "ROUTES")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                IndicatorsLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "INDICATORS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                IndicationsLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "INDICATIONS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                SideEffectsLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "SIDE_EFFECTS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                AldsLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "ALDS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                VatRateExceptionAffiliationCentersLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "VAT_EXC_AFFILIATION_CENTER")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                RefundIndicationsLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "REFUND_INDICATIONS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                MoleculesLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "MOLECULES")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                ActiveExcipientsLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "ACTIVE_EXCIPIENTS")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                OptDocumentLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "OPT_DOCUMENT")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                DocumentLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "DOCUMENT")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                UcdLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "UCD")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                CategoryTerm = entry.Element(ns + "category")?.Attribute("term")?.Value ?? string.Empty,
//                AuthorName = entry.Element(ns + "author")?.Element(ns + "name")?.Value ?? string.Empty,
//                PackageId = entry.Element(ns + "id")?.Value.Split('/').Last() ?? string.Empty,
//                Updated = DateTime.TryParse(entry.Element(ns + "updated")?.Value, out DateTime updatedDate) ? updatedDate : default,
//                Summary = entry.Element(ns + "summary")?.Value ?? string.Empty,
//                Name = entry.Element(vidalNs + "name")?.Value ?? string.Empty,
//                ItemType = entry.Element(vidalNs + "itemType")?.Value ?? string.Empty,
//                MarketStatus = entry.Element(vidalNs + "marketStatus")?.Attribute("name")?.Value ?? string.Empty,
//                Otc = bool.TryParse(entry.Element(vidalNs + "otc")?.Value, out bool otc) && otc,
//                IsCeps = bool.TryParse(entry.Element(vidalNs + "isCeps")?.Value, out bool isCeps) && isCeps,
//                DrugId = entry.Element(vidalNs + "drugId")?.Value ?? string.Empty,
//                Cip13 = entry.Element(vidalNs + "cip13")?.Value ?? string.Empty,
//                ShortLabel = entry.Element(vidalNs + "shortLabel")?.Value ?? string.Empty,
//                Tfr = bool.TryParse(entry.Element(vidalNs + "tfr")?.Value, out bool tfr) && tfr,
//                Company = entry.Element(vidalNs + "company")?.Value ?? string.Empty,
//                NarcoticPrescription = bool.TryParse(entry.Element(vidalNs + "narcoticPrescription")?.Value, out bool narcoticPrescription) && narcoticPrescription,
//                SafetyAlert = bool.TryParse(entry.Element(vidalNs + "safetyAlert")?.Value, out bool safetyAlert) && safetyAlert,
//                WithoutPrescription = bool.TryParse(entry.Element(vidalNs + "withoutPrescription")?.Value, out bool withoutPrescription) && withoutPrescription,
//                Product = entry.Element(vidalNs + "product")?.Value ?? string.Empty,
//                GalenicForm = entry.Element(vidalNs + "galenicForm")?.Value ?? string.Empty,
//                Ucd = entry.Element(vidalNs + "ucd")?.Value ?? string.Empty,
//            }).ToList();

//            return packages;
//        }

//        public static List<MoleculeEntry> ParseMoleculesXml(this string xmlContent)
//        {
//            XDocument doc = XDocument.Parse(xmlContent);
//            XNamespace ns = "http://www.w3.org/2005/Atom";
//            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

//            var molecules = doc.Descendants(ns + "entry").Select(entry => new MoleculeEntry
//            {
//                Title = entry.Element(ns + "title")?.Value ?? string.Empty,
//                MoleculeId = entry.Element(vidalNs + "id")?.Value ?? string.Empty,
//                AlternateLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("rel")?.Value == "alternate")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                UnitLink = entry.Elements(ns + "link").FirstOrDefault(l => l.Attribute("title")?.Value == "UNIT")?.Attribute("href")?.Value.Replace("/rest/api/", "") ?? string.Empty,
//                Summary = entry.Element(ns + "summary")?.Value ?? string.Empty,
//                SafetyAlert = bool.TryParse(entry.Element(vidalNs + "safetyAlert")?.Value, out bool safetyAlert) && safetyAlert,
//                Ranking = int.TryParse(entry.Element(vidalNs + "ranking")?.Value, out int ranking) ? ranking : 0,
//                ItemType = entry.Element(vidalNs + "itemType")?.Value ?? string.Empty,
//                PerVolume = entry.Element(vidalNs + "perVolume")?.Value ?? string.Empty,
//                PerVolumeValue = double.TryParse(entry.Element(vidalNs + "perVolumeValue")?.Value, out double perVolumeValue) ? perVolumeValue : 0.0
//            }).ToList();

//            return molecules;
//        }

//        public static IndicationsResponse ParseIndicationsXml(this string xmlContent)
//        {
//            XDocument doc = XDocument.Parse(xmlContent);
//            XNamespace ns = "http://www.w3.org/2005/Atom";
//            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

//            var indications = doc.Descendants(ns + "entry")
//                .Where(entry => entry.Attribute(vidalNs + "categories")?.Value == "INDICATION")
//                .Select(entry => new IndicationEntry
//                {
//                    Title = entry.Element(ns + "title")?.Value ?? string.Empty,
//                    IndicationId = entry.Element(vidalNs + "id")?.Value ?? string.Empty,
//                    Name = entry.Element(vidalNs + "name")?.Value ?? string.Empty,
//                    Type = entry.Element(vidalNs + "type")?.Attribute("name")?.Value ?? string.Empty
//                }).ToList();

//            var indicationGroups = doc.Descendants(ns + "entry")
//                .Where(entry => entry.Attribute(vidalNs + "categories")?.Value == "INDICATION_GROUP")
//                .Select(entry => new IndicationGroupEntry
//                {
//                    Title = entry.Element(ns + "title")?.Value ?? string.Empty,
//                    IndicationGroupId = entry.Element(vidalNs + "id")?.Value ?? string.Empty,
//                    Name = entry.Element(vidalNs + "name")?.Value ?? string.Empty
//                }).ToList();

//            return new IndicationsResponse
//            {
//                Indications = indications,
//                IndicationGroups = indicationGroups
//            };
//        }
//    }
//}
