using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Web;
using Alceste.Plugin.DataLoader;
using Alceste.Plugin.Utils;

namespace Alceste.Plugin.DataLoader
{
    public abstract class ADataLoaderController<TDataControllerConfig> : ADataLoaderController
        where TDataControllerConfig : DataControllerConfig
    {
        public TDataControllerConfig Сonfig { get; private set; }

        public ADataLoaderController(TDataControllerConfig config)
        {
            Сonfig = config;
        }
    }

    public abstract class ADataLoaderController
    {
        public abstract MemoryStream GetFileStream(string filepath);

        public abstract MemoryStream GetFileByMask(string filepath);

        public abstract bool FileExists(string filePath);
    }

}
