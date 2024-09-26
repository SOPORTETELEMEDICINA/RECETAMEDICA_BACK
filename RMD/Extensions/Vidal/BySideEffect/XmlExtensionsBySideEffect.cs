using RMD.Models.Vidal.BySideEffect;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.BySideEffect
{
    public static class XmlExtensionsBySideEffect
    {
        public static SideEffect ParseSideEffectXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var entry = document.Descendants("entry").FirstOrDefault();

            if (entry == null) return null;

            var sideEffect = new SideEffect
            {
                Id = (string)entry.Element("id"),
                Updated = DateTime.Parse((string)entry.Element("updated")),
                Title = (string)entry.Element("title"),
                VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),
                Name = (string)entry.Element(vidal + "name"),
                Apparatus = (string)entry.Element(vidal + "apparatus"),
                ApparatusVidalId = int.Parse(entry.Element(vidal + "apparatus")?.Attribute("vidalId")?.Value ?? "0")
            };

            return sideEffect;
        }
    }

}
