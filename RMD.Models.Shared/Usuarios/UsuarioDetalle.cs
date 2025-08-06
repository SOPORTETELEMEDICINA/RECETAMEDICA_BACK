using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Usuarios
{
    public class UsuarioDetalle
    {
        [Key]
        public Guid IdUsuario { get; set; }
        public string Usr { get; set; }
        public Guid IdTipoUsuario { get; set; }
        public string TipoUsuario { get; set; }
        public Guid? IdGEMP { get; set; }
        public string Empresa { get; set; }
        public Guid? IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; } = string.Empty;
        public int IdAsentamiento { get; set; }
        public string NombreAsentamiento { get; set; }
        public int IdCP { get; set; }
        public string CodigoPostal { get; set; }
        public int IdMunicipio { get; set; }
        public string Municipio { get; set; }
        public string Domicilio { get; set; }
        public string Movil { get; set; }
        public string Email { get; set; }
        public string Firma { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
    }

}
