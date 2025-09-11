using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMD.Shared.Models.Login
{
    [Table("AuthTokens", Schema = "auth")]
    public class AuthToken
    {
        [Key]
        [Column("IdToken")]
        public Guid IdToken { get; set; }

        [Required]
        [Column("IdUsuario")]
        public Guid IdUsuario { get; set; }

        [Required]
        [Column("Token")]
        public string Token { get; set; } = null!;

        [Required]
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("ExpiresAt")]
        public DateTime ExpiresAt { get; set; }

        [Column("RevokedAt")]
        public DateTime? RevokedAt { get; set; }

        [Column("LastAuth2F")]
        public DateTime? LastAuth2F { get; set; }

        [Column("Auth2FIsRequired")]
        public bool Auth2FIsRequired { get; set; }

        [Column("Auth2FIsAuth")]
        public bool Auth2FIsAuth { get; set; }
    }
}