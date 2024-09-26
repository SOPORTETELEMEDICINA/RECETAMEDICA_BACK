using RMD.Models.Vidal.ByForeignProduct;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByForeignProduct
{
    public static class XmlExtensionsByForeignProduct
    {
        public static List<ForeignProductEquivalent> ParseForeignProductEquivalentsXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var entries = document.Root.Elements("{http://www.w3.org/2005/Atom}entry");

            var vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            var equivalents = new List<ForeignProductEquivalent>();

            foreach (var entry in entries)
            {
                equivalents.Add(new ForeignProductEquivalent
                {
                    Id = (int)entry.Element(XName.Get("id", vidalNamespace)),
                    Name = (string)entry.Element(XName.Get("name", vidalNamespace)),
                    MarketStatus = (string)entry.Element(XName.Get("marketStatus", vidalNamespace)),
                    ActivePrinciples = (string)entry.Element(XName.Get("activePrinciples", vidalNamespace)),
                    Company = (string)entry.Element(XName.Get("company", vidalNamespace)),
                    Vmp = (string)entry.Element(XName.Get("vmp", vidalNamespace)),
                    GalenicForm = (string)entry.Element(XName.Get("galenicForm", vidalNamespace))
                });
            }

            return equivalents;
        }
    }
}
