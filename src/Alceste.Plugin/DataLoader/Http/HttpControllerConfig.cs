using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alceste.Plugin.DataLoader.Http
{
    public sealed class HttpControllerConfig : DataControllerWithLoginConfig
    {
        public bool ServerAllowsDirectoryList { get; set; }

        public string ServerDirectoryListUrl { get; set; }

        public HttpDirectoryListType HttpDirectoryListUrlType { get; set; }
    }

    public enum HttpDirectoryListType
    {
        JSon,
        XML,
        List
    }
}
