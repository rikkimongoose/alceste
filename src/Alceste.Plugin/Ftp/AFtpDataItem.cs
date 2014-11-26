namespace Alceste.Plugin.Ftp
{
    public abstract class AFtpDataItem
    {
        public string FileName { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
