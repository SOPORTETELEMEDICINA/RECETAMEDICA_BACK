using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMD.Models.Login
{
    [Table("RegistroPeticiones")]
    public class RegistroPeticion
    {
        [Key]
        public long IdRegistro { get; set; }
        public DateTime Fecha { get; set; }
        public string Controller { get; set; }
        public string Endpoint { get; set; }
        public Guid? IdGEMP { get; set; }
        public Guid? IdSucursal { get; set; }
        public Guid? IdUsuario { get; set; }
        public Guid? IdRol { get; set; }
        public string Parametros { get; set; }
    }
}
