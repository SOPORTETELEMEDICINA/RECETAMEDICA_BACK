using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Usuarios
{
    public class UsuarioDetalle
    {
        [Key]
        public Guid IdUsuario { get; set; }
        public required string Usr { get; set; }
        public Guid IdTipoUsuario { get; set; }
        public required string TipoUsuario { get; set; }
        public Guid? IdGEMP { get; set; }
        public required string Empresa { get; set; }
        public Guid? IdSucursal { get; set; }
        public required string NombreSucursal { get; set; }
        public required string Nombres { get; set; }
        public required string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; } = string.Empty;
        public int IdAsentamiento { get; set; }
        public required string NombreAsentamiento { get; set; }
        public int IdCP { get; set; }
        public required string CodigoPostal { get; set; }
        public int IdMunicipio { get; set; }
        public required string Municipio { get; set; }
        public required string Domicilio { get; set; }
        public required string Movil { get; set; }
        public required string Email { get; set; }
    }

}
