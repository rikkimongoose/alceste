using System.Configuration;
using Alceste.Plugin.Config.Element;

namespace Alceste.Plugin.Config
{
    public class LocalFilePluginConfig : PluginConfig
    {
        public const string LocalDirectoryKey = "LocalDirectory";
        [ConfigurationProperty(LocalDirectoryKey)]
        public LocalDirectoryElement LocalDirectory
        {
            get { return ((LocalDirectoryElement)(base[LocalDirectoryKey])); }
        }
    }
}
