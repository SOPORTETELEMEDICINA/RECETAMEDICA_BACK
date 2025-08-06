using RMD.Shared.Models.Usuarios;

namespace RMD.Shared.Models.Login
{
    public class LoginResult
    {
        // Este es el string que se debe enviar en el Authorization header: iv:cipherText
        public string TokenEncrypted { get; set; } = string.Empty;

        // Este es el contenido desencriptado
        public string TokenPlain { get; set; } = string.Empty;

        // Usuario desencriptado
        public UsuarioDetalle User { get; set; } = new();
    }
}
