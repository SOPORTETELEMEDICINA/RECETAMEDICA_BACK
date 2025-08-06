//using RMD.Models.Recetas.Header.Base;
//using RMD.Models.Vidal.Allergy;
//using RMD.Models.Vidal.CIM10;
//using RMD.Models.Vidal.Molecule;

//namespace RMD.Models.Recetas.Header.Internos
//{
//    public class Header_UpdatePacienteParsedRequest
//    {
//        public Guid IdReceta { get; set; }
//        public Guid IdMedico { get; set; }
//        public Guid IdPaciente { get; set; }
//        public string NombresPaciente { get; set; }
//        public string PrimerApellidoPaciente { get; set; }
//        public string SegundoApellidoPaciente { get; set; }
//        public int IdTipoIdentificacion { get; set; }
//        public string TipoIdentificacion { get; set; }
//        public string NumeroIdentificacion { get; set; }
//        public int EdadPaciente { get; set; }
//        public decimal PacPeso { get; set; }
//        public string Genero { get; set; }
//        public decimal PacTalla { get; set; }
//        public bool PacEmbarazo { get; set; }
//        public int PacSemAmenorrea { get; set; }
//        public bool PacLactancia { get; set; }
//        public decimal? PacCreatinina { get; set; }
//        public List<RequestSearchAllergy> Alergias { get; set; }
//        public List<RequestSearchMolecules> Molecules { get; set; }
//        public List<RequestSearchCIM10> Patologias { get; set; }
//        public Guid IdGEMP { get; set; }
//        public string NombreGrupoEmpresarial { get; set; }
//        public Guid IdSucursal { get; set; }
//        public string Sucursal { get; set; }
//        public int IdAsentamiento { get; set; }
//        public string NombreAsentamiento { get; set; }
//        public int IdTipoAsentamiento { get; set; }
//        public string TipoAsentamiento { get; set; }
//        public int IdCP { get; set; }
//        public string CodigoPostal { get; set; }
//        public int IdEntidad { get; set; }
//        public int IdMunicipio { get; set; }
//        public string NoMunicipio { get; set; }
//        public string Municipio { get; set; }
//        public int IdCiudad { get; set; }
//        public string Ciudad { get; set; }
//        public string Estado { get; set; }
//        public string Abreviatura { get; set; }
//        public DateTime Fecha { get; set; }
//        public string EstatusReceta { get; set; }
//    }
//}

using RMD.Models.Receta.Header.Base;
using RMD.Models.Vidal.Allergy;
using RMD.Models.Vidal.CIM10;
using RMD.Models.Vidal.Molecule;

namespace RMD.Models.Receta.Header.Internos
{
    public class Header_UpdatePacienteParsedRequest : Header_PacienteBase
    {
        public bool PacEmbarazo { get; set; }
        public int PacSemAmenorrea { get; set; }
        public bool PacLactancia { get; set; }
        public decimal? PacCreatinina { get; set; }
        public List<RequestSearchAllergy> Alergias { get; set; } = [];
        public List<RequestSearchMolecules> Molecules { get; set; } = [];
        public List<RequestSearchCIM10> Patologias { get; set; } = [];
    }
}
