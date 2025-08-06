using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.Shared.Models.Login
{
    public class TokenRenewResult
    {
        public string TokenEncrypted { get; set; } = string.Empty;
        public string TokenPlain { get; set; } = string.Empty;
    }

}
