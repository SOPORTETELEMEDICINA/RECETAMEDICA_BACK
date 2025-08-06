namespace RMD.Shared.Models.ServiciosInternos
{
    public class IvCipherPair
    {
        public string Iv { get; set; }
        public string CipherText { get; set; }

        public override string ToString()
            => $"{Iv}:{CipherText}";
    }

}
