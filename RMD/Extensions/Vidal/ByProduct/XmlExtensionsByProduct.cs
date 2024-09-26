using RMD.Models.Vidal.ByProduct;
using RMD.Models.Vidal;
using RMD.Models.Vidal.ByRoute;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByProduct
{
    public static class XmlExtensionsByProduct
    {
        
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/
        /***************************************************************************************************************************/

        public static List<Product> ParseProductsXml(this string xml)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xml);

                var products = document.Descendants(atom + "entry").Select(entry => new Product
                {
                    Title = entry.Element(atom + "title")?.Value ?? string.Empty,
                    ProductId = entry.Element(vidal + "id")?.Value ?? string.Empty,
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                    MarketStatus = entry.Element(vidal + "marketStatus")?.Value ?? string.Empty,
                    Company = entry.Element(vidal + "company")?.Value ?? string.Empty,
                    HasPublishedDoc = bool.TryParse(entry.Element(vidal + "hasPublishedDoc")?.Value, out var hasPublishedDoc) && hasPublishedDoc,
                    WithoutPrescription = bool.TryParse(entry.Element(vidal + "withoutPrescription")?.Value, out var withoutPrescription) && withoutPrescription,
                    SafetyAlert = bool.TryParse(entry.Element(vidal + "safetyAlert")?.Value, out var safetyAlert) && safetyAlert
                }).ToList();

                return products;
            }
            catch (Exception ex)
            {
                // Manejar la excepción y registrar el error
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return [];  // Retornar una lista vacía en caso de error
            }
        }

        public static ProductById ParseProductByIdXml(this string xml)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xml);

            var entry = document.Descendants(atom + "entry").FirstOrDefault();
            if (entry == null)
                return null;

            // Extraer el ID del VMP del enlace
            var vmpLink = entry.Elements(atom + "link")
                               .FirstOrDefault(link => (string)link.Attribute("title") == "VMP");
            string idVMP = vmpLink != null ? vmpLink.Attribute("href")?.Value.Split('/').Last() : string.Empty;

            return new ProductById
            {
                Id = entry.Element(vidal + "id")?.Value,
                Title = entry.Element(atom + "title")?.Value,
                Summary = entry.Element(atom + "summary")?.Value,
                Name = entry.Element(vidal + "name")?.Value,
                ItemType = entry.Element(vidal + "itemType")?.Attribute("name")?.Value,
                MarketStatus = entry.Element(vidal + "marketStatus")?.Value,
                HasPublishedDoc = bool.TryParse(entry.Element(vidal + "hasPublishedDoc")?.Value, out bool hasDoc) && hasDoc,
                WithoutPrescription = bool.TryParse(entry.Element(vidal + "withoutPrescription")?.Value, out bool withoutPresc) && withoutPresc,
                AmmType = entry.Element(vidal + "ammType")?.Attribute("vidalId")?.Value,
                BestDocType = entry.Element(vidal + "bestDocType")?.Attribute("name")?.Value,
                SafetyAlert = bool.TryParse(entry.Element(vidal + "safetyAlert")?.Value, out bool safetyAlert) && safetyAlert,
                ActivePrinciples = entry.Element(vidal + "activePrinciples")?.Value,
                Company = entry.Element(vidal + "company")?.Value,
                Vmp = entry.Element(vidal + "vmp")?.Value,
                GalenicForm = entry.Element(vidal + "galenicForm")?.Value,
                IdVMP = idVMP
            };
        }


        // Nuevo método
        // Nuevo método
        public static List<ProductPackage> ParseProductPackagesXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var packages = document.Descendants("entry")
                .Select(entry => new ProductPackage
                {
                    Title = (string)entry.Element("title"),
                    Id = (string)entry.Element("id"),
                    Name = (string)entry.Element(vidal + "name"),
                    ItemType = (string)entry.Element(vidal + "itemType")?.Attribute("name"),
                    ProductId = int.Parse(entry.Element(vidal + "productId")?.Value ?? "0"),
                    MarketStatus = (string)entry.Element(vidal + "marketStatus")?.Attribute("name"),
                    Otc = bool.Parse(entry.Element(vidal + "otc")?.Value ?? "false"),
                    IsCeps = bool.Parse(entry.Element(vidal + "isCeps")?.Value ?? "false"),
                    DrugId = int.Parse(entry.Element(vidal + "drugId")?.Value ?? "0"),
                    Cip13 = (string)entry.Element(vidal + "cip13"),
                    ShortLabel = (string)entry.Element(vidal + "shortLabel"),
                    Tfr = bool.Parse(entry.Element(vidal + "tfr")?.Value ?? "false"),
                    NarcoticPrescription = bool.Parse(entry.Element(vidal + "narcoticPrescription")?.Value ?? "false"),
                    SafetyAlert = bool.Parse(entry.Element(vidal + "safetyAlert")?.Value ?? "false"),
                    WithoutPrescription = bool.Parse(entry.Element(vidal + "withoutPrescription")?.Value ?? "false"),
                    Product = (string)entry.Element(vidal + "product"),
                    GalenicForm = (string)entry.Element(vidal + "galenicForm"),
                    CompanyVidalId = int.Parse(entry.Element(vidal + "company")?.Attribute("vidalId")?.Value ?? "0"),
                    CompanyName = (string)entry.Element(vidal + "company"),
                    PackageId = int.Parse(entry.Element(vidal + "id")?.Value.Split('/').Last() ?? "0"),
                    UcdId = int.Parse(entry.Element(vidal + "ucd")?.Attribute("id")?.Value ?? "0")
                }).ToList();

            return packages;
        }



        public static List<ProductMolecule> ParseProductMoleculesXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var molecules = document.Descendants("entry")
                .Select(entry => new ProductMolecule
                {
                    Title = (string)entry.Element("title"),
                    Id = (string)entry.Element("id"),
                    FullName = (string)entry.Element(vidal + "fullName"),
                    Reconstitued = bool.Parse(entry.Element(vidal + "reconstitued")?.Value ?? "false"),
                    HeaderName = (string)entry.Element(vidal + "headerName"),
                    HeaderQuantity = double.Parse(entry.Element(vidal + "headerQuantity")?.Value ?? "0"),
                    MoleculeTitle = (string)entry.Element("title"),
                    MoleculeId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    SafetyAlert = bool.Parse(entry.Element(vidal + "safetyAlert")?.Value ?? "false"),
                    Ranking = int.Parse(entry.Element(vidal + "ranking")?.Value ?? "0"),
                    ItemType = (string)entry.Element(vidal + "itemType")?.Attribute("name"),
                    PerVolume = (string)entry.Element(vidal + "perVolume"),
                    PerVolumeValue = double.Parse(entry.Element(vidal + "perVolumeValue")?.Value ?? "0"),
                    UnitId = int.Parse(entry.Descendants("link")
                                            .Where(x => x.Attribute("title")?.Value == "UNIT")
                                            .Select(x => x.Attribute("href")?.Value.Split('/').Last())
                                            .FirstOrDefault() ?? "0")
                }).ToList();

            return molecules;
        }

        public static List<ProductForeign> ParseProductForeignXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var foreignProducts = document.Descendants("entry")
                .Select(entry => new ProductForeign
                {
                    Title = (string)entry.Element("title"),
                    Id = (string)entry.Element("id"),
                    Name = (string)entry.Element(vidal + "name"),
                    LocalName = (string)entry.Element(vidal + "localName"),
                    Route = (string)entry.Element(vidal + "route"),
                    RouteId = int.Parse(entry.Element(vidal + "route")?.Attribute("id")?.Value ?? "0"),
                    GalenicForm = (string)entry.Element(vidal + "galenicForm"),
                    GalenicFormId = int.Parse(entry.Element(vidal + "galenicForm")?.Attribute("vidalId")?.Value ?? "0"),
                    Country = (string)entry.Element(vidal + "country"),
                    CountryCode = (string)entry.Element(vidal + "country")?.Attribute("countryCode"),
                    AtcClass = (string)entry.Element(vidal + "atcClass"),
                    AtcCode = (string)entry.Element(vidal + "atcClass")?.Attribute("code"),
                    ForeignProductId = int.Parse(entry.Elements("link") // Cambiado a Elements para obtener todos los elementos
                                              .Where(x => x.Attribute("title")?.Value == "PRODUCTS")
                                              .Select(x => x.Attribute("href")?.Value.Split('/').Last())
                                              .FirstOrDefault() ?? "0")
                }).ToList();

            return foreignProducts;
        }


        public static List<ProductIndication> ParseProductIndicationsXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var indications = document.Descendants("entry")
                .Select(entry => new ProductIndication
                {
                    Title = (string)entry.Element("title"),
                    Id = (string)entry.Element("id"),
                    Name = (string)entry.Element(vidal + "name"),
                    Type = (string)entry.Element(vidal + "type")?.Attribute("name"),
                    IndicationGroupId = entry.Elements("link")
                                             .FirstOrDefault(x => x.Attribute("title")?.Value == "INDICATION_GROUP")
                                             ?.Attribute("href")?.Value.Split('/').Last() ?? string.Empty,
                    IndicationId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0")
                }).ToList();

            return indications;
        }

        public static List<ProductUcd> ParseProductUcdsXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var ucds = document.Descendants("entry")
                .Select(entry => new ProductUcd
                {
                    Title = (string)entry.Element("title"),
                    Id = (string)entry.Element("id"),
                    Name = (string)entry.Element(vidal + "name"),
                    MarketStatus = (string)entry.Element(vidal + "marketStatus")?.Attribute("name"),
                    UcdId = (string)entry.Element(vidal + "ucd"),
                    Ucd13 = (string)entry.Element(vidal + "ucd13"),
                    SafetyAlert = bool.TryParse(entry.Element(vidal + "safetyAlert")?.Value, out bool safetyAlert) && safetyAlert,
                    ProductId = int.Parse(entry.Elements("link")
                                                .FirstOrDefault(x => x.Attribute("title")?.Value == "PRODUCT")
                                                ?.Attribute("href")?.Value.Split('/').Last() ?? "0"),
                    UcdLinkedId = int.Parse(entry.Elements("link")
                                                 .FirstOrDefault(x => x.Attribute("title")?.Value == "UCD")
                                                 ?.Attribute("href")?.Value.Split('/').Last() ?? "0")
                }).ToList();

            return ucds;
        }

        public static List<ProductUnit> ParseProductUnitsXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var units = document.Descendants("entry")
                .Select(entry => new ProductUnit
                {
                    Title = (string)entry.Element("title"),
                    Id = (string)entry.Element("id"),
                    UnitId = int.Parse(entry.Element(vidal + "unitId")?.Value ?? "0"),
                    Name = (string)entry.Element(vidal + "name"),
                    SingularName = (string)entry.Element(vidal + "singularName"),
                    ParentConversionRateDenominator = int.Parse(entry.Element(vidal + "parentConversionRate")?.Attribute("denominator")?.Value ?? "0"),
                    ParentConversionRateNumerator = int.Parse(entry.Element(vidal + "parentConversionRate")?.Attribute("numerator")?.Value ?? "0"),
                    ParentConversionRateUnitId = int.Parse(entry.Element(vidal + "parentConversionRate")?.Attribute("unitId")?.Value ?? "0"),
                    DerivedByWeight = (string)entry.Element(vidal + "derivedByWeight")?.Attribute("name"),
                    DerivedBySize = (string)entry.Element(vidal + "derivedBySize")?.Attribute("name"),
                    Rank = int.Parse(entry.Element(vidal + "rank")?.Value ?? "0"),
                    ConversionRateRefUnit = int.Parse(entry.Element(vidal + "ref_unit")?.Value ?? "0"),
                    ConversionRateUnit = int.Parse(entry.Element(vidal + "unit")?.Value ?? "0"),
                    ConversionRate = (string)entry.Element(vidal + "rate"),
                    Coeff = double.Parse(entry.Element(vidal + "coeff")?.Value ?? "0")
                }).ToList();

            return units;
        }

        public static List<ProductRoute> ParseProductRoutesXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var routes = document.Descendants("entry")
                .Select(entry => new ProductRoute
                {
                    Title = (string)entry.Element("title"),
                    Id = (string)entry.Element("id"),
                    Name = (string)entry.Element(vidal + "name"),
                    RouteId = int.Parse(entry.Element(vidal + "routeId")?.Value ?? "0"),
                    Ranking = int.Parse(entry.Element(vidal + "ranking")?.Value ?? "0"),
                    OutOfSPC = bool.Parse(entry.Element(vidal + "outOfSPC")?.Value ?? "false"),
                    ParentId = int.Parse(entry.Element(vidal + "parentId")?.Value ?? "0")
                }).ToList();

            return routes;
        }

        public static ProductIndicator ParseProductIndicatorsXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var entry = document.Descendants("entry").FirstOrDefault();
            if (entry == null)
                return null;

            var indicators = entry.Elements(vidal + "indicator")
                .Select(indicator => new Indicator
                {
                    VidalId = int.Parse(indicator.Attribute("vidalId")?.Value ?? "0"),
                    Name = indicator.Value
                }).ToList();

            return new ProductIndicator
            {
                Title = (string)entry.Element("title"),
                Id = (string)entry.Element("id"),
                Indicators = indicators
            };
        }

        public static ProductSideEffect ParseProductSideEffectsXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var entry = document.Descendants("entry").FirstOrDefault();
            if (entry == null)
                return null;

            var sideEffects = document.Descendants("entry")
                .Select(effect => new SideEffect
                {
                    VidalId = int.Parse(effect.Element(vidal + "id")?.Value ?? "0"),
                    Name = effect.Element(vidal + "name")?.Value,
                    ApparatusVidalId = int.Parse(effect.Element(vidal + "apparatus")?.Attribute("vidalId")?.Value ?? "0"),
                    ApparatusName = effect.Element(vidal + "apparatus")?.Value,
                    Frequency = effect.Element(vidal + "frequency")?.Attribute("name")?.Value ?? string.Empty
                }).ToList();

            return new ProductSideEffect
            {
                Title = (string)entry.Element("title"),
                Id = (string)entry.Element("id"),
                SideEffects = sideEffects
            };
        }

        public static ProductUCDV ParseProductUCDVXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var entry = document.Descendants("entry").FirstOrDefault();
            if (entry == null)
                return null;

            var ucdvEntries = document.Descendants("entry")
                .Select(ucdv => new UCDVEntry
                {
                    VidalId = int.Parse(ucdv.Element(vidal + "id")?.Value ?? "0"),
                    Name = ucdv.Element(vidal + "name")?.Value,
                    GalenicFormVidalId = int.Parse(ucdv.Element(vidal + "galenicForm")?.Attribute("vidalId")?.Value ?? "0"),
                    GalenicFormName = ucdv.Element(vidal + "galenicForm")?.Value,
                    VmpId = int.Parse(ucdv.Elements("link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "VMP")
                        ?.Attribute("href")?.Value.Split('/').Last() ?? "0"),
                }).ToList();

            return new ProductUCDV
            {
                Title = (string)entry.Element("title"),
                Id = (string)entry.Element("id"),
                UCDVEntries = ucdvEntries
            };
        }

        public static ProductAllergy ParseProductAllergyXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var entry = document.Descendants("entry").FirstOrDefault();
            if (entry == null)
                return null;

            var allergyEntries = document.Descendants("entry")
                .Select(allergy => new AllergyEntry
                {
                    AllergyId = int.Parse(allergy.Element(vidal + "id")?.Value ?? "0"),
                    Name = allergy.Element(vidal + "name")?.Value,
                    MoleculesId = int.Parse(allergy.Elements("link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "MOLECULES")
                        ?.Attribute("href")?.Value.Split('/').Last() ?? "0"),
                    CrossAllergiesId = int.Parse(allergy.Elements("link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "ALLERGIES")
                        ?.Attribute("href")?.Value.Split('/').Last() ?? "0"),
                }).ToList();

            return new ProductAllergy
            {
                Title = (string)entry.Element("title"),
                Id = (string)entry.Element("id"),
                AllergyEntries = allergyEntries
            };
        }

        public static ProductAtcClassification ParseProductAtcClassificationXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var entry = document.Descendants("entry").FirstOrDefault();
            if (entry == null)
                return null;

            var atcEntries = document.Descendants("entry")
                .Select(atc => new AtcEntry
                {
                    AtcId = int.Parse(atc.Element(vidal + "id")?.Value ?? "0"),
                    Name = atc.Element(vidal + "name")?.Value,
                    Code = atc.Element(vidal + "code")?.Value,
                    VmpsId = int.Parse(atc.Elements("link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "VMPS")
                        ?.Attribute("href")?.Value.Split('/').Skip(3).FirstOrDefault() ?? "0"),
                    ProductsId = int.Parse(atc.Elements("link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "PRODUCTS")
                        ?.Attribute("href")?.Value.Split('/').Skip(3).FirstOrDefault() ?? "0"),
                    ChildrenId = int.Parse(atc.Elements("link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "CHILDREN")
                        ?.Attribute("href")?.Value.Split('/').Skip(3).FirstOrDefault() ?? "0"),
                    MoleculesId = int.Parse(atc.Elements("link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "MOLECULES")
                        ?.Attribute("href")?.Value.Split('/').Skip(3).FirstOrDefault() ?? "0"),
                    ParentId = int.Parse(atc.Elements("link")
                        .FirstOrDefault(link => (string)link.Attribute("title") == "PARENT")
                        ?.Attribute("href")?.Value.Split('/').Skip(3).FirstOrDefault() ?? "0")
                }).ToList();

            return new ProductAtcClassification
            {
                Title = (string)entry.Element("title"),
                Id = (string)entry.Element("id"),
                AtcEntries = atcEntries
            };
        }

        public static ProductVMPGroup ParseProductVmpGroupXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var entry = document.Descendants("entry").FirstOrDefault();
            if (entry == null)
                return null;

            return new ProductVMPGroup
            {
                Id = entry.Element(vidal + "id")?.Value,
                Name = entry.Element(vidal + "name")?.Value,
                Route = entry.Element(vidal + "route")?.Value,
                GalenicForm = entry.Element(vidal + "galenicForm")?.Value,
                ActivePrinciples = entry.Element(vidal + "activePrinciples")?.Value
            };
        }

        public static List<ProductUnit> PackageUnitByLinkXmlParser(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var units = document.Descendants("entry")
                .Select(entry => new ProductUnit
                {
                    Title = (string)entry.Element("title"),
                    Id = (string)entry.Element("id"),
                    UnitId = int.Parse(entry.Element(vidal + "unitId")?.Value ?? "0"),
                    Name = (string)entry.Element(vidal + "name"),
                    SingularName = (string)entry.Element(vidal + "singularName"),
                    ParentConversionRateDenominator = int.Parse(entry.Element(vidal + "parentConversionRate")?.Attribute("denominator")?.Value ?? "0"),
                    ParentConversionRateNumerator = int.Parse(entry.Element(vidal + "parentConversionRate")?.Attribute("numerator")?.Value ?? "0"),
                    ParentConversionRateUnitId = int.Parse(entry.Element(vidal + "parentConversionRate")?.Attribute("unitId")?.Value ?? "0"),
                    DerivedByWeight = (string)entry.Element(vidal + "derivedByWeight")?.Attribute("name"),
                    DerivedBySize = (string)entry.Element(vidal + "derivedBySize")?.Attribute("name"),
                    Rank = int.Parse(entry.Element(vidal + "rank")?.Value ?? "0"),
                    ConversionRateRefUnit = int.Parse(entry.Element(vidal + "ref_unit")?.Value ?? "0"),
                    ConversionRateUnit = int.Parse(entry.Element(vidal + "unit")?.Value ?? "0"),
                    ConversionRate = (string)entry.Element(vidal + "rate"),
                    Coeff = double.Parse(entry.Element(vidal + "coeff")?.Value ?? "0")
                }).ToList();

            return units;
        }

        public static List<ProductEntry> ParseProductConsultaXml(this string xmlContent)
        {
            var productEntries = new List<ProductEntry>();
            var xmlDoc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            foreach (var entry in xmlDoc.Descendants(atomNs + "entry"))
            {
                var productEntry = new ProductEntry
                {
                    Title = entry.Element(atomNs + "title")?.Value,
                    Id = entry.Element(atomNs + "id")?.Value,
                    VidalId = entry.Element(ns + "id")?.Value,
                    Name = entry.Element(ns + "name")?.Value,
                    MarketStatus = entry.Element(ns + "marketStatus")?.Value,
                    ActivePrinciples = entry.Element(ns + "activePrinciples")?.Value,
                    GalenicForm = entry.Element(ns + "galenicForm")?.Value,
                    SafetyAlert = bool.Parse(entry.Element(ns + "safetyAlert")?.Value ?? "false"),
                    Summary = entry.Element(atomNs + "summary")?.Value
                };

                foreach (var link in entry.Elements(atomNs + "link"))
                {
                    productEntry.RelatedLinks.Add(new RelatedLink
                    {
                        Relation = link.Attribute("rel")?.Value,
                        Type = link.Attribute("type")?.Value,
                        Href = link.Attribute("href")?.Value,
                        Title = link.Attribute("title")?.Value
                    });
                }

                productEntries.Add(productEntry);
            }

            return productEntries;
        }
    }
}
