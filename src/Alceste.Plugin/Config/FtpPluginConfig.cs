using System.Configuration;
using Alceste.Plugin.Config.Element;

namespace Alceste.Plugin.Config
{
    public sealed class FtpPluginConfig : DbPluginConfig
    {
        public const string FTPKey = "FTP";
        [ConfigurationProperty(FTPKey)]
        public FtpElement FTP
        {
            get { return ((FtpElement)(base[FTPKey])); }
        }
    }
}
