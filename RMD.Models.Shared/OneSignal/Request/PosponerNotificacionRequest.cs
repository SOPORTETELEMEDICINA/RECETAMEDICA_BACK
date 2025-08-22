using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Shared.Models.OneSignal.Request
{
    public sealed class PosponerNotificacionRequest
    {
        public Guid IdAlertaTomaProgramadaBase { get; init; }   // fila base a posponer
        public Guid IdPaciente { get; init; }                   // external_user_id en OneSignal
        public int TipoAlerta { get; init; }                   // 1=normal, 2=manual
        public string Medicamento { get; init; } = default!;    // texto a mostrar
        public short SnoozeMinutos { get; init; }               // minutos a posponer (>0)
        public string? OneSignalNotificationId { get; init; }   // para cancelar la previa (opcional)
    }

}
