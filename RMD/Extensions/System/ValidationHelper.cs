using System.Text.RegularExpressions;

namespace RMD.Extensions.System
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Valida si el email tiene el formato correcto.
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Patrón: texto@texto.texto
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        /// <summary>
        /// Valida que la contraseña tenga al menos 8 caracteres, 1 mayúscula, 1 minúscula, 1 número y 1 símbolo.
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Patrón: mínimo 8 caracteres, al menos 1 mayúscula, 1 minúscula, 1 dígito y 1 carácter especial.
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
        }
        public static async Task<int> ReadErrorCodeAsync(SqlDataReader reader)
        {
            int code = 0;
            if (await reader.ReadAsync())
            {
                code = reader.GetInt32(0);
            }
            return code;
        }
    }
}
