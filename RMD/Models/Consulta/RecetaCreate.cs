namespace RMD.Models.Consulta
{
    public class RecetaCreate
    {
        public Guid IdReceta { get; set; }  // ID único de la receta
        public Guid IdMedico { get; set; }  // ID del médico
        public Guid IdPaciente { get; set; }  // ID del paciente
        public decimal PacPeso { get; set; }  // Peso del paciente
        public decimal PacTalla { get; set; }  // Talla del paciente
        public bool PacEmbarazo { get; set; }  // Embarazo
        public int? PacSemAmenorrea { get; set; }  // Semanas de amenorrea (opcional)
        public bool PacLactancia { get; set; }  // Lactancia
        public decimal? PacCreatinina { get; set; }  // Creatinina (opcional)
        public string Alergias { get; set; } = string.Empty;  // Alergias (como string delimitado por comas)
        public string Molecules { get; set; } = string.Empty;  // Moléculas (como string delimitado por comas)
        public string Patologias { get; set; } = string.Empty;  // Patologías (como string delimitado por comas)
        public Guid IdSucursal { get; set; }  // ID de la sucursal
        public Guid IdGEMP { get; set; }  // ID del GEMP
        public DateTime FechaCreacion { get; set; }  // Fecha de creación
        public DateTime FechaUltimaModificacion { get; set; }  // Fecha de última modificación
    }
}
