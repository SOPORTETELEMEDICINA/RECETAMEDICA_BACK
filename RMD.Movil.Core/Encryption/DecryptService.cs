using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RMD.Shared.Models.ServiciosInternos;
using RMD.Shared.Utils.Encryption;

namespace RMD.Movil.Core.Encryption
{
    public static class DecryptService
    {
        // 🔐 Key para desencriptar el token
        private static readonly byte[] _tokenKey = Convert.FromBase64String("Zjk3KOXcE5PGoOA5FmpG67olfRR7QoBeKjczYGe8bB8=");

        // 🔐 Key para desencriptar user (común)
        //private static readonly byte[] _mainKey = Convert.FromBase64String("G4n8F8zkWTh+I6K2bfIo8m1gU+KbVYb0OtG7W3A5axQ=");

        private static readonly byte[] _mainKey = Convert.FromBase64String("Zjk3KOXcE5PGoOA5FmpG67olfRR7QoBeKjczYGe8bB8=");
        /// <summary>
        /// Desencripta un payload cifrado (iv + cipherText) con la clave indicada.
        /// </summary>
        private static string Decrypt(string ivBase64, string cipherBase64, byte[] key)
        {
            var iv = Convert.FromBase64String(ivBase64);
            var cipherBytes = Convert.FromBase64String(cipherBase64);

            if (iv.Length != 16)
                throw new InvalidOperationException("IV inválido. Debe tener 16 bytes.");

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes).Trim();
        }

        /// <summary>
        /// Desencripta el token cifrado (EncryptedPayload) usando la clave del token.
        /// </summary>
        public static string FromTokenPayload(EncryptedPayload payload)
        {
            return DecryptionHelper.DecryptPayload(payload, _tokenKey);
        }

        /// <summary>
        /// Desencripta un payload genérico (como el usuario) y lo convierte a objeto usando la clave principal.
        /// </summary>
        public static T FromUserPayload<T>(EncryptedPayload payload)
        {
            var json = Decrypt(payload.iv, payload.cipherText, _mainKey);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }
    }
}
