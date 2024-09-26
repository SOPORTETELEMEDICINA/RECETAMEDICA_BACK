using RMD.Models.Vidal.ByUnit;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByUnit
{
    public static class XmlExtensionsByUnit
    {
        public static Unit ParseUnitXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atomNamespace = "http://www.w3.org/2005/Atom";

            var entryElement = document.Root.Element(atomNamespace + "entry");

            if (entryElement == null)
            {
                return new Unit(); // Retorna un objeto Unit vacío si no se encuentra el elemento entry
            }

            return new Unit
            {
                Id = (int?)entryElement.Element(vidalNamespace + "unitId") ?? 0,
                Name = (string)entryElement.Element(vidalNamespace + "name") ?? string.Empty,
                SingularName = (string)entryElement.Element(vidalNamespace + "singularName") ?? string.Empty,
                ParentConversionRate = (string)entryElement.Element(vidalNamespace + "parentConversionRate") ?? string.Empty
            };
        }

        public static List<Units> ParseUnitsXml(this string xmlContent)
        {
            XDocument document = XDocument.Parse(xmlContent);
            XNamespace vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atomNamespace = "http://www.w3.org/2005/Atom";

            var unitEntries = document.Descendants(atomNamespace + "entry")
                .Select(entry => new Units
                {
                    Title = (string)entry.Element(atomNamespace + "title") ?? string.Empty,
                    Id = (string)entry.Element(atomNamespace + "id") ?? string.Empty,
                    Updated = DateTime.TryParse((string)entry.Element(atomNamespace + "updated"), out var updatedDate) ? updatedDate : DateTime.MinValue,
                    UnitId = int.TryParse((string)entry.Element(vidalNamespace + "unitId"), out var unitId) ? unitId : 0,
                    Name = (string)entry.Element(vidalNamespace + "name") ?? string.Empty,
                    SingularName = (string)entry.Element(vidalNamespace + "singularName") ?? string.Empty,
                    ParentConversionRate = (string)entry.Element(vidalNamespace + "parentConversionRate") ?? string.Empty
                })
                .ToList();

            return unitEntries.Count > 0 ? unitEntries : new List<Units>(); // Si no hay unidades, retorna una lista vacía
        }
    }
}
