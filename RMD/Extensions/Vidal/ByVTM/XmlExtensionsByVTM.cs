
using RMD.Models.Consulta;
using RMD.Models.Vidal.ByVTM;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByVTM
{
    public static class XmlExtensionsByVTM
    {
        public static VTMEntry ParseVTMXml(this string xmlContent)
        {
            VTMEntry vTMEntry = new();
            var document = XDocument.Parse(xmlContent);
            var ns = "http://www.w3.org/2005/Atom";
            var vidalNs = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entry = document.Root?.Element(XName.Get("entry", ns));
            if (entry == null) return vTMEntry;

            return new VTMEntry
            {
                IdVtm = (int?)entry.Element(XName.Get("id", vidalNs)) ?? 0,
                Name = (string?)entry.Element(XName.Get("name", vidalNs)) ?? string.Empty,
                Summary = (string?)entry.Element(XName.Get("summary", ns)) ?? string.Empty
            };
        }

        public static List<VTMS> ParseVtmXml(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var vtmEntries = doc.Descendants("entry").Select(entry => new VTMS
            {
                Title = entry.Element("title")?.Value ?? string.Empty,
                Id = entry.Element("id")?.Value ?? string.Empty,
                Updated = DateTime.TryParse(entry.Element("updated")?.Value, out var updated) ? updated : DateTime.MinValue,
                VidalId = int.TryParse(entry.Element(vidal + "id")?.Value, out var vidalId) ? vidalId : 0,
                Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
            }).ToList();

            return vtmEntries;
        }

        public static List<VTMMolecule> ParseMoleculeXml(this string xmlContent)
        {
            XDocument doc = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var moleculeEntries = doc.Descendants("entry").Select(entry => new VTMMolecule
            {
                Title = entry.Element("title")?.Value ?? string.Empty,
                Id = entry.Element("id")?.Value ?? string.Empty,
                Updated = DateTime.TryParse(entry.Element("updated")?.Value, out var updated) ? updated : DateTime.MinValue,
                VidalId = int.TryParse(entry.Element(vidal + "id")?.Value, out var vidalId) ? vidalId : 0,
                Name = entry.Element(vidal + "name")?.Value ?? string.Empty,
                SafetyAlert = bool.TryParse(entry.Element(vidal + "safetyAlert")?.Value, out var safetyAlert) && safetyAlert,
                Homeopathy = bool.TryParse(entry.Element(vidal + "homeopathy")?.Value, out var homeopathy) && homeopathy,
                Role = entry.Element(vidal + "role")?.Attribute("name")?.Value ?? string.Empty
            }).ToList();

            return moleculeEntries;
        }

        public static List<VmpEntry> ParseVmpConsultaXml(this string xmlContent)
        {
            var vmpEntries = new List<VmpEntry>();
            var xmlDoc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            foreach (var entry in xmlDoc.Descendants(atomNs + "entry"))
            {
                var vmpEntry = new VmpEntry
                {
                    Title = entry.Element(atomNs + "title")?.Value ?? string.Empty,
                    Id = entry.Element(atomNs + "id")?.Value ?? string.Empty,
                    VidalId = entry.Element(ns + "id")?.Value ?? string.Empty,
                    Name = entry.Element(ns + "name")?.Value ?? string.Empty,
                    ActivePrinciples = entry.Element(ns + "activePrinciples")?.Value ?? string.Empty,
                    Route = entry.Element(ns + "route")?.Value ?? string.Empty,
                    GalenicForm = entry.Element(ns + "galenicForm")?.Value ?? string.Empty,
                    RegulatoryGenericPrescription = bool.TryParse(entry.Element(ns + "regulatoryGenericPrescription")?.Value, out var regulatoryGeneric) && regulatoryGeneric,
                    Summary = entry.Element(atomNs + "summary")?.Value ?? string.Empty
                };

                foreach (var link in entry.Elements(atomNs + "link"))
                {
                    vmpEntry.RelatedLinks.Add(new RelatedLink
                    {
                        Relation = link.Attribute("rel")?.Value ?? string.Empty,
                        Type = link.Attribute("type")?.Value ?? string.Empty,
                        Href = link.Attribute("href")?.Value ?? string.Empty,
                        Title = link.Attribute("title")?.Value ?? string.Empty
                    });
                }

                vmpEntries.Add(vmpEntry);
            }

            return vmpEntries;
        }



    }
}
