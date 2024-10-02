using RMD.Models.Shared;
using RMD.Models.Vidal;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal
{
    public static class XmlExtensionsByForeignProduct
    {
        public static List<ForeignProductEquivalent> ParseForeignProductEquivalentsXml(this string xmlContent)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var equivalents = document.Descendants(atom + "entry")
                .Select(entry => new ForeignProductEquivalent
                {
                    Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                    MarketStatus = entry.Element(vidal + "marketStatus")?.Value ?? string.Empty,
                    ActivePrinciples = entry.Element(vidal + "activePrinciples")?.Value ?? string.Empty,
                    Company = entry.Element(vidal + "company")?.Value ?? string.Empty,
                    Vmp = entry.Element(vidal + "vmp")?.Value ?? string.Empty,
                    GalenicForm = entry.Element(vidal + "galenicForm")?.Value ?? string.Empty,
                    RelatedLinks = entry.Elements(atom + "link")
                        .Select(link => new RelatedLink
                        {
                            Href = (string)link.Attribute("href"),
                            Relation = (string)link.Attribute("rel"),
                            Type = (string)link.Attribute("type"),
                            Title = (string)link.Attribute("title")
                        }).ToList()
                }).ToList();

            return equivalents;
        }
    }
}
