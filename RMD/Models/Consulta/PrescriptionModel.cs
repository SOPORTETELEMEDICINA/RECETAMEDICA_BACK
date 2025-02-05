using System.Text.RegularExpressions;
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
            var allLines = PrescriptionLines.Concat(MedicamentoActivo ?? new List<PrescriptionLineModel>())
              .Where(line => line.Drug != 0); // Filtrar donde Drug != 0

            var xdoc = new XDocument(
                new XElement("prescription",
                    new XElement("patient",
                        new XElement("gender", Patient.Gender), // Género
                        new XElement("dateOfBirth", Patient.DateOfBirth.ToString("yyyy-MM-ddTHH:mm:sszzz")), // FechaNacimiento
                        new XElement("weight", Patient.Weight), // Peso
                        new XElement("height", Patient.Height), // Altura

                        // Solo agregar lactancia y amenorrea si el género no es "M" o "Male"
                        !string.Equals(Patient.Gender, "M", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Patient.Gender, "Male", StringComparison.OrdinalIgnoreCase)
                            ? new XElement("breastFeeding", Patient.BreastFeeding) // Lactancia
                            : null,
                        new XElement("pregnancy", Patient.Pregnancy.ToString().ToLower()),
                        !string.Equals(Patient.Gender, "M", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Patient.Gender, "Male", StringComparison.OrdinalIgnoreCase)
                            ? new XElement("weeksOfAmenorrhea", Patient.WeeksOfAmenorrhea) // Amenorrea
                            : null,
                        new XElement("creatin", Patient.Creatin), // Creatinina

                        // Transformar molecules a formato vidal://molecule/xxx
                        new XElement("molecules",
                            Patient.Molecules.Select(m =>
                                new XElement("molecule", $"vidal://molecule/{m}")
                            )
                        ),

                        // Transformar allergies a formato vidal://allergy/xxx
                        new XElement("allergies",
                            Patient.Allergies.Select(a =>
                                new XElement("allergy", $"vidal://allergy/{a}")
                            )
                        ),

                        // Transformar pathologies a formato vidal://cim10/code/xxx
                        new XElement("pathologies",
                            Patient.Pathologies.Select(p =>
                                new XElement("pathology", $"vidal://cim10/code/{p}")
                            )
                        )
                    ),
                    new XElement("prescription-lines",
                        allLines.Select(line =>
                            new XElement("prescription-line",
                                new XElement("drug", $"vidal://{line.DrugType}/{line.Drug}"), // Tipo + ID del fármaco
                                new XElement("dose",
                                    int.TryParse(Regex.Match(line.FrequencyType, @"\\d+").Value, out int hours) && hours != 0
                                        ? (24 / hours) * line.Dose
                                        : line.Dose),
                                new XElement("unitId", line.UnitId),
                                new XElement("duration", line.Duration),
                                new XElement("durationType", line.DurationType),
                                new XElement("frequencyType",
                                     hours == 0
                                        ? "THIS_DAY"
                                        : "PER_24_HOURS"),
                                    new XElement("route", line.Route),

                                // Agregar <indications> si Indication no es nulo
                                !string.IsNullOrEmpty(line.Indication)
                                    ? new XElement("indications",
                                        new XElement("indication", $"vidal://indication/{line.Indication}")
                                      )
                                    : null
                            )
                        )
                    )
                )
            );

            return xdoc.ToString(SaveOptions.None);
        }
    }
}
