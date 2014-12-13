using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Alceste.Plugin.DataLoader.Http
{
    public sealed class HttpController : AWebLoaderController<HttpWebRequest, HttpWebResponse, HttpControllerConfig>
    {
        public HttpController(HttpControllerConfig config)
            : base(config)
        {
        }

        public override string ProtocolCode
        {
            get { return "http"; }
        }

        public override HttpWebRequest CreateRequestDownload(string filePath)
        {
            var request = CreateRequest(filePath);
            request.Method = WebRequestMethods.Http.Get;
            return request;
        }

        protected override DataLoaderExecutionResult resultByErrorCode(HttpWebResponse responceResult)
        {
            switch (responceResult.StatusCode)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.NotAcceptable:
                    return new DataLoaderExecutionResult(DataLoaderExecutionCode.FileNotFound);
                default:
                    return new DataLoaderExecutionResult(DataLoaderExecutionCode.AuthorisationFailed);
            }
        }

        public override HttpWebRequest CreateRequestDirectory(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
