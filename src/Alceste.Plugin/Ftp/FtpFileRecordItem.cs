using System;

namespace Alceste.Plugin.Ftp
{
    public sealed class FtpFileRecordItem : AFtpDataItem
    {
        public DateTime Date { get; set; }
        public long Size { get; set; }
    }
}
