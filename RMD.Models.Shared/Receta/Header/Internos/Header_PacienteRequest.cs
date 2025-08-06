using RMD.Shared.Models.Receta.Header.Base;

namespace RMD.Shared.Models.Receta.Header.Internos
{
    public class Header_PacienteRequest: Header_PacienteBase
    {
        public string NombresMedico { get; set; }
        public string PrimerApellidoMedico { get; set; }
        public string SegundoApellidoMedico { get; set; }
        public string Movil { get; set; }
        public string Email { get; set; }
        public string Universidad { get; set; }
        public string CedulaGeneral { get; set; }
        public string Especialidad { get; set; }
        public string CedulaEspecialidad { get; set; }
        public string Horario { get; set; }
        public string Firma { get; set; }
        public string LogoBase64 { get; set; }
        public string NumeroSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string RegistroSanitario { get; set; }
        public string Domicilio { get; set; }
        public string NombreMunicipio { get; set; }
        public string NombreCiudad { get; set; }
        public string Diagnosticos { get; set; }
        public string Folio { get; set; }
        public int Estatus { get; set; }

    }

}
