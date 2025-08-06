using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMD.Models.GlobalResponse
{
    [Table("CatalogoNotificaciones", Schema = "Configuracion")]
    public class CatalogoNotificacion
    {
        [Key]
        public int CodigoNotificacion { get; set; }

        [Required]
        [StringLength(100)]
        public string Tipo { get; set; }

        [Required]
        [StringLength(100)]
        public string Funcion { get; set; }

        [Required]
        [StringLength(250)]
        public string Mensaje { get; set; }

        public string? Descripcion { get; set; }

        [Required]
        public int HttpStatusCode { get; set; }

        [Required]
        [StringLength(50)]
        public string ToastType { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }
    }
}
