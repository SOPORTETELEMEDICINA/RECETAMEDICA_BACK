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
    }
}
