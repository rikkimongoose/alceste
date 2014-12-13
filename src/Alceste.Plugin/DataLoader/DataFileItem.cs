using System;

namespace Alceste.Plugin.DataLoader
{
    public class DataFileItem
    {
        public string FileName { get; set; }

        public override string ToString()
        {
            return FileName;
        }

        public DateTime Date { get; set; }
        public long Size { get; set; }
    }
}
