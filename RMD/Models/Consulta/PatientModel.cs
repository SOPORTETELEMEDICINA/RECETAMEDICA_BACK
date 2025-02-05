namespace RMD.Models.Consulta
{
    public class PatientModel
    {
        public Guid IdPaciente { get; set; }
        public string Gender { get; set; } = string.Empty; // Género
        public DateTime DateOfBirth { get; set; } // Fecha de nacimiento
        public float Weight { get; set; } // Peso
        public int Height { get; set; } // Altura

        // Lactancia como valor opcional (nullable)
        public string? BreastFeeding { get; set; }

        // Amenorrea como valor opcional (nullable)
        public int? WeeksOfAmenorrhea { get; set; }
        public bool Pregnancy { get; set; } // Embarazo
        public float Creatin { get; set; } // Creatinina

        public List<string> Molecules { get; set; } = []; // Moléculas
        public List<string> Allergies { get; set; } = []; // Alergias
        public List<string> Pathologies { get; set; } = []; // Patologías
    }
}
