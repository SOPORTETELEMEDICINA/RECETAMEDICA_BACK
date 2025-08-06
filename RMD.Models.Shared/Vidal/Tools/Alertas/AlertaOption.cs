namespace RMD.Shared.Models.Vidal.Tools.Alertas
{
    public class AlertaOption
    {
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string Provider { get; set; } = string.Empty;
    }

}
