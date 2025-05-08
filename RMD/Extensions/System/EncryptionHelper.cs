using System.Security.Cryptography;
using System.Text;

namespace RMD.Extensions.System
{
    public static class EncryptionHelper
    {
        private static readonly string Key = "'%:i>Cu`Ut=Hi{&w7XtL0*CWvD]Sw.cD";

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(Key) || Key.Length != 32)
            {
                throw new ArgumentException("La clave de cifrado debe tener exactamente 32 caracteres.");
            }

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.GenerateIV(); // Genera un vector de inicialización (IV) aleatorio

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length); // Escribe el IV al inicio del flujo
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText); // Escribe el texto cifrado
            }

            return Convert.ToBase64String(ms.ToArray()); // Convierte el flujo completo (IV + texto cifrado) a Base64
        }


        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(Key) || Key.Length != 32)
            {
                throw new ArgumentException("La clave de cifrado debe tener exactamente 32 caracteres.");
            }

            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);

            // Extraer el IV del inicio del texto cifrado
            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(fullCipher, iv, iv.Length);
            aes.IV = iv;

            // Extraer el texto cifrado (sin el IV)
            var cipher = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

    }
}
