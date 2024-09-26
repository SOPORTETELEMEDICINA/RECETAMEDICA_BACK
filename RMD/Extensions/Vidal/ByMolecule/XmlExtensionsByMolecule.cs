using RMD.Models.Consulta;
using RMD.Models.Vidal.ByMolecule;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByMolecule
{
    public static class XmlExtensionsByMolecule
    {

        public static List<Molecule> ParseMoleculesXml(this string xml)
        {
            try
            {
                XNamespace atom = "http://www.w3.org/2005/Atom";
                XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

                var document = XDocument.Parse(xml);

                var molecules = document.Descendants(atom + "entry").Select(entry => new Molecule
                {
                    Id = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                    Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                    SafetyAlert = bool.TryParse(entry.Element(vidal + "safetyAlert")?.Value, out var safetyAlert) && safetyAlert,
                    Homeopathy = bool.TryParse(entry.Element(vidal + "homeopathy")?.Value, out var homeopathy) && homeopathy,
                    Role = entry.Element(vidal + "role")?.Attribute("name")?.Value ?? string.Empty,
                }).ToList();

                return molecules;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al analizar el XML: {ex.Message}");
                return []; // Retornar una lista vacía en caso de error
            }
        }

        public static Molecule ParseMoleculesEntrysXml(this string xml)
        {
            var document = XDocument.Parse(xml);
            var entryElement = document.Root.Element("{http://www.w3.org/2005/Atom}entry");

            if (entryElement == null)
            {
                return null;
            }

            var vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            return new Molecule
            {
                Id = (int)entryElement.Element(XName.Get("id", vidalNamespace)),
                Name = (string)entryElement.Element(XName.Get("name", vidalNamespace)),
                SafetyAlert = (bool)entryElement.Element(XName.Get("safetyAlert", vidalNamespace)),
                Homeopathy = (bool)entryElement.Element(XName.Get("homeopathy", vidalNamespace)),
                Role = (string)entryElement.Element(XName.Get("role", vidalNamespace)).Attribute("name")
            };
        }

        public static List<MoleculeEntry> ParseMoleculeConsultaXml(this string xmlContent)
        {
            var moleculeEntries = new List<MoleculeEntry>();
            var xmlDoc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            foreach (var entry in xmlDoc.Descendants(atomNs + "entry"))
            {
                var moleculeEntry = new MoleculeEntry
                {
                    Title = entry.Element(atomNs + "title")?.Value,
                    Id = entry.Element(atomNs + "id")?.Value,
                    VidalId = entry.Element(ns + "id")?.Value,
                    Name = entry.Element(ns + "name")?.Value,
                    SafetyAlert = bool.Parse(entry.Element(ns + "safetyAlert")?.Value ?? "false"),
                    Homeopathy = bool.Parse(entry.Element(ns + "homeopathy")?.Value ?? "false"),
                    Role = entry.Element(ns + "role")?.Attribute("name")?.Value,
                    FullName = entry.Element(ns + "fullName")?.Value
                };

                foreach (var link in entry.Elements(atomNs + "link"))
                {
                    moleculeEntry.RelatedLinks.Add(new RelatedLink
                    {
                        Relation = link.Attribute("rel")?.Value,
                        Type = link.Attribute("type")?.Value,
                        Href = link.Attribute("href")?.Value,
                        Title = link.Attribute("title")?.Value
                    });
                }

                moleculeEntries.Add(moleculeEntry);
            }

            return moleculeEntries;
        }
    }
}
