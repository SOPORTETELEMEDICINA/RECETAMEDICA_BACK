using RMD.Models.Vidal.ByIndicationGroup;
using System.Xml.Linq;

namespace RMD.Extensions.Vidal.ByIndicationGroup
{
    public static class XmlExtensionsByIndicationGroup
    {
        public static IndicationGroup ParseIndicationGroupXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var entry = document.Root.Element("{http://www.w3.org/2005/Atom}entry");

            var vidalNamespace = "http://api.vidal.net/-/spec/vidal-api/1.0/";

            return new IndicationGroup
            {
                Id = (int)entry.Element(XName.Get("id", vidalNamespace)),
                Name = (string)entry.Element(XName.Get("name", vidalNamespace))
            };
        }

        public static List<IndicationGroupProduct> ParseIndicationGroupProductsXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var products = document.Descendants(atom + "entry")  // Usar correctamente el namespace 'atom'
                .Select(entry => new IndicationGroupProduct
                {
                    Id = (string)entry.Element(atom + "id"),  // Elementos bajo el namespace 'atom'
                    Updated = DateTime.Parse((string)entry.Element(atom + "updated")),
                    Summary = (string)entry.Element(atom + "summary"),
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"),  // Elementos bajo el namespace 'vidal'
                    Name = (string)entry.Element(vidal + "name"),
                    ItemType = (string)entry.Element(vidal + "itemType")?.Attribute("name"),
                    MarketStatus = (string)entry.Element(vidal + "marketStatus")?.Attribute("name"),
                    HasPublishedDoc = (bool?)entry.Element(vidal + "hasPublishedDoc") ?? false,
                    WithoutPrescription = (bool?)entry.Element(vidal + "withoutPrescription") ?? false,
                    AmmTypeId = int.Parse((string)entry.Element(vidal + "ammType")?.Attribute("vidalId") ?? "0"),
                    BestDocType = (string)entry.Element(vidal + "bestDocType")?.Attribute("name"),
                    SafetyAlert = (bool?)entry.Element(vidal + "safetyAlert") ?? false,
                    ActivePrinciples = (string)entry.Element(vidal + "activePrinciples"),
                    Company = (string)entry.Element(vidal + "company"),
                    Vmp = (string)entry.Element(vidal + "vmp"),
                    GalenicForm = (string)entry.Element(vidal + "galenicForm")
                }).ToList();

            return products;
        }

        public static List<CIM10> ParseCIM10EntriesXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var cim10Entries = document.Descendants(atom + "entry")  // Usamos el namespace atom para "entry"
                .Select(entry => new CIM10
                {
                    Id = (string)entry.Element(atom + "id"),  // Elemento bajo el namespace atom
                    Updated = DateTime.Parse((string)entry.Element(atom + "updated")),
                    Name = (string)entry.Element(vidal + "name"),  // Elementos bajo el namespace vidal
                    Code = (string)entry.Element(vidal + "code"),
                    Rank = int.Parse((string)entry.Element(vidal + "rank") ?? "0")
                }).ToList();

            return cim10Entries;
        }


        public static List<VMP> ParseVMPsXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var vmps = document.Descendants(atom + "entry") // Utilizando el espacio de nombres "atom"
                .Select(entry => new VMP
                {
                    Id = (string)entry.Element(atom + "id"), // El campo "id" pertenece al espacio de nombres Atom
                    Updated = DateTime.Parse((string)entry.Element(atom + "updated")), // "updated" también pertenece a Atom
                    Title = (string)entry.Element(atom + "title"), // "title" pertenece a Atom
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"), // "id" del espacio de nombres Vidal
                    Name = (string)entry.Element(vidal + "name"), // "name" pertenece al espacio de nombres Vidal
                    ActivePrinciples = (string)entry.Element(vidal + "activePrinciples"), // "activePrinciples" pertenece a Vidal
                    Route = (string)entry.Element(vidal + "route"), // "route" pertenece a Vidal
                    GalenicForm = (string)entry.Element(vidal + "galenicForm"), // "galenicForm" pertenece a Vidal
                    RegulatoryGenericPrescription = bool.Parse(entry.Element(vidal + "regulatoryGenericPrescription")?.Value ?? "false")
                }).ToList();

            return vmps;
        }


        public static List<Indication> ParseIndicationsXml(this string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            XNamespace vidal = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var indications = document.Descendants(atom + "entry") // Usando el espacio de nombres Atom
                .Select(entry => new Indication
                {
                    Id = (string)entry.Element(atom + "id"), // Elemento "id" en el espacio de nombres Atom
                    Updated = DateTime.Parse((string)entry.Element(atom + "updated")), // Elemento "updated" en el espacio de nombres Atom
                    Title = (string)entry.Element(atom + "title"), // Elemento "title" en el espacio de nombres Atom
                    VidalId = int.Parse(entry.Element(vidal + "id")?.Value ?? "0"), // Elemento "id" en el espacio de nombres Vidal
                    Name = (string)entry.Element(vidal + "name"), // Elemento "name" en el espacio de nombres Vidal
                    Type = (string)entry.Element(vidal + "type")?.Attribute("name") // Atributo "name" del elemento "type" en el espacio de nombres Vidal
                }).ToList();

            return indications;
        }


    }
}
