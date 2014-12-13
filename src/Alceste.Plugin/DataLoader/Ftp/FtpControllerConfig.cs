using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alceste.Plugin.DataLoader.Ftp
{
    public sealed class FtpControllerConfig : DataControllerWithLoginConfig
    {
        public FtpControllerConfig(bool isSsl = true, bool isBinary = true, bool isPassive = true)
        {
            IsSsl = isSsl;
            IsBinary = isBinary;
            IsPassive = isPassive;
        }

        public bool IsSsl { get; set; }

        public bool IsBinary { get; set; }

        public bool IsPassive { get; set; }
    }
}
