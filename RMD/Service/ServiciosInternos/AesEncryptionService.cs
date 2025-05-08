using System.Security.Cryptography;
using System.Text;
using RMD.Interface.Security;

namespace RMD.Service.ServiciosInternos
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        public AesEncryptionService(IConfiguration config)
        {
            _key = Convert.FromBase64String(config["Encryption:Key"]!.Trim());
        }
        public string EncryptString(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return $"{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(cipherBytes)}";
        }
    }
}
