namespace RMD.Shared.Models.Login
{
    public class JwtKeyHolder
    {
        public byte[] Key { get; }
        public byte[] ResponseKey { get; }

        public JwtKeyHolder(string base64Key, string base64ResponseKey)
        {
            Key = Convert.FromBase64String(base64Key);
            ResponseKey = Convert.FromBase64String(base64ResponseKey);
        }
    }
}

