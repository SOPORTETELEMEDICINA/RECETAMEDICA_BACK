using Microsoft.VisualBasic;

namespace RMD.Models.Consulta
{

    public class XmlDataModel
    {
        public List<PatientEntry> PatientEntries { get; set; } = new List<PatientEntry>();
        public List<AlertEntry> AlertEntries { get; set; } = new List<AlertEntry>();
        public List<PrescriptionLineEntry> PrescriptionLineEntries { get; set; } = new List<PrescriptionLineEntry>();
        public List<OtherEntry> OtherEntries { get; set; } = new List<OtherEntry>();
    }

    public class PatientEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
    }

    public class AlertEntry
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class PrescriptionLineEntry
    {
        public string Id { get; set; }
        public string DrugName { get; set; }
        public string Dosage { get; set; }
    }

    public class OtherEntry
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }

}

