using System.Collections.Generic;
using Alceste.Model;

namespace Alceste.LocalApp.AudioStream.Loader
{
    public interface IAudioItemsListLoader
    {
        IList<MediaFileServerRecord> GetMediaFileServerRecords();
    }
}
