using System.Xml.Linq;

namespace RMD.Models.Consulta
{
    public class PrescriptionModel
    {
        public PatientModel Patient { get; set; } = new PatientModel();
        public List<PrescriptionLineModel> PrescriptionLines { get; set; } = [];

        public string ParseToXml()
        {
            var xdoc = new XDocument(
                new XElement("prescription",
                    new XElement("patient",
                        new XElement("gender", Patient.Gender),
                        new XElement("dateOfBirth", Patient.DateOfBirth.ToString("yyyy-MM-ddTHH:mm:sszzz")),
                        new XElement("weight", Patient.Weight),
                        new XElement("height", Patient.Height),
                        new XElement("breastFeeding", Patient.BreastFeeding),
                        new XElement("pregnancy", Patient.Pregnancy.ToString().ToLower()),
                        new XElement("weeksOfAmenorrhea", Patient.WeeksOfAmenorrhea),
                        new XElement("creatin", Patient.Creatin),
                        new XElement("molecules",
                            Patient.Molecules.Select(m => new XElement("molecule", m))
                        ),
                        new XElement("allergies",
                            Patient.Allergies.Select(a => new XElement("allergy", a))
                        ),
                        new XElement("pathologies",
                            Patient.Pathologies.Select(p => new XElement("pathology", p))
                        )
                    ),
                    new XElement("prescription-lines",
                        PrescriptionLines.Select(line =>
                            new XElement("prescription-line",
                                new XElement("drug", line.Drug),
                                new XElement("dose", line.Dose),
                                new XElement("unitId", line.UnitId),
                                new XElement("frequencyType", line.FrequencyType),
                                new XElement("duration", line.Duration),
                                new XElement("durationType", line.DurationType)
                            )
                        )
                    )
                )
            );

            return xdoc.ToString(SaveOptions.None);
        }
    }
}
