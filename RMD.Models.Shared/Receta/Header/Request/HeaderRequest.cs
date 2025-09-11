using RMD.Shared.Models.Receta.Header.Internos;

namespace RMD.Shared.Models.Receta.Header.Request
{
    public class HeaderRequest
    {
        public Guid IdMedico { get; set; }  // ID del médico
        public Guid IdPaciente { get; set; }  // ID del paciente
        public PatientDataRequest Paciente { get; set; } = new();
        public List<DetalleRecetaRequestModel> PrescriptionLines { get; set; } = new();
        public List<int>? PatologiasCronicas { get; set; } = new();  // Patologías
        public List<int>? AlergiasCronicas { get; set; } = new();  // Alergias
        public List<int>? MoleculasCronicas { get; set; } = new();  // Moléculas
        public string Folio { get; set; } = string.Empty;  // Folio (opcional)
    }
}
