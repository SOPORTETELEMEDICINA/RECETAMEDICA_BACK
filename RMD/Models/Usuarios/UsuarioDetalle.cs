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
        public required string Firma { get; set; } = string.Empty;
        public required string Imagen { get; set; } = string.Empty;
        public static UsuarioDetalle FromDataReader(SqlDataReader reader)
        {
            return new UsuarioDetalle
            {
                IdUsuario = reader["IdUsuario"] != DBNull.Value ? (Guid)reader["IdUsuario"] : Guid.Empty,
                Usr = reader["Usr"] as string ?? string.Empty,
                IdTipoUsuario = reader["IdTipoUsuario"] != DBNull.Value ? (Guid)reader["IdTipoUsuario"] : Guid.Empty,
                TipoUsuario = reader["TipoUsuario"] as string ?? string.Empty,
                IdGEMP = reader["IdGEMP"] != DBNull.Value ? (Guid?)reader["IdGEMP"] : null,
                Empresa = reader["Empresa"] as string ?? string.Empty,
                IdSucursal = reader["IdSucursal"] != DBNull.Value ? (Guid?)reader["IdSucursal"] : null,
                NombreSucursal = reader["NombreSucursal"] as string ?? string.Empty,
                Nombres = reader["Nombres"] as string ?? string.Empty,
                PrimerApellido = reader["PrimerApellido"] as string ?? string.Empty,
                SegundoApellido = reader["SegundoApellido"] as string ?? string.Empty,
                IdAsentamiento = reader["IdAsentamiento"] != DBNull.Value ? Convert.ToInt32(reader["IdAsentamiento"]) : 0,
                NombreAsentamiento = reader["NombreAsentamiento"] as string ?? string.Empty,
                IdCP = reader["IdCP"] != DBNull.Value ? Convert.ToInt32(reader["IdCP"]) : 0,
                CodigoPostal = reader["CodigoPostal"] as string ?? string.Empty,
                IdMunicipio = reader["IdMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["IdMunicipio"]) : 0,
                Municipio = reader["Municipio"] as string ?? string.Empty,
                Domicilio = reader["Domicilio"] as string ?? string.Empty,
                Movil = reader["Movil"] as string ?? string.Empty,
                Email = reader["Email"] as string ?? string.Empty,
                Firma = reader["Firma"] as string ?? string.Empty,
                Imagen = reader["Imagen"] as string ?? string.Empty,
            };
        }
    }

}
