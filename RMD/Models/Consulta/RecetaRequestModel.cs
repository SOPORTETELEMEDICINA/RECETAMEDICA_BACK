namespace RMD.Models.Consulta
{
    public class RecetaRequestModel
    {
        public Guid IdMedico { get; set; }  // ID del médico
        public Guid IdPaciente { get; set; }  // ID del paciente
        public PatientDataModel Paciente { get; set; } = new();
        public List<DetalleRecetaRequestModel> PrescriptionLines { get; set; } = new();
        public List<int>? PatologiasCronicas { get; set; } = new();  // Patologías
        public List<int>? AlergiasCronicas { get; set; } = new();  // Alergias
        public List<int>? MoleculasCronicas { get; set; } = new();  // Moléculas
    }
}
