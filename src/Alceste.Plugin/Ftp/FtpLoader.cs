using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Web;
using Alceste.Plugin.Utils;

namespace Alceste.Plugin.Ftp
{
    public sealed class FtpLoader
    {
        public string ServerName { get; private set; }
        public string ServerLogin { get; private set; }
        public string ServerPassword { get; private set; }
        public bool IsFtps { get; private set; }

        public FtpLoader(string serverName, string serverLogin, string serverPassword, bool isFtps)
        {
            ServerName = serverName;
            ServerLogin = serverLogin;
            ServerPassword = serverPassword;
            IsFtps = isFtps;
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
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

        public MemoryStream GetFileByFtp(string filepath)
        {
            var streamItem = ExecuteFTPFunc(UtilsController.PrepareFTPFileName(filepath), FtpController.GetFileByFTP);
            return streamItem;
        }

        public MemoryStream GetFileByFtpMask(string filepath)
        {
            var streamItem = ExecuteFTPFunc(UtilsController.PrepareFTPFileName(filepath), FtpController.GetFileByFTPMask);
            return streamItem;
        }

        public bool IsExistFileFTP(string filepath)
        {
            var streamItem = FtpController.GetFileByFTP(ServerName, ServerPassword,
                                                  ServerPassword, UtilsController.PrepareFTPFileName(filepath), IsFtps);
            if (streamItem.ResultItem == null)
                return false;

            streamItem.ResultItem.Close();

            return streamItem.ExecutionCode == FTPControllerExecutionCode.Ok;
        }

        private MemoryStream ExecuteFTPFunc(string filePath, Func<string, string, string, string, bool, FtpControllerExecutionResult> ftpAction)
        {
            var streamItem = ftpAction(ServerName, ServerLogin,
                                                  ServerPassword, filePath, IsFtps);
            if (streamItem.ExecutionCode == FTPControllerExecutionCode.AuthorisationFailed)
                throw new WebFaultException<string>("Ошибка авторизации.", HttpStatusCode.BadRequest);
            if (streamItem.ExecutionCode == FTPControllerExecutionCode.ServerNotAccessible)
                throw new WebFaultException<string>("Сервер со звуковыми файлами недоступен.", HttpStatusCode.BadRequest);
            if (streamItem.ExecutionCode != FTPControllerExecutionCode.Ok || streamItem.ResultItem == null)
                throw new WebFaultException<string>("Не удаётся загрузить файл.", HttpStatusCode.BadRequest);

            return streamItem.ResultItem;
        }
    }
}
