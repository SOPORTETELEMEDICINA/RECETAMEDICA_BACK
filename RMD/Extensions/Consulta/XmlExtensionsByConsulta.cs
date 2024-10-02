using RMD.Models.Consulta;
using RMD.Models.Shared;
using System.Xml.Linq;

namespace RMD.Extensions.Consulta
{
    public static class XmlExtensionsByConsulta
    {
        public static List<Cim10Entry> ParseCim10Xml(this string xmlContent)
        {
            var cim10Entries = new List<Cim10Entry>();
            var xmlDoc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://api.vidal.net/-/spec/vidal-api/1.0/";
            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            foreach (var entry in xmlDoc.Descendants(atomNs + "entry"))
            {
                var cim10Entry = new Cim10Entry
                {
                    Title = entry.Element(atomNs + "title")?.Value ?? string.Empty,
                    Id = entry.Element(atomNs + "id")?.Value ?? string.Empty,
                    VidalId = entry.Element(ns + "id")?.Value ?? string.Empty,
                    Name = entry.Element(ns + "name")?.Value ?? string.Empty,
                    Code = entry.Element(ns + "code")?.Value ?? string.Empty,
                    Summary = entry.Element(atomNs + "summary")?.Value ?? string.Empty,
                    Updated = DateTime.TryParse(entry.Element(atomNs + "updated")?.Value, out DateTime updated) ? updated : DateTime.MinValue
                };

                foreach (var link in entry.Elements(atomNs + "link"))
                {
                    cim10Entry.RelatedLinks.Add(new RelatedLink
                    {
                        Relation = link.Attribute("rel")?.Value ?? string.Empty,
                        Type = link.Attribute("type")?.Value ?? string.Empty,
                        Href = link.Attribute("href")?.Value ?? string.Empty,
                        Title = link.Attribute("title")?.Value ?? string.Empty
                    });
                }

                cim10Entries.Add(cim10Entry);
            }

            return cim10Entries;
        }

      

        public static string ParseToXml(this PrescriptionModel model)
        {
            var patientElement = new XElement("patient",
                new XElement("gender", model.Patient.Gender),
                new XElement("dateOfBirth", model.Patient.DateOfBirth.ToString("yyyy-MM-ddTHH:mm:ssK")),
                new XElement("weight", model.Patient.Weight),
                new XElement("height", model.Patient.Height),
                new XElement("breastFeeding", model.Patient.BreastFeeding),
                new XElement("pregnancy", model.Patient.Pregnancy),
                new XElement("weeksOfAmenorrhea", model.Patient.WeeksOfAmenorrhea ?? 0),
                new XElement("creatin", model.Patient.Creatin),
                new XElement("molecules",
                    model.Patient.Molecules.Select(m => new XElement("molecule", $"vidal://molecule/{m.Trim().Replace("vidal://molecule/", "")}"))
                ),
                new XElement("allergies",
                    model.Patient.Allergies.Select(a => new XElement("allergy", $"vidal://allergy/{a.Trim().Replace("vidal://allergy/", "")}"))
                ),
                new XElement("pathologies",
                    model.Patient.Pathologies.Select(p => new XElement("pathology", $"vidal://cim10/{p.Trim().Replace("vidal://cim10/", "")}"))
                )
            );

            var prescriptionLinesElement = new XElement("prescription-lines",
                model.PrescriptionLines.Select(line => new XElement("prescription-line",
                    new XElement("drug", line.Drug),
                    new XElement("dose", line.Dose),
                    new XElement("unitId", line.UnitId),
                    new XElement("frequencyType", line.FrequencyType),
                    new XElement("duration", line.Duration),
                    new XElement("durationType", line.DurationType)
                ))
            );

            var prescriptionElement = new XElement("prescription", patientElement, prescriptionLinesElement);

            var xmlDocument = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                prescriptionElement
            );

            return xmlDocument.ToString();
        }


        public static XmlDataModel ConvertXmlToModel(this string xml)
        {
            var xmlDataModel = new XmlDataModel();
            var doc = XDocument.Parse(xml);

            var entries = doc.Descendants("entry");

            foreach (var entry in entries)
            {
                var category = entry.Descendants(XName.Get("categories", "vidal"))?.FirstOrDefault()?.Value;

                switch (category)
                {
                    case "PATIENT":
                        xmlDataModel.PatientEntries.Add(ParsePatientEntry(entry));
                        break;

                    case "ALERT":
                        xmlDataModel.AlertEntries.Add(ParseAlertEntry(entry));
                        break;

                    case "PRESCRIPTION_LINE":
                        xmlDataModel.PrescriptionLineEntries.Add(ParsePrescriptionLineEntry(entry));
                        break;

                    default:
                        xmlDataModel.OtherEntries.Add(ParseOtherEntry(entry));
                        break;
                }
            }

            return xmlDataModel;
        }

        private static PatientEntry ParsePatientEntry(XElement entry)
        {
            return new PatientEntry
            {
                Id = entry.Element(XName.Get("id", "vidal"))?.Value,
                Name = entry.Element(XName.Get("name", "vidal"))?.Value,
                Age = entry.Element(XName.Get("age", "vidal"))?.Value
            };
        }

        private static AlertEntry ParseAlertEntry(XElement entry)
        {
            return new AlertEntry
            {
                Id = entry.Element(XName.Get("id", "vidal"))?.Value,
                Type = entry.Element(XName.Get("type", "vidal"))?.Value,
                Message = entry.Element(XName.Get("message", "vidal"))?.Value
            };
        }

        private static PrescriptionLineEntry ParsePrescriptionLineEntry(XElement entry)
        {
            return new PrescriptionLineEntry
            {
                Id = entry.Element(XName.Get("id", "vidal"))?.Value,
                DrugName = entry.Element(XName.Get("drugName", "vidal"))?.Value,
                Dosage = entry.Element(XName.Get("dosage", "vidal"))?.Value
            };
        }

        private static OtherEntry ParseOtherEntry(XElement entry)
        {
            return new OtherEntry
            {
                Id = entry.Element(XName.Get("id", "vidal"))?.Value,
                Category = entry.Element(XName.Get("categories", "vidal"))?.Value,
                Description = entry.Element(XName.Get("description", "vidal"))?.Value
            };
        }
    }
}
