namespace RMD.Shared.Models.ServiciosInternos
{
    public class EncryptRequest
    {
        public string TextoPlano { get; set; } = string.Empty;
        public string Entorno { get; set; } = "DEV"; // DEV, QA, PROD
    }

}
