using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RMD.Shared.Models.ServiciosInternos;

namespace RMD.Shared.Utils.Encryption
{
    public static class DecryptionHelper
    {
        // ✅ DecryptData: recibe string "iv:cipherText" y la llave
        public static string DecryptData(string base64Combined, byte[] key)
        {
            var parts = base64Combined.Split(':', 2);
            var ivBytes = Convert.FromBase64String(parts[0]);
            var cipherBytes = Convert.FromBase64String(parts[1]);

            if (ivBytes.Length != 16)
                throw new InvalidOperationException("IV inválido. Debe tener 16 bytes.");

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes).TrimStart();
        }

        // ✅ DecryptData: recibe string "iv:cipherText" y la llave
        private static string DecryptString(string ivBase64, string cipherTextBase64, byte[] key)
        {
            var ivBytes = Convert.FromBase64String(ivBase64);
            var cipherBytes = Convert.FromBase64String(cipherTextBase64);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = ivBytes;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

        // ✅ DecryptPayload: toma el EncryptedPayload y la key, devuelve texto plano
        public static string DecryptPayload(EncryptedPayload payload, byte[] key)
        {
            return DecryptString(payload.iv, payload.cipherText, key);
        }

        // ✅ DecryptJson<T>: toma el payload y key, y deserializa el resultado
        public static T DecryptJson<T>(EncryptedPayload payload, byte[] key)
        {
            var json = DecryptPayload(payload, key);
            return JsonSerializer.Deserialize<T>(json)!;
        }
    }
}