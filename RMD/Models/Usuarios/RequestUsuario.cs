using System.Data.Common;

namespace RMD.Models.Usuarios
{
    public class RequestUsuario
    {
        public Guid IdUsuario { get; set; }
        public string? Usr { get; set; }
        public Guid IdTipoUsuario { get; set; }
        public string? TipoUsuario { get; set; }
        public Guid IdGEMP { get; set; }
        public string? Empresa { get; set; }
        public Guid IdSucursal { get; set; }
        public int? NoSucursal { get; set; }
        public string? NombreSucursal { get; set; }
        public string? Nombres { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public int? IdAsentamiento { get; set; }
        public string? NombreAsentamiento { get; set; }
        public int? IdTipoAsentamiento { get; set; }
        public string? TipoAsentamiento { get; set; }
        public int? IdCP { get; set; }
        public string? CodigoPostal { get; set; }
        public int? IdMunicipio { get; set; }
        public short? NoMunicipio { get; set; }
        public string? Municipio { get; set; }
        public int? IdCiudad { get; set; }
        public string? Ciudad { get; set; }
        public int? IdEntidad { get; set; }
        public string? Estado { get; set; }
        public string? Abreviatura { get; set; }
        public string? Domicilio { get; set; }
        public string? Movil { get; set; }
        public string? Email { get; set; }
        public required string Firma { get; set; } = string.Empty;
        public required string Imagen { get; set; } = string.Empty;
        public string? Status { get; set; }

        public static RequestUsuario FromDataReader(DbDataReader reader)
        {
            return new RequestUsuario
            {
                IdUsuario = reader["IdUsuario"] != DBNull.Value ? (Guid)reader["IdUsuario"] : Guid.Empty,
                Usr = reader["Usr"] as string,
                IdTipoUsuario = reader["IdTipoUsuario"] != DBNull.Value ? (Guid)reader["IdTipoUsuario"] : Guid.Empty,
                TipoUsuario = reader["TipoUsuario"] as string,
                IdGEMP = reader["IdGEMP"] != DBNull.Value ? (Guid)reader["IdGEMP"] : Guid.Empty,
                Empresa = reader["Empresa"] as string,
                IdSucursal = reader["IdSucursal"] != DBNull.Value ? (Guid)reader["IdSucursal"] : Guid.Empty,
                NoSucursal = reader["NoSucursal"] != DBNull.Value ? Convert.ToInt32(reader["NoSucursal"]) : (int?)null,
                NombreSucursal = reader["NombreSucursal"] as string,
                Nombres = reader["Nombres"] as string,
                PrimerApellido = reader["PrimerApellido"] as string,
                SegundoApellido = reader["SegundoApellido"] as string,
                IdAsentamiento = reader["IdAsentamiento"] != DBNull.Value ? Convert.ToInt32(reader["IdAsentamiento"]) : (int?)null,
                NombreAsentamiento = reader["NombreAsentamiento"] as string,
                IdTipoAsentamiento = reader["IdTipoAsentamiento"] != DBNull.Value ? Convert.ToInt32(reader["IdTipoAsentamiento"]) : (int?)null,
                TipoAsentamiento = reader["TipoAsentamiento"] as string,
                IdCP = reader["IdCP"] != DBNull.Value ? Convert.ToInt32(reader["IdCP"]) : (int?)null,
                CodigoPostal = reader["CodigoPostal"] as string,
                IdMunicipio = reader["IdMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["IdMunicipio"]) : (int?)null,
                NoMunicipio = reader["NoMunicipio"] != DBNull.Value ? Convert.ToInt16(reader["NoMunicipio"]) : (short?)null,
                Municipio = reader["Municipio"] as string,
                IdCiudad = reader["IdCiudad"] != DBNull.Value ? Convert.ToInt32(reader["IdCiudad"]) : (int?)null,
                Ciudad = reader["Ciudad"] as string,
                IdEntidad = reader["IdEntidad"] != DBNull.Value ? Convert.ToInt32(reader["IdEntidad"]) : (int?)null,
                Estado = reader["Estado"] as string,
                Abreviatura = reader["Abreviatura"] as string,
                Domicilio = reader["Domicilio"] as string,
                Movil = reader["Movil"] as string,
                Email = reader["Email"] as string,
                Firma = reader["Firma"] as string ?? string.Empty,
                Imagen = reader["Imagen"] as string ?? string.Empty,
                Status = reader["Status"] as string
            };
        }

    }
}
