using RMD.Interface.Security;
using RMD.Shared.Models.Login;
using RMD.Shared.Models.ServiciosInternos;
using RMD.Shared.Utils.Interface;
using System.Security.Cryptography;
using System.Text;

namespace RMD.Controllers.HerramientasAdministrativas
{
    [ApiController]
    [Route("api/[controller]")]
    public class EncryptionController : ControllerBase
    {
        private readonly JwtKeyHolder _keyHolder;
        private readonly IEncryptionService _encryptionService;

        public EncryptionController(JwtKeyHolder keyHolder, IEncryptionService encryptionService)
        {
            _keyHolder = keyHolder;
            _encryptionService = encryptionService;
        }

        [HttpPost("decrypt-data")]
        [AllowAnonymous]
        public IActionResult DecryptData([FromBody] ResponseFromService<string> encryptedResponse)
        {
            if (string.IsNullOrWhiteSpace(encryptedResponse.Data) || !encryptedResponse.Data.Contains(":"))
                return BadRequest("El formato del dato encriptado no es válido.");

            try
            {
                var parts = encryptedResponse.Data.Split(':', 2);
                var ivBytes = Convert.FromBase64String(parts[0]);
                var cipherBytes = Convert.FromBase64String(parts[1]);

                if (ivBytes.Length != 16)
                    return BadRequest("IV inválido. Debe tener 16 bytes.");

                using var aes = Aes.Create();
                aes.Key = _keyHolder.ResponseKey;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                var decrypted = Encoding.UTF8.GetString(plainBytes).TrimStart();

                if (decrypted.StartsWith("{") || decrypted.StartsWith("["))
                    return Ok(JsonSerializer.Deserialize<object>(decrypted, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
                if (decrypted.StartsWith("<html", StringComparison.OrdinalIgnoreCase) || decrypted.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase))
                    return Content(decrypted, "text/html");
                if (decrypted.StartsWith("<"))
                    return Content(decrypted, "text/xml");

                return Content(decrypted, "text/plain");
            }
            catch (Exception ex)
            {
                return BadRequest($"No se pudo desencriptar o interpretar la respuesta: {ex.Message}");
            }
        }

        [HttpPost("decrypt-token")]
        [AllowAnonymous]
        public IActionResult Decrypt([FromBody] EncryptedPayload payload)
        {
            try
            {
                var decrypted = _encryptionService.DecryptString(payload.iv, payload.cipherText);
                return Ok(new { token = decrypted });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Error al desencriptar token.",
                    details = ex.Message
                });
            }
        }

        [HttpPost("decrypt-json")]
        [AllowAnonymous]
        public IActionResult DecryptJson([FromBody] EncryptedPayload payload)
        {
            try
            {
                var json = _encryptionService.DecryptString(payload.iv, payload.cipherText);
                var obj = JsonSerializer.Deserialize<object>(json);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "No se pudo desencriptar/deserializar",
                    details = ex.Message
                });
            }
        }

        [HttpPost("encriptar-cadena")]
        [AllowAnonymous]
        public IActionResult EncriptarTexto([FromBody] EncryptRequest request)
        {
            try
            {
                var entorno = request.Entorno.Trim().ToLower();
                var keyPath = $@"C:\Secrets\RecetaMedica\{entorno}.key";

                if (!System.IO.File.Exists(keyPath))
                    return BadRequest($"No se encontró el archivo de clave para el entorno '{entorno}'.");

                var base64Key = System.IO.File.ReadAllText(keyPath).Trim();
                var keyBytes = Convert.FromBase64String(base64Key);

                var encrypted = EncryptionHelper.Encrypt(request.TextoPlano, keyBytes);
                return Ok(new { Encrypted = encrypted });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al encriptar: {ex.Message}");
            }
        }

        [HttpPost("encriptar-comun")]
        [AllowAnonymous]
        public IActionResult EncriptarTextoComun([FromBody] string textoPlano)
        {
            try
            {
                var basePaths = new[]
                {
                @"C:\Secrets\RecetaMedica",
                @"D:\Secrets\RecetaMedica"
            };

                var keyFile = basePaths
                    .Select(path => Path.Combine(path, "common.key"))
                    .FirstOrDefault(System.IO.File.Exists);

                if (keyFile is null)
                    return BadRequest("No se encontró el archivo 'common.key'.");

                var base64Key = System.IO.File.ReadAllText(keyFile).Trim();
                var keyBytes = Convert.FromBase64String(base64Key);

                var encrypted = EncryptionHelper.Encrypt(textoPlano, keyBytes);
                return Ok(new { Encrypted = encrypted });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al encriptar: {ex.Message}");
            }
        }

        public class EncryptedPayload
        {
            public string iv { get; set; } = null!;
            public string cipherText { get; set; } = null!;
        }
    }
}

