using Alceste.Plugin.Config;
using Alceste.Plugin.DataLoader.File;
using Alceste.Plugin.DataLoader.Ftp;
using Alceste.Plugin.DataLoader.Http;
namespace Alceste.Plugin.DataLoader
{
    public static class DataLoaderGenerator
    {
        public static ADataLoaderController GetLoader(PluginConfig config)
        {
            var configFile = config as FilePluginConfig;
            if (configFile != null)
                return new FileController(new FileControllerConfig { });
            var configFtp = config as FtpPluginConfig;
            if (configFtp != null)
                return new FtpController(new FtpControllerConfig { });
            var configHttp = config as HttpPluginConfig;
            if (configFtp != null)
                return new HttpController(new HttpControllerConfig { });

            return null;
        }
    }
}
