using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Shared.Models.OneSignal.Request
{
    public sealed class CancelarNotificacionRequest
    {
        public Guid IdAlertaTomaProgramada { get; set; }
        public string OneSignalNotificationId { get; set; } = "";
        public Guid IdPaciente { get; set; }         
    }

}
