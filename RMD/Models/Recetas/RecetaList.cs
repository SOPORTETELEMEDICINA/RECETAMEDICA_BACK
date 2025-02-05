namespace RMD.Models.Recetas
{
    public class RecetaList
    {
        public Guid IdReceta { get; set; }
        public Guid IdMedico { get; set; }
        public string NombreMedico { get; set; }
        public Guid IdPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public Guid IdSucursal { get; set; }
        public Guid IdGEMP { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
