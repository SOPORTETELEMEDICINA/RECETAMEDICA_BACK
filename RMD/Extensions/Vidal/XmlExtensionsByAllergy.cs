using RMD.Models.Shared;
using RMD.Models.Vidal;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal
{
    public static class XmlExtensionsByAllergy
    {
        public static List<Allergy> ParseAllergyXmlToList(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var document = XDocument.Parse(xmlContent);

            var allergies = document.Descendants(atom + "entry")
                .Select(entry => new Allergy
                {
                    Id = int.Parse((string)entry.Element(vidal + "id")),
                    Name = (string)entry.Element(vidal + "name"),
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
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

            return allergies;
        }


        public static Allergy ParseAllergyXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var document = XDocument.Parse(xmlContent);
            var entry = document.Descendants(atom + "entry").FirstOrDefault();

            if (entry == null)
            {
                return null;
            }

            return new Allergy
            {
                Id = int.Parse((string)entry.Element(vidal + "id")),
                Name = (string)entry.Element(vidal + "name"),
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
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

        public static List<AllergyMolecule> ParseAllergyMoleculesXml(this string xmlContent)
        {
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var document = XDocument.Parse(xmlContent);

            var molecules = document.Descendants(atom + "entry")
                .Select(entry => new AllergyMolecule
                {
                    Title = (string)entry.Element(atom + "title"),
                    Id = (string)entry.Element(atom + "id"),
                    Name = (string)entry.Element(vidal + "name"),
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    SafetyAlert = bool.Parse(entry.Element(vidal + "safetyAlert")?.Value ?? "false"),
                    Homeopathy = bool.Parse(entry.Element(vidal + "homeopathy")?.Value ?? "false"),
                    Role = (string)entry.Element(vidal + "role")?.Attribute("name")
                }).ToList();

            return molecules;
        }
      
        public static List<Allergy> ParseAllergiesXml(this string xmlContent)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xmlContent);

                var allergies = document.Descendants(atom + "entry")
                    .Select(entry => new Allergy
                    {                      
                        Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                        Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                    }).ToList();

                return allergies;
            }
            catch (Exception ex)
            {
                // Mejorar la información de error para facilitar la depuración
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return new List<Allergy>(); // Retornar una lista vacía en caso de error
            }
        }
    }
}
