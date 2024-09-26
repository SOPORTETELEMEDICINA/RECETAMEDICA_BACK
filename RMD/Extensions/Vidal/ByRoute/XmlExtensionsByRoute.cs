using RMD.Models.Vidal.ByRoute;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByRoute
{
    public static class XmlExtensionsByRoute
    {
        public static Routes ParseRouteXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entry = document.Descendants("entry").FirstOrDefault();

            if (entry == null) return null;

            var route = new Routes
            {
                Id = (string)entry.Element("id"),
                Updated = DateTime.Parse((string)entry.Element("updated")),
                Title = (string)entry.Element("title"),
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Name = (string)entry.Element(vidal + "name"),
                RouteId = int.Parse(entry.Element(vidal + "routeId")?.Value ?? "0"),
                Systemic = bool.Parse(entry.Element(vidal + "systemic")?.Value ?? "false"),
                Topical = bool.Parse(entry.Element(vidal + "topical")?.Value ?? "false"),
                ParentId = int.Parse(entry.Element(vidal + "parentId")?.Value ?? "0")
            };

            return route;
        }

        public static List<Routes> ParseRoutesXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var routes = document.Descendants("entry")
                .Select(entry => new Routes
                {
                    Id = (string)entry.Element("id"),
                    Updated = DateTime.Parse((string)entry.Element("updated")),
                    Title = (string)entry.Element("title"),
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Name = (string)entry.Element(vidal + "name"),
                    RouteId = int.Parse(entry.Element(vidal + "routeId")?.Value ?? "0"),
                    Systemic = bool.Parse(entry.Element(vidal + "systemic")?.Value ?? "false"),
                    Topical = bool.Parse(entry.Element(vidal + "topical")?.Value ?? "false"),
                    ParentId = int.Parse(entry.Element(vidal + "parentId")?.Value ?? "0")
                }).ToList();

            return routes;
        }
    }

}
