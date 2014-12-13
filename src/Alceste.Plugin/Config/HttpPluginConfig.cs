using System.Configuration;
using Alceste.Plugin.Config.Element;

namespace Alceste.Plugin.Config
{
    public sealed class HttpPluginConfig : DbPluginConfig
    {
        public const string HTTPKey = "HTTP";
        [ConfigurationProperty(HTTPKey)]
        public HttpElement HTTP
        {
            get { return ((HttpElement)(base[HTTPKey])); }
        }
    }
}
