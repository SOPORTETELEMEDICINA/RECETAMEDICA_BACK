using RMD.Models.Vidal.ByATC;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByATC
{
    public static class ATCXmlParser
    {
        public static List<ATCVMPEntry> ParseATCVMPXml(string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entries = doc.Descendants(XName.Get("entry", ns.NamespaceName))
                .Select(entry => new ATCVMPEntry
                {
                    Title = entry.Element(XName.Get("title", ns.NamespaceName))?.Value ?? string.Empty,
                    VMPLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "VMP")?.Attribute("href")?.Value ?? string.Empty,
                    ProductsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "PRODUCTS")?.Attribute("href")?.Value ?? string.Empty,
                    AtcClassificationLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "ATC_CLASSIFICATION")?.Attribute("href")?.Value ?? string.Empty,
                    MoleculesLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "MOLECULES")?.Attribute("href")?.Value ?? string.Empty,
                    UnitsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "UNITS")?.Attribute("href")?.Value ?? string.Empty,
                    ContraindicationsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "CONTRAINDICATION")?.Attribute("href")?.Value ?? string.Empty,
                    PhysicoChemicalInteractionsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "PHYSICO_CHEMICAL_INTERACTIONS")?.Attribute("href")?.Value ?? string.Empty,
                    RoutesLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "ROUTES")?.Attribute("href")?.Value ?? string.Empty,
                    IndicatorsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "INDICATORS")?.Attribute("href")?.Value ?? string.Empty,
                    IndicationsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "INDICATIONS")?.Attribute("href")?.Value ?? string.Empty,
                    SideEffectsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "SIDE_EFFECTS")?.Attribute("href")?.Value ?? string.Empty,
                    AldsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "ALDS")?.Attribute("href")?.Value ?? string.Empty,
                    UcdvsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "UCDVS")?.Attribute("href")?.Value ?? string.Empty,
                    UcdsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "UCDS")?.Attribute("href")?.Value ?? string.Empty,
                    PrescribablesLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "PRESCRIBABLES")?.Attribute("href")?.Value ?? string.Empty,
                    AllergiesLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "ALLERGIES")?.Attribute("href")?.Value ?? string.Empty,
                    OptDocumentsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "OPT_DOCUMENT")?.Attribute("href")?.Value ?? string.Empty,
                    DocumentsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "DOCUMENTS")?.Attribute("href")?.Value ?? string.Empty,
                    VtmLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "VTM")?.Attribute("href")?.Value ?? string.Empty,
                    Id = entry.Element(XName.Get("id", vidal.NamespaceName))?.Value ?? string.Empty,
                    Name = entry.Element(XName.Get("name", vidal.NamespaceName))?.Value ?? string.Empty,
                    ActivePrinciples = entry.Element(XName.Get("activePrinciples", vidal.NamespaceName))?.Value ?? string.Empty,
                    Route = entry.Element(XName.Get("route", vidal.NamespaceName))?.Value ?? string.Empty,
                    GalenicForm = entry.Element(XName.Get("galenicForm", vidal.NamespaceName))?.Value ?? string.Empty,
                    RegulatoryGenericPrescription = bool.Parse(entry.Element(XName.Get("regulatoryGenericPrescription", vidal.NamespaceName))?.Value ?? "false")
                }).ToList();

            return entries;
        }

        public static List<AtcProduct> ParseProductsXml(string xmlContent)
        {
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var products = document.Descendants(XName.Get("entry", ns.NamespaceName))
                .Select(entry => new AtcProduct
                {
                    Title = entry.Element(XName.Get("title", ns.NamespaceName))?.Value ?? string.Empty,
                    ProductLink = entry.Elements(XName.Get("link", ns.NamespaceName))
                        .FirstOrDefault(link => link.Attribute("title")?.Value == "PRODUCT")
                        ?.Attribute("href")?.Value ?? string.Empty,
                    MoleculesLink = entry.Elements(XName.Get("link", ns.NamespaceName))
                        .FirstOrDefault(link => link.Attribute("title")?.Value == "MOLECULES")
                        ?.Attribute("href")?.Value ?? string.Empty,
                    IndicationsLink = entry.Elements(XName.Get("link", ns.NamespaceName))
                        .FirstOrDefault(link => link.Attribute("title")?.Value == "INDICATIONS")
                        ?.Attribute("href")?.Value ?? string.Empty,
                    CompanyName = entry.Element(XName.Get("company", vidalNs.NamespaceName))?.Value ?? string.Empty,
                    ActivePrinciples = entry.Element(XName.Get("activePrinciples", vidalNs.NamespaceName))?.Value ?? string.Empty,
                    MarketStatus = entry.Element(XName.Get("marketStatus", vidalNs.NamespaceName))?.Attribute("name")?.Value ?? string.Empty
                })
                .ToList();

            return products;
        }

        public static List<ATCClassificationEntry> ParseATCChildrenXml(string xmlContent)
        {
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var doc = XDocument.Parse(xmlContent);

            var entries = doc.Descendants(XName.Get("entry", ns.NamespaceName))
                .Select(entry => new ATCClassificationEntry
                {
                    Title = entry.Element(XName.Get("title", ns.NamespaceName))?.Value ?? string.Empty,
                    ProductsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "PRODUCTS")?.Attribute("href")?.Value ?? string.Empty,
                    VmpsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "VMPS")?.Attribute("href")?.Value ?? string.Empty,
                    ChildrenLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "CHILDREN")?.Attribute("href")?.Value ?? string.Empty,
                    ParentLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "PARENT")?.Attribute("href")?.Value ?? string.Empty,
                    MoleculesLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(e => e.Attribute("title")?.Value == "MOLECULES")?.Attribute("href")?.Value ?? string.Empty,
                    Id = entry.Element(XName.Get("id", vidal.NamespaceName))?.Value ?? string.Empty,
                    Name = entry.Element(XName.Get("name", vidal.NamespaceName))?.Value ?? string.Empty,
                    Code = entry.Element(XName.Get("code", vidal.NamespaceName))?.Value ?? string.Empty
                }).ToList();

            return entries;
        }

        public static List<ATCClassification> ParseATCClassificationsXml(this string xmlContent)
        {
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var xdoc = XDocument.Parse(xmlContent);

            var classifications = xdoc.Descendants(XName.Get("entry", ns.NamespaceName))
                .Select(entry => new ATCClassification
                {
                    Id = entry.Element(XName.Get("id", ns.NamespaceName))?.Value ?? string.Empty,
                    VidalId = entry.Element(XName.Get("id", vidalNs.NamespaceName))?.Value ?? string.Empty,
                    Name = entry.Element(XName.Get("name", vidalNs.NamespaceName))?.Value ?? string.Empty,
                    Code = entry.Element(XName.Get("code", vidalNs.NamespaceName))?.Value ?? string.Empty,
                    ParentLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(l => l.Attribute("title")?.Value == "PARENT")?.Attribute("href")?.Value ?? string.Empty,
                    ChildrenLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(l => l.Attribute("title")?.Value == "CHILDREN")?.Attribute("href")?.Value ?? string.Empty,
                    Updated = DateTime.TryParse(entry.Element(XName.Get("updated", ns.NamespaceName))?.Value, out DateTime updated) ? updated : DateTime.MinValue,
                    Category = entry.Element(XName.Get("category", ns.NamespaceName))?.Attribute("term")?.Value ?? string.Empty,
                    Author = entry.Element(XName.Get("author", ns.NamespaceName))?.Element(XName.Get("name", ns.NamespaceName))?.Value ?? string.Empty
                }).ToList();

            return classifications;
        }

        public static ATCClassificationDetail ParseATCClassificationByIdXml(this string xmlContent)
        {
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var xdoc = XDocument.Parse(xmlContent);

            var entry = xdoc.Descendants(XName.Get("entry", ns.NamespaceName)).FirstOrDefault();

            if (entry == null)
            {
                return new ATCClassificationDetail(); // Devolver una instancia vacía en lugar de null
            }

            var classificationDetail = new ATCClassificationDetail
            {
                Id = entry.Element(XName.Get("id", vidalNs.NamespaceName))?.Value ?? string.Empty,
                VidalId = int.TryParse(entry.Element(XName.Get("id", vidalNs.NamespaceName))?.Value, out int vidalId) ? vidalId : 0,
                Name = entry.Element(XName.Get("name", vidalNs.NamespaceName))?.Value ?? string.Empty,
                Updated = DateTime.TryParse(entry.Element(XName.Get("updated", ns.NamespaceName))?.Value, out DateTime updated) ? updated : DateTime.MinValue,
                ProductsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(l => string.Equals((string?)l.Attribute("title"), "PRODUCTS", StringComparison.OrdinalIgnoreCase))?.Attribute("href")?.Value ?? string.Empty,
                ChildrenLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(l => string.Equals((string?)l.Attribute("title"), "CHILDREN", StringComparison.OrdinalIgnoreCase))?.Attribute("href")?.Value ?? string.Empty,
                MoleculesLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(l => string.Equals((string?)l.Attribute("title"), "MOLECULES", StringComparison.OrdinalIgnoreCase))?.Attribute("href")?.Value ?? string.Empty,
                VMPsLink = entry.Elements(XName.Get("link", ns.NamespaceName)).FirstOrDefault(l => string.Equals((string?)l.Attribute("title"), "VMPS", StringComparison.OrdinalIgnoreCase))?.Attribute("href")?.Value ?? string.Empty,
                Category = entry.Element(XName.Get("category", ns.NamespaceName))?.Attribute("term")?.Value ?? string.Empty,
                Author = entry.Element(XName.Get("author", ns.NamespaceName))?.Element(XName.Get("name", ns.NamespaceName))?.Value ?? string.Empty
            };

            return classificationDetail;
        }
    }
}
