using System.Configuration;
using Alceste.Plugin.Config.Element;

namespace Alceste.Plugin.Config
{
    public class DbPluginConfig : PluginConfig
    {
        private const string DatabaseKey = "Database";
        [ConfigurationProperty(DatabaseKey)]
        public DbElement Database
        {
            get { return ((DbElement)(base[DatabaseKey])); }
        }
    }
}
