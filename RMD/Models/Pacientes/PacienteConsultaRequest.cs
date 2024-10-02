using RMD.Models.Vidal.CargaCatalogos;

namespace RMD.Models.Pacientes
{
    public class PacienteConsultaRequest
    {
        public Guid IdPaciente { get; set; }
        public Guid? IdGEMP { get; set; }
        public Guid? IdSucursal { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string SegundoApellido { get; set; } = string.Empty;
        public string FechaNacimiento { get; set; }
        public int? IdEntidadNacimiento { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string Movil { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;

        public List<AllergyModel> Alergias { get; set; } = new List<AllergyModel>();
        public List<MoleculeModel> Molecules { get; set; } = new List<MoleculeModel>();
        public List<CIM10Model> Patologias { get; set; } = new List<CIM10Model>();
    }

}
