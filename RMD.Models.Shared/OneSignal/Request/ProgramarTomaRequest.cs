namespace RMD.Shared.Models.OneSignal.Request
{
    public sealed class ProgramarTomaRequest
    {
        public Guid IdAlertaToma { get; set; }                // NUEVO: identificador de la alerta
        public int TipoAlerta { get; set; } = 1;             // NUEVO: 1 = normal, 2 = manual

        public string Medicamento { get; set; } = "";         // texto visible
        public DateTimeOffset FechaHoraTomaUtc { get; set; }  // hora exacta de toma (UTC)
        public int MinutosAntes { get; set; } = 10;           // cuánto antes avisar

    }
}
