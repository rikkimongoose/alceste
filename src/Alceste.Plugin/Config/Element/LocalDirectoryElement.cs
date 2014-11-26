using System.Configuration;

namespace Alceste.Plugin.Config.Element
{
    public class LocalDirectoryElement : ConfigurationElement
    {
        [ConfigurationProperty("path", DefaultValue = "", IsRequired = false)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }
}
