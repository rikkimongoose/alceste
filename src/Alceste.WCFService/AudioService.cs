using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Alceste.Model;

namespace Alceste.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AudioService" in both code and config file together.
    public class AudioService : IAudioService
    {
        public MediaServiceController _mediaServiceController;

        public AudioService()
        {
            _mediaServiceController = new MediaServiceController();
        }
        public List<MediaFileServerRecord> getFilesList()
        {
            return _mediaServiceController.GetFilesList();
        }

        public Stream getMedia(string fileId, string channel)
        {
            return _mediaServiceController.GetMedia(fileId, channel);
        }

        public Stream getMediaImage(string fileId, string width, string height, string channel)
        {
            return _mediaServiceController.GetMediaImage(fileId, width, height, channel);
        }

        public List<MediaFileItem> getInfo(string fileId)
        {
            return _mediaServiceController.GetInfo(fileId);
        }
    }
}
