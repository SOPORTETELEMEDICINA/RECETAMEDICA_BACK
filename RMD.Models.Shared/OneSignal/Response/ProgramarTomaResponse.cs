using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Shared.Models.OneSignal.Response
{
    public sealed class ProgramarTomaResponse
    {
        public Guid IdAlertaToma { get; set; }
        public int TipoAlerta { get; set; }
        public bool Exito { get; set; }
        public string? OneSignalNotificationId { get; set; }
        public int HttpStatus { get; set; }
        public DateTimeOffset? SendAfterUtc { get; set; }
        public string? Error { get; set; }
    }
}
