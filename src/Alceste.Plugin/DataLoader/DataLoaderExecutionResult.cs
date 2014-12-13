using System.IO;

namespace Alceste.Plugin.DataLoader
{
    public class DataLoaderExecutionResult<T>
    {
        public T ResultItem { get; private set; }
        public DataLoaderExecutionCode ExecutionCode { get; private set; }

        public DataLoaderExecutionResult(T item,
                                            DataLoaderExecutionCode executionCode = DataLoaderExecutionCode.Ok)
        {
            ResultItem = item;
            ExecutionCode = executionCode;
        }

        public DataLoaderExecutionResult(DataLoaderExecutionCode executionCode = DataLoaderExecutionCode.FileNotFound)
        {
            ResultItem = default(T);
            ExecutionCode = executionCode;
        }
    }

    public enum DataLoaderExecutionCode
    {
        Ok,
        FileNotFound,
        ServerNotAccessible,
        AuthorisationFailed
    }

    public class DataLoaderExecutionResult : DataLoaderExecutionResult<MemoryStream>
    {

        public DataLoaderExecutionResult(MemoryStream item,
                                            DataLoaderExecutionCode executionCode = DataLoaderExecutionCode.Ok)
            : base(item, executionCode)
        {
        }

        public DataLoaderExecutionResult(DataLoaderExecutionCode executionCode = DataLoaderExecutionCode.FileNotFound)
            : base(executionCode)
        {
        }
    }
}
