namespace RMD.Extensions
{
    public class CifradoHelper
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            bool result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            return result;
        }
    }
}
