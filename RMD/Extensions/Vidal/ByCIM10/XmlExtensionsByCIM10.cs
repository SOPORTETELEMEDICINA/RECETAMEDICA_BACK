
using RMD.Models.Vidal.ByCIM10;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByCIM10
{
    public static class XmlExtensionsByCIM10
    {
        public static List<CIM10> ParseCIM10sXml(this string xml)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xml);

                var cim10s = document.Descendants(atom + "entry").Select(entry => new CIM10
                {
                    Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Code = entry.Element(vidal + "code")?.Value ?? string.Empty,
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                }).ToList();

                return cim10s;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<CIM10>(); // Retornar una lista vacía en caso de error
            }
        }

        public static CIM10 ParseCIM10Xml(this string xml)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xml);
                var entry = document.Descendants(atom + "entry").FirstOrDefault();

                if (entry == null)
                    return null;

                var cim10 = new CIM10
                {
                    Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Code = entry.Element(vidal + "code")?.Value ?? string.Empty,
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty
                };

                return cim10;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return null; // Retornar null en caso de error
            }
        }

        public static List<CIM10Child> ParseCIM10ChildrenXml(this string xml)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xml);

                var children = new List<CIM10Child>();

                foreach (var entry in document.Descendants(atom + "entry"))
                {
                    var child = new CIM10Child
                    {
                        Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                        Code = entry.Element(vidal + "code")?.Value ?? string.Empty,
                        Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                        Title = entry.Element(atom + "title")?.Value ?? string.Empty,
                        Summary = entry.Element(atom + "summary")?.Value ?? string.Empty,
                        ALDSLink = entry.Elements(atom + "link").FirstOrDefault(x => x.Attribute("title")?.Value == "ALDS")?.Attribute("href")?.Value,
                        ChildrenLink = entry.Elements(atom + "link").FirstOrDefault(x => x.Attribute("title")?.Value == "CHILDREN")?.Attribute("href")?.Value,
                    };

                    children.Add(child);
                }

                return children;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<CIM10Child>(); // Retornar una lista vacía en caso de error
            }
        }

    }
}
