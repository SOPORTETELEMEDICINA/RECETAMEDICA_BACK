using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Usuarios
{
    /// <summary>
    /// Representa un tipo de usuario.
    /// </summary>
    public class TipoUsuario
    {
        [Key]
        public Guid IdTipoUsuario { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
        public static TipoUsuario FromDataReader(SqlDataReader reader)
        {
            return new TipoUsuario
            {
                IdTipoUsuario = reader.GetGuid(reader.GetOrdinal("IdTipoUsuario")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre"))
            };
        }
    }
}
