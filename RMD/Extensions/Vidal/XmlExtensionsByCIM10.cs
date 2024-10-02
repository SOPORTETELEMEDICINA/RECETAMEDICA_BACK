
using RMD.Models.Shared;
using RMD.Models.Vidal;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal
{
    public static class XmlExtensionsByCIM10
    {
        public static List<CIM10> ParseCIM10sXml(this string xmlContent)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var cim10List = document.Descendants(atom + "entry")
                .Select(entry => new CIM10
                {
                    Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Code = entry.Element(vidal + "code")?.Value ?? string.Empty,
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
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

            return cim10List;
        }


        public static CIM10 ParseCIM10Xml(this string xmlContent)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);
            var entry = document.Descendants(atom + "entry").FirstOrDefault();

            if (entry == null)
            {
                return null;
            }

            return new CIM10
            {
                Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Code = entry.Element(vidal + "code")?.Value ?? string.Empty,
                Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                Updated = (DateTime?)entry.Element(atom + "updated"),
                RelatedLinks = entry.Elements(atom + "link")
                    .Select(link => new RelatedLink
                    {
                        Href = (string)link.Attribute("href"),
                        Relation = (string)link.Attribute("rel"),
                        Type = (string)link.Attribute("type"),
                        Title = (string)link.Attribute("title")
                    }).ToList()
            };
        }

        public static List<CIM10Child> ParseCIM10ChildrenXml(this string xmlContent)
        {
            XNamespace atom = "http://www.w3.org/2005/Atom";
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var document = XDocument.Parse(xmlContent);

            var children = document.Descendants(atom + "entry")
                .Select(entry => new CIM10Child
                {
                    Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Code = entry.Element(vidal + "code")?.Value ?? string.Empty,
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                    Summary = entry.Element(atom + "summary")?.Value ?? string.Empty,
                    RelatedLinks = entry.Elements(atom + "link")
                        .Select(link => new RelatedLink
                        {
                            Href = (string)link.Attribute("href"),
                            Relation = (string)link.Attribute("rel"),
                            Type = (string)link.Attribute("type"),
                            Title = (string)link.Attribute("title")
                        }).ToList()
                }).ToList();

            return children;
        }

    }
}
