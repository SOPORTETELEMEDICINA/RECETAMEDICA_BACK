namespace RMD.Models.Consulta
{
    public class RecetaGetSQL
    {
        public Guid IdReceta { get; set; }
        public Guid IdMedico { get; set; }
        public string NombresMedico { get; set; }
        public string PrimerApellidoMedico { get; set; }
        public string SegundoApellidoMedico { get; set; }
        public string Universidad { get; set; }
        public string CedulaGeneral { get; set; }
        public string Especialidad { get; set; }
        public string CedulaEspecialidad { get; set; }
        public Guid IdPaciente { get; set; }
        public string NombresPaciente { get; set; }
        public string PrimerApellidoPaciente { get; set; }
        public string SegundoApellidoPaciente { get; set; }
        public DateTime FechaNacimientoPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public decimal PacTalla { get; set; }
        public bool PacEmbarazo { get; set; }
        public int PacSemAmenorrea { get; set; }
        public bool PacLactancia { get; set; }
        public decimal PacCreatinina { get; set; }
        public string Alergias { get; set; }
        public string Molecules { get; set; }
        public string Patologias { get; set; }
        public Guid IdSucursal { get; set; }
        public Guid IdGEMP { get; set; }
    }
}
