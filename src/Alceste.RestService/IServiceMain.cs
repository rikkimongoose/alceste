using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Alceste.Model;

namespace Alceste.RestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServiceMain
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "getids")]
        List<MediaFileServerRecord> getFilesList();

        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "getinfo/{fileid}")]
        List<MediaFileItem> getInfo(string fileId);

        [OperationContract]
        [WebGet(UriTemplate = "media/{fileid}/{channel=1}")]
        Stream getMedia(string fileId, string channel);

        [OperationContract]
        [WebGet(UriTemplate = "mediapic/{fileid}/{channel=1}/{width=500}/{height=150}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream getMediaImage(string fileId, string width, string height, string channel);
    }
}
