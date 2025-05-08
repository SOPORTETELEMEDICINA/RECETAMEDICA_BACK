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

        public string Abreviatura { get; set; } = string.Empty;
        public static CatGrupoEmpresarial FromDataReader(SqlDataReader reader)
        {
            return new CatGrupoEmpresarial
            {
                IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                Nombre = reader["Nombre"] as string ?? string.Empty,
                LogoBase64 = reader["LogoBase64"] as string,
                Abreviatura = reader["Abreviatura"] as string ?? string.Empty
            };
        }
    }

}
