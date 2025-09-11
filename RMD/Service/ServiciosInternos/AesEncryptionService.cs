//using System.Security.Cryptography;
//using System.Text;
//using RMD.Interface.Security;

//namespace RMD.Service.ServiciosInternos
//{
//    public class AesEncryptionService : IEncryptionService
//    {
//        private readonly byte[] _key;

//        public AesEncryptionService(byte[] encryptionKey)
//        {
//            _key = encryptionKey;
//        }
//        public string EncryptString(string plainText)
//        {
//            using var aes = Aes.Create();
//            aes.Key = _key;
//            aes.GenerateIV();
//            using var encryptor = aes.CreateEncryptor();
//            var plainBytes = Encoding.UTF8.GetBytes(plainText);
//            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
//            return $"{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(cipherBytes)}";
//        }

//        public string DecryptString(string ivBase64, string cipherTextBase64)
//        {
//            var ivBytes = Convert.FromBase64String(ivBase64);
//            var cipherBytes = Convert.FromBase64String(cipherTextBase64);

//            using var aes = Aes.Create();
//            aes.Key = _key;
//            aes.IV = ivBytes;
//            using var decryptor = aes.CreateDecryptor();
//            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

//            return Encoding.UTF8.GetString(plainBytes);
//        }
//    }
//}