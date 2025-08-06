namespace RMD.Shared.Models.ServiciosInternos
{

    public class EncryptedPayload
    {
        public string iv { get; set; } = null!;
        public string cipherText { get; set; } = null!;
    }
}
