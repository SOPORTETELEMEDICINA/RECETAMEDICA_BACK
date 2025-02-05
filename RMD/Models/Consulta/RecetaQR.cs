namespace RMD.Models.Consulta
{
    public class RecetaQR
    {
        public Guid IdRecetaQR { get; set; } = Guid.NewGuid(); // Identificador único del QR
        public Guid IdReceta { get; set; } // Identificador de la receta
        public Guid IdPaciente { get; set; } // Identificador del paciente
        public string QRData { get; set; } = string.Empty; // Código QR en formato Base64
        public bool Estatus { get; set; } = true; // Estado del QR (Activo/Inactivo)
        public DateTime FechaCreacion { get; set; } = DateTime.Now; // Fecha de creación
        public DateTime FechaUltimaModificacion { get; set; } = DateTime.Now; // Fecha de última modificación
    }


}
