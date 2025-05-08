using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Sucursales
{
    public class SucursalRequest
    {
        public Guid IdSucursal { get; set; }
        [Required]
        public Guid IdGEMP { get; set; }

        public int Numero { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string RegistroSanitario { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Responsable { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CedulaResponsable { get; set; } = string.Empty;

        [MaxLength(15)]
        public string TelefonoResponsable { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EmailResponsable { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Domicilio { get; set; } = string.Empty;

        public int IdAsentamiento { get; set; }

        [MaxLength(150)]
        public string NombreAsentamiento { get; set; } = string.Empty;

        public int IdTipoAsentamiento { get; set; }

        [MaxLength(50)]
        public string TipoAsentamiento { get; set; } = string.Empty;

        public int IdCP { get; set; }

        [MaxLength(10)]
        public string CodigoPostal { get; set; } = string.Empty;

        public int IdMunicipio { get; set; }

        [MaxLength(50)]
        public short NoMunicipio { get; set; } 

        [MaxLength(150)]
        public string Municipio { get; set; } = string.Empty;

        public int IdCiudad { get; set; }

        [MaxLength(150)]
        public string Ciudad { get; set; } = string.Empty;

        public int IdEntidad { get; set; }

        [MaxLength(150)]
        public string Estado { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Abreviatura { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;
        public static SucursalRequest FromDataReader(SqlDataReader reader)
        {
            return new SucursalRequest
            {
                IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                Numero = reader.GetInt32(reader.GetOrdinal("Numero")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                RegistroSanitario = reader.GetString(reader.GetOrdinal("RegistroSanitario")),
                Responsable = reader.GetString(reader.GetOrdinal("Responsable")),
                CedulaResponsable = reader.GetString(reader.GetOrdinal("CedulaResponsable")),
                TelefonoResponsable = reader.GetString(reader.GetOrdinal("TelefonoResponsable")),
                EmailResponsable = reader.GetString(reader.GetOrdinal("EmailResponsable")),
                Domicilio = reader.GetString(reader.GetOrdinal("Domicilio")),
                IdAsentamiento = reader.GetInt32(reader.GetOrdinal("IdAsentamiento")),
                NombreAsentamiento = reader.GetString(reader.GetOrdinal("NombreAsentamiento")),
                IdTipoAsentamiento = reader.GetInt32(reader.GetOrdinal("IdTipoAsentamiento")),
                TipoAsentamiento = reader.GetString(reader.GetOrdinal("TipoAsentamiento")),
                IdCP = reader.GetInt32(reader.GetOrdinal("IdCP")),
                CodigoPostal = reader.GetString(reader.GetOrdinal("CodigoPostal")),
                IdMunicipio = reader.GetInt32(reader.GetOrdinal("IdMunicipio")),
                NoMunicipio = (short)reader.GetInt16(reader.GetOrdinal("NoMunicipio")),
                Municipio = reader.GetString(reader.GetOrdinal("Municipio")),
                IdCiudad = reader.GetInt32(reader.GetOrdinal("IdCiudad")),
                Ciudad = reader.GetString(reader.GetOrdinal("Ciudad")),
                IdEntidad = reader.GetInt32(reader.GetOrdinal("IdEntidad")),
                Estado = reader.GetString(reader.GetOrdinal("Estado")),
                Abreviatura = reader.GetString(reader.GetOrdinal("Abreviatura")),
                Status = reader.GetString(reader.GetOrdinal("Status"))
            };
        }
    }
}
