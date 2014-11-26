using System.Collections.Generic;

namespace Alceste.Plugin.Ftp
{
    public sealed class FtpDirectoryDataItem : AFtpDataItem
    {
        public List<FtpFileRecordItem> FileRecords { get; set; }
    }
}
