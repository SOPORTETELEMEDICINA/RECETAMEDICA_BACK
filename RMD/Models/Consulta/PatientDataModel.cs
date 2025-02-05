namespace RMD.Models.Consulta
{
    public class PatientDataModel
    {
        public decimal Peso { get; set; }  // Peso del paciente
        public decimal Talla { get; set; }  // Talla del paciente
        public bool Embarazo { get; set; }  // Embarazo
        public int? SemanasAmenorrea { get; set; }  // Semanas de amenorrea
        public bool Lactancia { get; set; }  // Lactancia
        public decimal? Creatinina { get; set; }  // Creatinina
        public List<int> Alergias { get; set; } = new();  // Alergias
        public List<int> Moleculas { get; set; } = new();  // Moléculas
        public List<int> Patologias { get; set; } = new();  // Patologías
    }
}
