using RMD.Models.Vidal.ByIndication;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByIndication
{
    public static class IndicationXmlParser
    {
        public static IndicationDetail ParseIndicationXml(this string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entry = doc.Descendants(ns + "entry").FirstOrDefault();

            if (entry != null)
            {
                return new IndicationDetail
                {
                    Title = entry.Element(ns + "title")?.Value,
                    SelfLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("rel")?.Value == "self")?.Attribute("href")?.Value,
                    AlternateLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "INDICATION")?.Attribute("href")?.Value,
                    ProductsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "PRODUCTS")?.Attribute("href")?.Value,
                    VmpsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "VMPS")?.Attribute("href")?.Value,
                    Id = entry.Element(vidal + "id")?.Value,
                    Name = entry.Element(vidal + "name")?.Value
                };
            }

            return null;
        }

        public static List<IndicationProduct> ParseIndicationProductsXml(this string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var products = doc.Descendants(ns + "entry")
                .Select(entry => new IndicationProduct
                {
                    Title = entry.Element(ns + "title")?.Value,
                    AlternateLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "PRODUCT")?.Attribute("href")?.Value,
                    PackagesLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "PACKAGES")?.Attribute("href")?.Value,
                    MoleculesLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "MOLECULES")?.Attribute("href")?.Value,
                    ActiveExcipientsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ACTIVE_EXCIPIENTS")?.Attribute("href")?.Value,
                    RecosLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "RECOS")?.Attribute("href")?.Value,
                    ForeignProductsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "FOREIGN_PRODUCTS")?.Attribute("href")?.Value,
                    IndicationsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "INDICATIONS")?.Attribute("href")?.Value,
                    ContraindicationsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "CONTRAINDICATION")?.Attribute("href")?.Value,
                    RestrictedPrescriptionsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "RESTRICTED_PRESCRIPTIONS")?.Attribute("href")?.Value,
                    PdsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "PDS")?.Attribute("href")?.Value,
                    UcdsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "UCDS")?.Attribute("href")?.Value,
                    UnitsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "UNITS")?.Attribute("href")?.Value,
                    FoodInteractionsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "FOOD_INTERACTIONS")?.Attribute("href")?.Value,
                    PhysicoChemicalInteractionsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "PHYSICO_CHEMICAL_INTERACTIONS")?.Attribute("href")?.Value,
                    RoutesLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ROUTES")?.Attribute("href")?.Value,
                    IndicatorsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "INDICATORS")?.Attribute("href")?.Value,
                    SideEffectsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "SIDE_EFFECTS")?.Attribute("href")?.Value,
                    AldsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ALDS")?.Attribute("href")?.Value,
                    UcdvsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "UCDVS")?.Attribute("href")?.Value,
                    AllergiesLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ALLERGIES")?.Attribute("href")?.Value,
                    AtcClassificationLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ATC_CLASSIFICATION")?.Attribute("href")?.Value,
                    VmpLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "VMP")?.Attribute("href")?.Value,
                    Id = entry.Element(vidal + "id")?.Value,
                    Name = entry.Element(vidal + "name")?.Value,
                    Summary = entry.Element(ns + "summary")?.Value,
                    ItemType = entry.Element(vidal + "itemType")?.Attribute("name")?.Value,
                    MarketStatus = entry.Element(vidal + "marketStatus")?.Attribute("name")?.Value,
                    HasPublishedDoc = bool.Parse(entry.Element(vidal + "hasPublishedDoc")?.Value ?? "false"),
                    WithoutPrescription = bool.Parse(entry.Element(vidal + "withoutPrescription")?.Value ?? "false"),
                    AmmType = entry.Element(vidal + "ammType")?.Attribute("vidalId")?.Value,
                    BestDocType = entry.Element(vidal + "bestDocType")?.Attribute("name")?.Value,
                    SafetyAlert = bool.Parse(entry.Element(vidal + "safetyAlert")?.Value ?? "false"),
                    ActivePrinciples = entry.Element(vidal + "activePrinciples")?.Value,
                    Company = entry.Element(vidal + "company")?.Value,
                    Vmp = entry.Element(vidal + "vmp")?.Value
                }).ToList();

            return products;
        }

        // Método para extraer los VMPs de la respuesta XML
        public static List<IndicationVMP> ParseIndicationVmpsXml(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            return doc.Descendants(ns + "entry")
                .Select(entry => new IndicationVMP
                {
                    Title = entry.Element(ns + "title")?.Value,
                    VmpLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "VMP")?.Attribute("href")?.Value,
                    ProductsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "PRODUCTS")?.Attribute("href")?.Value,
                    AtcClassificationLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ATC_CLASSIFICATION")?.Attribute("href")?.Value,
                    MoleculesLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "MOLECULES")?.Attribute("href")?.Value,
                    UnitsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "UNITS")?.Attribute("href")?.Value,
                    ContraindicationsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "CONTRAINDICATION")?.Attribute("href")?.Value,
                    PhysicoChemicalInteractionsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "PHYSICO_CHEMICAL_INTERACTIONS")?.Attribute("href")?.Value,
                    RoutesLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ROUTES")?.Attribute("href")?.Value,
                    IndicatorsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "INDICATORS")?.Attribute("href")?.Value,
                    IndicationsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "INDICATIONS")?.Attribute("href")?.Value,
                    SideEffectsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "SIDE_EFFECTS")?.Attribute("href")?.Value,
                    AldsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ALDS")?.Attribute("href")?.Value,
                    UcdvsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "UCDVS")?.Attribute("href")?.Value,
                    UcdsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "UCDS")?.Attribute("href")?.Value,
                    PrescribablesLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "PRESCRIBABLES")?.Attribute("href")?.Value,
                    AllergiesLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "ALLERGIES")?.Attribute("href")?.Value,
                    OptDocumentsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "OPT_DOCUMENT")?.Attribute("href")?.Value,
                    DocumentsLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "DOCUMENTS")?.Attribute("href")?.Value,
                    VtmLink = entry.Elements(ns + "link").FirstOrDefault(e => e.Attribute("title")?.Value == "VTM")?.Attribute("href")?.Value,
                    Id = entry.Element(vidal + "id")?.Value,
                    Name = entry.Element(vidal + "name")?.Value,
                    Summary = entry.Element(ns + "summary")?.Value,
                    ActivePrinciples = entry.Element(vidal + "activePrinciples")?.Value,
                    Route = entry.Element(vidal + "route")?.Value,
                    GalenicForm = entry.Element(vidal + "galenicForm")?.Value,
                    RegulatoryGenericPrescription = bool.Parse(entry.Element(vidal + "regulatoryGenericPrescription")?.Value ?? "false")
                }).ToList();
        }

        // Método para obtener valores de OpenSearch para paginación
        public static T GetOpenSearchValue<T>(this string xmlContent, string elementName)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace opensearch = "http://a9.com/-/spec/opensearch/1.1/";

            var element = doc.Descendants(opensearch + elementName).FirstOrDefault();
            if (element != null && !string.IsNullOrWhiteSpace(element.Value))
            {
                return (T)Convert.ChangeType(element.Value, typeof(T));
            }

            return default;
        }

    }
}
