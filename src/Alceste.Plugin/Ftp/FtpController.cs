using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Alceste.Plugin.Utils;

namespace Alceste.Plugin.Ftp
{
    public static class FtpController
    {
        public static FtpControllerExecutionResult GetFileByFTP(string server, string username, string password, string filepath, bool isSsl)
        {
            var request = CreateFTPRequestDownload(server, username, password, filepath, isSsl);
            return ExecuteFTPRequest(request);
        }

        public static FtpControllerExecutionResult GetFileByFTPMask(string server, string username, string password, string filepath, bool isSsl)
        {
            if (!UtilsController.HasWildcards(filepath))
                return GetFileByFTP(server, username, password, filepath, isSsl);

            var dirUrl = UtilsController.GetDirUrlFromPath(filepath);
            var fileNameMask = UtilsController.GetFileNameFromPath(filepath);

            var files = GetDirectoryList(server, username, password, dirUrl, isSsl);

            if (files.ExecutionCode != FTPControllerExecutionCode.Ok || files.ResultItem == null)
                return new FtpControllerExecutionResult(files.ExecutionCode);

            var fileItem = files.ResultItem.FirstOrDefault(item => UtilsController.СompareWithWildcards(item.FileName, fileNameMask));

            var fileItemFullPath = string.Format("{0}/{1}", dirUrl, fileItem);

            var request = CreateFTPRequestDownload(server, username, password, fileItemFullPath, isSsl);

            return ExecuteFTPRequest(request);
        }

        public static FtpControllerExecutionResult<IList<FtpFileRecordItem>> GetDirectoryList(string server, string username, string password,
                                                                string filepath, bool isSsl)
        {
            var request = CreateFTPRequestDirectory(server, username, password, filepath, isSsl);

            var dirListStream = ExecuteFTPRequest(request);
            if (dirListStream.ExecutionCode != FTPControllerExecutionCode.Ok || dirListStream.ResultItem == null)
                return new FtpControllerExecutionResult<IList<FtpFileRecordItem>>(dirListStream.ExecutionCode);

            var filesStr = UtilsController.GetListFromStream(dirListStream.ResultItem);
            dirListStream.ResultItem.Close();
            return new FtpControllerExecutionResult<IList<FtpFileRecordItem>>(UtilsController.ParseFtpFileItems(filesStr));
        }

        private static FtpControllerExecutionResult ExecuteFTPRequest(FtpWebRequest request)
        {
            FtpWebResponse response = null;
            MemoryStream memResult = null;
            try
            {
                response = request.GetResponse() as FtpWebResponse;
                var responseStream = response.GetResponseStream();
                memResult = ReadToMemory(responseStream);
            }
            catch (WebException ex)
            {
                var responceResult = ex.Response as FtpWebResponse;
                switch (responceResult.StatusCode)
                {
                    case FtpStatusCode.ActionNotTakenFileUnavailable:
                    case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy:
                    case FtpStatusCode.ActionNotTakenFilenameNotAllowed:
                        return new FtpControllerExecutionResult();
                    default:
                        return new FtpControllerExecutionResult(FTPControllerExecutionCode.AuthorisationFailed);
                }
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            return new FtpControllerExecutionResult(memResult);
        }

        private static MemoryStream ReadToMemory(Stream input)
        {
            var ms = new MemoryStream();
            input.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static FtpWebRequest CreateFTPRequestDownload(string server, string username, string password, string filepath, bool isSsl)
        {
            var request = CreateFTPRequest(server, username, password, filepath, isSsl);
            request.EnableSsl = isSsl;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            return request;
        }

        private static FtpWebRequest CreateFTPRequestDirectory(string server, string username, string password, string filepath, bool isSsl)
        {
            var request = CreateFTPRequest(server, username, password, filepath, isSsl);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            return request;
        }

        public static FtpWebRequest CreateFTPRequest(string server, string username, string password, string filepath, bool isSsl)
        {
            var baseUri = new Uri(string.Format("ftp://{0}", server));
            var fileUri = new Uri(baseUri, filepath);

            var request = WebRequest.Create(fileUri) as FtpWebRequest;

            request.UsePassive = true;
            request.UseBinary = true;
            request.EnableSsl = isSsl;

            request.Credentials = new NetworkCredential(username, password);
            return request;
        }
    }
}
