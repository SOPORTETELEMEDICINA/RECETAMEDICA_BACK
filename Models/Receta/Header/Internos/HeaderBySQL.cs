using RMD.Models.Receta.Header.Base;

namespace RMD.Models.Receta.Header.Internos
{
    public class HeaderBySQL : Header_Base
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
        public int IdTipoIdentificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public DateTime FechaNacimientoPaciente { get; set; }
        public string Alergias { get; set; }
        public string Molecules { get; set; }
        public string Patologias { get; set; }
        public string Sucursal { get; set; }
        public string GrupoEmpresarial { get; set; }

        public int Estatus { get; set; }
        public string Descripcion { get; set; }
    }
}