using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;
using Alceste.Model;

namespace Alceste.LocalApp.AudioStream.Loader
{

    public class AudioItemsListLoader : IAudioItemsListLoader
    {
        public readonly string AudioItemsListPathString;
        private readonly JavaScriptSerializer _jsonSerializer;

        public AudioItemsListLoader(string itemsListPath)
        {
            AudioItemsListPathString = itemsListPath;
            _jsonSerializer = new JavaScriptSerializer();
        }

        public IList<MediaFileServerRecord> GetMediaFileServerRecords()
        {
            IList<MediaFileServerRecord> mediaFileServerRecords = null;
            using (var client = new WebClient())
            {
                try
                {
                    var json = client.DownloadString(AudioItemsListPathString);
                    mediaFileServerRecords = _jsonSerializer.Deserialize<List<MediaFileServerRecord>>(json);
                }
                catch (WebException ex)
                {
                    throw new MediaDataLoadingException(string.Format("Не удаётся загрузить список файлов с адреса {0}",
                                                                AudioItemsListPathString));
                }
            }
            return mediaFileServerRecords;
        }
    }
}
