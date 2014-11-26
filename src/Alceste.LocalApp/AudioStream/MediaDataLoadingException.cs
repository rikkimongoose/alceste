using System;
namespace Alceste.LocalApp.AudioStream
{
    public class MediaDataLoadingException : Exception
    {
        public MediaDataLoadingException()
        {
        }

        public MediaDataLoadingException(string message)
            : base(message)
        {
        }

        public MediaDataLoadingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
