using System.Security.Cryptography;
using System.Text;

namespace RMD.Shared.Utils.Helpers
{
    public static class EncryptionHelper
    {
        public static string Encrypt(string plainText, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return $"{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(cipherBytes)}";
        }
    }

}
