using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alceste.Plugin.Ftp
{
    public class FtpControllerExecutionResult<T>
    {
        public T ResultItem { get; private set; }
        public FTPControllerExecutionCode ExecutionCode { get; private set; }

        public FtpControllerExecutionResult(T item,
                                            FTPControllerExecutionCode executionCode = FTPControllerExecutionCode.Ok)
        {
            ResultItem = item;
            ExecutionCode = executionCode;
        }

        public FtpControllerExecutionResult(FTPControllerExecutionCode executionCode = FTPControllerExecutionCode.FileNotFound)
        {
            ResultItem = default(T);
            ExecutionCode = executionCode;
        }
    }

    public enum FTPControllerExecutionCode
    {
        Ok,
        FileNotFound,
        ServerNotAccessible,
        AuthorisationFailed
    }

    public class FtpControllerExecutionResult : FtpControllerExecutionResult<MemoryStream>
    {

        public FtpControllerExecutionResult(MemoryStream item,
                                            FTPControllerExecutionCode executionCode = FTPControllerExecutionCode.Ok)
            : base(item, executionCode)
        {
        }

        public FtpControllerExecutionResult(FTPControllerExecutionCode executionCode = FTPControllerExecutionCode.FileNotFound)
            : base(executionCode)
        {
        }
    }
}
