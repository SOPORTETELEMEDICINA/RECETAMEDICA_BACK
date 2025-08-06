using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Catalogo
{
    public class CatAsentamientos
    {
        [Key]
        public int IdAsentamiento { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public int IdTipoAsentamiento { get; set; }
        public int IdCP { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdCiudad { get; set; }
    }
}
