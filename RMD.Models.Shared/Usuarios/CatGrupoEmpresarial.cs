using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Usuarios
{
    /// <summary>
    /// Representa un grupo empresarial.
    /// </summary>
    public class CatGrupoEmpresarial
    {
        [Key]
        public Guid IdGEMP { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Logo en formato Base64.
        /// </summary>
        public string? LogoBase64 { get; set; }

        public string Abreviatura { get; set; } = string.Empty;
       
    }

}
