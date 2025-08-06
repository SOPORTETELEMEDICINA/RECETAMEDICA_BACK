using System.Xml.Linq;

namespace RMD.Models.Consulta
{
    public class PrescriptionModel
    {
        public PatientModel Patient { get; set; } = new PatientModel();
        public List<PrescriptionLineModel> PrescriptionLines { get; set; } = new();
        public List<PrescriptionLineModel>? MedicamentoActivo { get; set; } = new();

        public string ParseToXml()
        {
            var medicamentoActivoSeguro = MedicamentoActivo?.Any() == true
                ? MedicamentoActivo
                : new List<PrescriptionLineModel>();

            var allLines = PrescriptionLines
                .Concat(medicamentoActivoSeguro)
                .Where(line => line.Drug != 0); // Filtrar donde Drug != 0

            var xdoc = new XDocument(
                new XElement("prescription",
                    new XElement("patient",
                        new XElement("gender", Patient.Gender),
                        new XElement("dateOfBirth", Patient.DateOfBirth.ToString("yyyy-MM-ddTHH:mm:sszzz")),
                        new XElement("weight", Patient.Weight),
                        new XElement("height", Patient.Height),

                        !string.Equals(Patient.Gender, "M", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Patient.Gender, "Male", StringComparison.OrdinalIgnoreCase)
                            ? new XElement("breastFeeding", Patient.BreastFeeding)
                            : null,

                        new XElement("pregnancy", Patient.Pregnancy.ToString().ToLower()),

                        !string.Equals(Patient.Gender, "M", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Patient.Gender, "Male", StringComparison.OrdinalIgnoreCase)
                            ? new XElement("weeksOfAmenorrhea", Patient.WeeksOfAmenorrhea)
                            : null,

                        new XElement("creatin", Patient.Creatin),

                        new XElement("molecules",
                            Patient.Molecules.Select(m =>
                                new XElement("molecule", $"vidal://molecule/{m}")
                            )
                        ),

                        new XElement("allergies",
                            Patient.Allergies.Select(a =>
                                new XElement("allergy", $"vidal://allergy/{a}")
                            )
                        ),

                        new XElement("pathologies",
                            Patient.Pathologies.Select(p =>
                                new XElement("pathology", $"vidal://cim10/code/{p}")
                            )
                        )
                    ),
                    new XElement("prescription-lines",
                        allLines.Select(line =>
                            new XElement("prescription-line",
                                new XElement("drug", $"vidal://{line.DrugType}/{line.Drug}"),
                                new XElement("dose", line.Dose),
                                new XElement("unitId", line.UnitId),
                                new XElement("duration", line.Duration),
                                new XElement("durationType", line.DurationType),
                                new XElement("frequencyType", line.FrequencyType),
                                new XElement("route", line.Route),
                                !string.IsNullOrEmpty(line.Indication)
                                    ? new XElement("indications",
                                        new XElement("indication", $"vidal://indication/{line.Indication}")
                                    )
                                    : null
                            )
                        )
                    )

                //new XElement("prescription-lines",
                //    allLines.Select(line =>
                //    {
                //        int hours = int.TryParse(Regex.Match(line.FrequencyType, @"\\d+").Value, out int h) ? h : 0;

                //        return new XElement("prescription-line",
                //            new XElement("drug", $"vidal://{line.DrugType}/{line.Drug}"),
                //            new XElement("dose", hours != 0 ? (24 / hours) * line.Dose : line.Dose),
                //            new XElement("unitId", line.UnitId),
                //            new XElement("duration", line.Duration),
                //            new XElement("durationType", line.DurationType),
                //            new XElement("frequencyType", hours == 0 ? "THIS_DAY" : "168"),
                //            new XElement("route", line.Route),
                //            !string.IsNullOrEmpty(line.Indication)
                //                ? new XElement("indications",
                //                    new XElement("indication", $"vidal://indication/{line.Indication}")
                //                  )
                //                : null
                //        );
                //    })
                //)
                )
            );

            return xdoc.ToString(SaveOptions.None);
        }

    }
}
