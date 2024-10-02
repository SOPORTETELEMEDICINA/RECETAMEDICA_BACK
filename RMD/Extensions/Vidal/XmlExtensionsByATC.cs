using RMD.Models.Vidal;
using System.Xml.Linq;
using RMD.Models.Shared;

namespace RMD.Extensions.Vidal
{
    public static class XmlExtensionsByATC
    {
        public static List<ATCClassification> ParseATCClassificationsXml(this string xmlContent)
        {
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var xdoc = XDocument.Parse(xmlContent);

            var classifications = xdoc.Descendants(XName.Get("entry", ns.NamespaceName))
                .Select(entry => new ATCClassification
                {
                    Id = int.Parse(entry.Element(vidalNs + "id")?.Value ?? "0"),
                    Name = entry.Element(vidalNs + "name")?.Value ?? string.Empty,
                    Code = entry.Element(vidalNs + "code")?.Value ?? string.Empty,
                    Updated = DateTime.TryParse(entry.Element(ns + "updated")?.Value, out DateTime updated) ? updated : DateTime.MinValue,
                    RelatedLinks = entry.Elements(ns + "link")
                        .Select(link => new RelatedLink
                        {
                            Href = (string)link.Attribute("href"),
                            Relation = (string)link.Attribute("rel"),
                            Type = (string)link.Attribute("type"),
                            Title = (string)link.Attribute("title")
                        }).ToList()
                }).ToList();

            return classifications;
        }

        public static List<ATCDetail> ParseProductsXml(this string xmlContent)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var products = document.Descendants(atom + "entry")
                .Select(entry => new ATCDetail
                {
                    Id = int.Parse(entry.Element(vidalNs + "id")?.Value ?? "0"),
                    Name = entry.Element(vidalNs + "name")?.Value ?? string.Empty,
                    Route = entry.Element(vidalNs + "route")?.Value ?? string.Empty,
                    ActivePrinciples = entry.Element(vidalNs + "activePrinciples")?.Value ?? string.Empty,
                    RelatedLinks = entry.Elements(atom + "link")
                        .Select(link => new RelatedLink
                        {
                            Href = (string)link.Attribute("href"),
                            Relation = (string)link.Attribute("rel"),
                            Type = (string)link.Attribute("type"),
                            Title = (string)link.Attribute("title")
                        }).ToList()
                }).ToList();

            return products;
        }

        public static List<ATCDetail> ParseATCVMPXml(this string xmlContent)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var atcDetails = document.Descendants(atom + "entry")
                .Select(entry => new ATCDetail
                {
                    Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                    Route = entry.Element(vidal + "route")?.Value ?? string.Empty,
                    ActivePrinciples = entry.Element(vidal + "activePrinciples")?.Value ?? string.Empty,
                    RegulatoryGenericPrescription = bool.Parse(entry.Element(vidal + "regulatoryGenericPrescription")?.Value ?? "false"),
                    Updated = (DateTime?)entry.Element(atom + "updated"),
                    RelatedLinks = entry.Elements(atom + "link")
                        .Select(link => new RelatedLink
                        {
                            Href = (string)link.Attribute("href"),
                            Relation = (string)link.Attribute("rel"),
                            Type = (string)link.Attribute("type"),
                            Title = (string)link.Attribute("title")
                        }).ToList()
                }).ToList();

            return atcDetails;
        }

        public static List<ATCClassification> ParseATCChildrenXml(string xmlContent)
        {
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var doc = XDocument.Parse(xmlContent);

            var entries = doc.Descendants(XName.Get("entry", ns.NamespaceName))
                .Select(entry => new ATCClassification
                {
                    Id = int.TryParse(entry.Element(vidal + "id")?.Value, out int idValue) ? idValue : 0,
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                    Code = entry.Element(vidal + "code")?.Value ?? string.Empty,
                    Updated = DateTime.TryParse(entry.Element(ns + "updated")?.Value, out DateTime updated) ? updated : DateTime.MinValue,
                    RelatedLinks = entry.Elements(ns + "link")
                        .Select(link => new RelatedLink
                        {
                            Href = (string)link.Attribute("href"),
                            Relation = (string)link.Attribute("rel"),
                            Type = (string)link.Attribute("type"),
                            Title = (string)link.Attribute("title")
                        }).ToList()
                }).ToList();

            return entries;
        }

        public static ATCDetail ParseATCClassificationByIdXml(this string xmlContent)
        {
            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var xdoc = XDocument.Parse(xmlContent);

            var entry = xdoc.Descendants(XName.Get("entry", ns.NamespaceName)).FirstOrDefault();

            if (entry == null)
            {
                return new ATCDetail(); // Devolver una instancia vacía en lugar de null
            }

            var classificationDetail = new ATCDetail
            {
                Id = int.TryParse(entry.Element(XName.Get("id", vidalNs.NamespaceName))?.Value, out int idValue) ? idValue : 0,
                Name = entry.Element(XName.Get("name", vidalNs.NamespaceName))?.Value ?? string.Empty,
                Updated = DateTime.TryParse(entry.Element(XName.Get("updated", ns.NamespaceName))?.Value, out DateTime updated) ? updated : DateTime.MinValue,
                RelatedLinks = entry.Elements(ns + "link")
                    .Select(link => new RelatedLink
                    {
                        Href = (string)link.Attribute("href"),
                        Relation = (string)link.Attribute("rel"),
                        Type = (string)link.Attribute("type"),
                        Title = (string)link.Attribute("title")
                    }).ToList()
            };

            return classificationDetail;
        }

    }
}
