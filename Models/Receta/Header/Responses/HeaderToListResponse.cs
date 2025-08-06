using RMD.Models.Receta.Header.Base;
using RMD.Models.Vidal.Allergy;
using RMD.Models.Vidal.CIM10;
using RMD.Models.Vidal.Molecule;

namespace RMD.Models.Receta.Header.Responses
{
    public class HeaderToListResponse : Header_Base
    {
        public string NombresMedico { get; set; }
        public string PrimerApellidoMedico { get; set; }
        public string SegundoApellidoMedico { get; set; }
        public string Universidad { get; set; }
        public string CedulaGeneral { get; set; }
        public string Especialidad { get; set; }
        public string CedulaEspecialidad { get; set; }

        public string NombresPaciente { get; set; }
        public string PrimerApellidoPaciente { get; set; }
        public string SegundoApellidoPaciente { get; set; }
        public DateTime FechaNacimientoPaciente { get; set; }

        public List<RequestSearchAllergy> Alergias { get; set; }
        public List<RequestSearchMolecules> Molecules { get; set; }
        public List<RequestSearchCIM10> Patologias { get; set; }
    }
}