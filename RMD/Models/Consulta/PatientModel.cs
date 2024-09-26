namespace RMD.Models.Consulta
{
    public class PatientModel
    {
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public float Weight { get; set; }
        public int Height { get; set; }
        public string BreastFeeding { get; set; } = string.Empty;
        public bool Pregnancy { get; set; }
        public int? WeeksOfAmenorrhea { get; set; }
        public float Creatin { get; set; }
        public List<string> Molecules { get; set; } = [];
        public List<string> Allergies { get; set; } = [];
        public List<string> Pathologies { get; set; } = [];
    }
}
