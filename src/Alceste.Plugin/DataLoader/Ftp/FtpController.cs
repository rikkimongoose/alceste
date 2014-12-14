using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Alceste.Plugin.Utils;

namespace Alceste.Plugin.DataLoader.Ftp
{
    public class FtpController : AWebLoaderController<FtpWebRequest, FtpWebResponse, FtpControllerConfig>
    {
        public override string ProtocolCode
        {
            get { return "ftp"; }
        }

        public FtpController(FtpControllerConfig config)
            : base(config)
        {
        }

        public override FtpWebRequest CreateRequestDirectory(string filePath)
        {
            var request = CreateRequest(filePath);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            return request;
        }

        public override FtpWebRequest CreateRequest(Uri fileUri)
        {
            var request = base.CreateRequest(fileUri) as FtpWebRequest;

            request.EnableSsl = Сonfig.IsSsl;
            request.UseBinary = Сonfig.IsBinary;
            request.UsePassive = Сonfig.IsPassive;

            return request;
        }

        protected override DataLoaderExecutionResult resultByErrorCode(FtpWebResponse responceResult)
        {
            switch (responceResult.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy:
                case FtpStatusCode.ActionNotTakenFilenameNotAllowed:
                    return new DataLoaderExecutionResult(DataLoaderExecutionCode.FileNotFound);
                default:
                    return new DataLoaderExecutionResult(DataLoaderExecutionCode.AuthorisationFailed);
            }
        }

        public override FtpWebRequest CreateRequestDownload(string filePath)
        {
            throw new NotImplementedException();
        }

        public override IList<MemoryStream> GetFilesByMask(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
