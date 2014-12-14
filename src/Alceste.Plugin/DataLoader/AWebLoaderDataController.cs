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
    public abstract class AWebLoaderController<TWebRequest, TWebResponce, TDataControllerConfig> : ADataLoaderController<TDataControllerConfig>
        where TWebRequest : WebRequest
        where TWebResponce : WebResponse
        where TDataControllerConfig : DataControllerWithLoginConfig
    {
        public AWebLoaderController(TDataControllerConfig config) : base(config)
        {
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
        }

        public TWebRequest CreateRequest(string filePath)
        {
            var request = CreateRequest(ServerToUri(Сonfig.Server, filePath));
            if (Сonfig.IsCredentials)
                request.Credentials = getCredentials(Сonfig.Username, Сonfig.Password);
            return request;
        }

        public virtual DataLoaderExecutionResult DownloadFileByMask(string filePath)
        {
            if (!UtilsController.HasWildcards(filePath))
                return GetFile(filePath);
            var dirUrl = UtilsController.GetDirUrlFromPath(filePath);
            var fileNameMask = UtilsController.GetFileNameFromPath(filePath);
            var files = GetDirectoryList(filePath);
            if (files.ExecutionCode != DataLoaderExecutionCode.Ok || files.ResultItem == null)
                return new DataLoaderExecutionResult(files.ExecutionCode);
            var fileItem = files.ResultItem.FirstOrDefault(item => UtilsController.СompareWithWildcards(item.FileName, fileNameMask));
            var fileItemFullPath = string.Format("{0}/{1}", dirUrl, fileItem);
            var request = CreateRequestDownload(filePath);
            return ExecuteRequest(request);
        }

        public virtual DataLoaderExecutionResult<List<MemoryStream>> DownloadFilesByMask(string filePath)
        {
            if (!UtilsController.HasWildcards(filePath))
            {
                var result = GetFile(filePath);
                if (result.ExecutionCode != DataLoaderExecutionCode.Ok || result.ResultItem == null)
                    return new DataLoaderExecutionResult<List<MemoryStream>>(result.ExecutionCode);
                return new DataLoaderExecutionResult<List<MemoryStream>>(new List<MemoryStream> { result.ResultItem }, result.ExecutionCode);
            }

            var dirUrl = UtilsController.GetDirUrlFromPath(filePath);
            var fileNameMask = UtilsController.GetFileNameFromPath(filePath);
            var files = GetDirectoryList(filePath);
            if (files.ExecutionCode != DataLoaderExecutionCode.Ok || files.ResultItem == null)
                return new DataLoaderExecutionResult<List<MemoryStream>>(files.ExecutionCode);
            var fileItems = files.ResultItem.Select(item => UtilsController.СompareWithWildcards(item.FileName, fileNameMask)).ToList();
            var streamResults = new List<MemoryStream>();
            foreach(var fileItem in fileItems)
            {
                var fileItemFullPath = string.Format("{0}/{1}", dirUrl, fileItem);
                var request = CreateRequestDownload(filePath);
                var requestResult = ExecuteRequest(request);
                if (requestResult.ExecutionCode == DataLoaderExecutionCode.Ok)
                    streamResults.Add(requestResult.ResultItem);
            }
            return new DataLoaderExecutionResult<List<MemoryStream>>(streamResults);
        }

        public virtual DataLoaderExecutionResult<IList<DataFileItem>> GetDirectoryList(string filePath)
        {
            var request = CreateRequestDirectory(filePath);
            var dirListStream = ExecuteRequest(request);
            if (dirListStream.ExecutionCode != DataLoaderExecutionCode.Ok || dirListStream.ResultItem == null)
                return new DataLoaderExecutionResult<IList<DataFileItem>>(dirListStream.ExecutionCode);
            var filesStr = UtilsController.GetListFromStream(dirListStream.ResultItem);
            dirListStream.ResultItem.Close();
            return new DataLoaderExecutionResult<IList<DataFileItem>>(UtilsController.ParseFtpFileItems(filesStr));
        }

        public override IList<DataFileItem> GetFilesList(string filePath)
        {
            var filesList = GetDirectoryList(filePath);
            return filesList.ResultItem;
        }

        public abstract TWebRequest CreateRequestDownload(string filePath);
        public abstract TWebRequest CreateRequestDirectory(string filePath);

        public DataLoaderExecutionResult GetFile(string filePath)
        {
            var request = CreateRequestDownload(filePath);
            return ExecuteRequest(request);
        }

        protected virtual DataLoaderExecutionResult ExecuteRequest(TWebRequest request)
        {
            TWebResponce response = null;
            MemoryStream memResult = null;
            try
            {
                response = request.GetResponse() as TWebResponce;
                var responseStream = response.GetResponseStream();
                memResult = UtilsController.ReadToMemory(responseStream);
            }
            catch (WebException ex)
            {
                var responceResult = ex.Response as TWebResponce;
                return resultByErrorCode(responceResult);
            }
            catch (FileNotFoundException ex)
            {
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            return new DataLoaderExecutionResult(memResult);
        }

        protected abstract DataLoaderExecutionResult resultByErrorCode(TWebResponce responceResult);

        private ICredentials getCredentials(string username, string password)
        {
            return new NetworkCredential(username, password); ;
        }

        private Uri ServerToUri(string server, string filepath)
        {
            return new Uri(getBaseUri(server), filepath);
        }

        protected virtual Uri getBaseUri(string server)
        {
            return new Uri(string.Format("{0}://{1}", ProtocolCode, server));
        }

        public abstract string ProtocolCode { get; }

        public virtual TWebRequest CreateRequest(Uri fileUri)
        {
            var request = WebRequest.Create(fileUri) as TWebRequest;
            return request;
        }

        public override MemoryStream GetFileStream(string filepath)
        {
            var streamItem = ExecuteFunc(UtilsController.PrepareFTPFileName(filepath), GetFile);
            return streamItem.ResultItem;
        }

        public override MemoryStream GetFileByMask(string filepath)
        {
            var streamItem = ExecuteFunc(UtilsController.PrepareFTPFileName(filepath), DownloadFileByMask);
            return streamItem.ResultItem;
        }

        public override bool FileExists(string filePath)
        {
            var streamItem = ExecuteFunc(UtilsController.PrepareFTPFileName(filePath), GetFile);
            if (streamItem.ResultItem == null)
                return false;

            streamItem.ResultItem.Close();

            return streamItem.ExecutionCode == DataLoaderExecutionCode.Ok;
        }

        protected DataLoaderExecutionResult ExecuteFunc(string filePath, Func<string, DataLoaderExecutionResult> doAction)
        {
            var streamItem = doAction(filePath);
            if (streamItem.ExecutionCode == DataLoaderExecutionCode.AuthorisationFailed)
                throw new WebFaultException<string>("Ошибка авторизации.", HttpStatusCode.BadRequest);
            if (streamItem.ExecutionCode == DataLoaderExecutionCode.ServerNotAccessible)
                throw new WebFaultException<string>("Сервер со звуковыми файлами недоступен.", HttpStatusCode.BadRequest);
            if (streamItem.ExecutionCode != DataLoaderExecutionCode.Ok || streamItem.ResultItem == null)
                throw new WebFaultException<string>("Не удаётся загрузить файл.", HttpStatusCode.BadRequest);

            return streamItem;
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
    }
}
