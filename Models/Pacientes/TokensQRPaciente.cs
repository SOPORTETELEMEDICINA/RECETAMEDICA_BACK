namespace RMD.Models.Pacientes
{
    public class TokensQRPaciente
    {
        public Guid IdToken { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = string.Empty;
        public Guid IdPaciente { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; } = false;
        public DateTime? UsadoEn { get; set; }
        public string? IpUso { get; set; }
        public string? EndpointAccedido { get; set; }
    }
}
