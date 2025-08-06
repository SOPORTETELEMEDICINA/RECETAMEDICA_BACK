namespace RMD.Models.Consulta
{
    public class RequestSearchVMP
    {
        public int IdVMP{ get; set; }
        public string NameVMP { get; set; } = string.Empty;
        public string ActivePrinciples { get; set; } = string.Empty;
        public string GalenicForm { get; set; } = string.Empty;

    }
}
