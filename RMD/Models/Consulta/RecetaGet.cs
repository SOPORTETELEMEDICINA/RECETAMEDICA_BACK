namespace RMD.Models.Consulta
{
    public class RecetaGet
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
        public List <RequestSearchAllergy> Alergias { get; set; }
        public List <RequestSearchMolecules> Molecules { get; set; }
        public List<ListIM10> Patologias { get; set; }
        public Guid IdSucursal { get; set; }
        public Guid IdGEMP { get; set; }
    }

}
