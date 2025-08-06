using System.Xml.Linq;

namespace RMD.Shared.Models.Consulta
{
    public class PrescriptionModel
    {
        public PatientModel Patient { get; set; } = new PatientModel();
        public List<PrescriptionLineModel> PrescriptionLines { get; set; } = [];
        public List<PrescriptionLineModel>? MedicamentoActivo { get; set; } = [];

        public string ParseToXml()
        {
            var medicamentoActivoSeguro = MedicamentoActivo?.Any() == true
                ? MedicamentoActivo
                : [];

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
                    //new XElement("prescription-lines",
                    //    allLines.Select(line =>

                    //        new XElement("prescription-line",
                    //            new XElement("drug", $"vidal://{line.DrugType}/{line.Drug}"),
                    //            new XElement("dose", line.Dose),
                    //            new XElement("unitId", line.UnitId),
                    //            new XElement("duration", line.Duration),
                    //            new XElement("durationType", line.DurationType),
                    //            new XElement("frequencyType", line.FrequencyType),
                    //            new XElement("route", line.Route),
                    //            !string.IsNullOrEmpty(line.Indication)
                    //                ? new XElement("indications",
                    //                    new XElement("indication", $"vidal://indication/{line.Indication}")
                    //                )
                    //                : null
                    //        )
                    //    )
                    //)
                    new XElement("prescription-lines",
                        allLines.Select(line =>
                        {
                            // 1. FrequencyType → THIS_DAY o PER_24_HOURS
                            var frequencyTypeXml = line.FrequencyType.Trim().ToUpper() switch
                            {
                                "DOSIS UNICA" => "THIS_DAY",
                                _ => "PER_24_HOURS"
                            };

                            // 2. Calcular dosis total en 24 horas
                            double dosisFinal = line.Dose; // default por si no se puede calcular

                            var tipo = line.FrequencyType.Trim().ToUpper();
                            var frecuencia = line.Frecuency;

                            if (frecuencia > 0)
                            {
                                dosisFinal = tipo switch
                                {
                                    "HORA(S)" => (24.0 / frecuencia) * line.Dose,
                                    "MINUTO(S)" => (24.0 * 60 / frecuencia) * line.Dose,
                                    "DIA(S)" => (1.0 / frecuencia) * line.Dose,
                                    "SEMANA(S)" => (1.0 / (frecuencia * 7)) * line.Dose,
                                    "MES(ES)" => (1.0 / (frecuencia * 30)) * line.Dose,
                                    _ => line.Dose
                                };
                            }

                            return new XElement("prescription-line",
                                new XElement("drug", $"vidal://{line.DrugType}/{line.Drug}"),
                                new XElement("dose", Math.Round(dosisFinal, 2)),
                                new XElement("unitId", line.UnitId),
                                new XElement("duration", line.Duration),
                                new XElement("durationType", line.DurationType),
                                new XElement("frequencyType", frequencyTypeXml),
                                new XElement("route", line.Route),
                                !string.IsNullOrEmpty(line.Indication)
                                    ? new XElement("indications",
                                        new XElement("indication", $"vidal://indication/{line.Indication}")
                                    )
                                    : null
                            );
                        })
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
