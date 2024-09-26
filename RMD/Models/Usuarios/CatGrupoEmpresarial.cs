using System;
using System.ComponentModel.DataAnnotations;

namespace RMD.Models.Usuarios
{
    /// <summary>
    /// Representa un grupo empresarial.
    /// </summary>
    public class CatGrupoEmpresarial
    {
        [Key]
        public Guid IdGEMP { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Logo en formato Base64.
        /// </summary>
        public string? LogoBase64 { get; set; }
    }
}
