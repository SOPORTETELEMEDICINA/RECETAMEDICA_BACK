using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Catalogos
{
    /// <summary>
    /// Representa un catálogo de ubicación geográfica.
    /// </summary>

    public class Catalogo
    {
        [Key]
        public int IdCP { get; set; }

        [Required]
        [StringLength(10)]
        public required string CodigoPostal { get; set; }

        public int IdAsentamiento { get; set; }

        [Required]
        [StringLength(50)]
        public required string TipoAsentamiento { get; set; }

        [Required]
        [StringLength(100)]
        public required string NombreAsentamiento { get; set; }

        public int IdMunicipio { get; set; }

        [Required]
        [StringLength(50)]
        public required string NoMunicipio { get; set; }

        [Required]
        [StringLength(100)]
        public required string NombreMunicipio { get; set; }

        public int IdCiudad { get; set; }

        [Required]
        [StringLength(100)]
        public required string NombreCiudad { get; set; }

        public int IdEntidad { get; set; }

        [Required]
        [StringLength(100)]
        public required string NombreEntidad { get; set; }

        [Required]
        [StringLength(10)]
        public required string AbreviaturaEntidad { get; set; }
    }
}

