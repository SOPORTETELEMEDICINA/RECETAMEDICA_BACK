namespace RMD.Shared.Models.OneSignal.Request
{
    public sealed class ProgramarTomaRequest
    {
        public Guid IdAlertaTomaProgramada { get; set; }   // del SP
        public Guid IdPaciente { get; set; }               // del SP
        public int TipoAlerta { get; set; } = 1;          // 1=normal, 2=manual (del SP)
        public string Medicamento { get; set; } = "";      // del SP

        // IMPORTANTE: el SP entrega hora LOCAL (datetime), no UTC
        public DateTime FechaHoraTomaLocal { get; set; }   // del SP: FechaHoraTomaLocal

        public int MinutosAntes { get; set; } = 10;        // del SP
    }
}
