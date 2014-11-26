using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using Alceste.AudioDataService;
using Alceste.Model;
using Alceste.DataCache;
using Alceste.DataControllers;
using Alceste.Plugin;
using Alceste.Plugin.AudioController.InputFileFormat;

namespace Alceste.WCFService
{
    public class MediaServiceController
    {
        public IAudioDataSourcePlugin _audioDataSource;
        public ImageFormat _imageFormat;

        private const int DefaultImageWidth = 500;
        private const int DefaultImageHeight = 150;

        public const string AsteriskFTPConfig = "AsteriskFTP";
        public const string CiscoWinFTPConfig = "CiscoWinFTP";
        public const string FakeConfig = "Fake";

        public MediaServiceController()
        {
            switch (AppSettings.CallCenterDriver)
            {
                case AsteriskFTPConfig:
                    _audioDataSource = AudioItemsController.GetAsteriskFTPSource();
                    break;
                case CiscoWinFTPConfig:
                    _audioDataSource = AudioItemsController.GetCiscoWindowsFTPSource();
                    break;
                case FakeConfig:
                    _audioDataSource = AudioItemsController.GetFakeAudioSource();
                    break;
                default:
                    throw new WebFaultException<string>("Не найден драйвер поддержки колл-центра. Проверьте параметр CallCenterDriver в настройках сервера.", HttpStatusCode.BadRequest);
            }

            _imageFormat = ImageFormat.Png;
        }

        public List<MediaFileServerRecord> GetFilesList()
        {
            return _audioDataSource.GetFilesList();
        }

        public Stream GetMedia(string fileId, string channel)
        {
            int channelNum = 1;
            int.TryParse(channel, out channelNum);
            var info = loadAudioDataInfo(fileId, channelNum);
            return _audioDataSource.GetMedia(info.AudioFilePath, channelNum);
        }

        private IAudioDataInfo loadAudioDataInfo(string fileId, int channelNum)
        {
            var infoCashed = DataCacheController.GetCachedDataByChannel(fileId, channelNum);
            if (infoCashed != null)
                return infoCashed;

            var infoItems = _audioDataSource.GetInfo(fileId);
            int i = 0;
            infoItems.ForEach(infoItem =>
            {
                infoItem.ChannelNumber = ++i;
                infoItem.ChannelsCount = infoItems.Count;
                DataCacheController.UpdateCacheItem(infoItem);
            });
            return infoItems.FirstOrDefault(infoInChannel => infoInChannel.ChannelNumber == channelNum);
        }

        public Stream GetMediaImage(string fileId, string widthStr, string heightStr, string channel)
        {
            int width, height;

            int channelNum = 1;
            int.TryParse(channel, out channelNum);

            if (!int.TryParse(widthStr, out width) || width == 0)
                width = DefaultImageWidth;
            if (!int.TryParse(heightStr, out height) || height == 0)
                height = DefaultImageHeight;

            //var time = DateTime.Now;
            var soundImage = LoadMediaImage(fileId, width, height, channelNum);

            //var timeSpan = DateTime.Now - time;

            if (soundImage == null)
                return null;

            if (WebOperationContext.Current == null)
                return null;

            var memStream = _audioDataSource.GetMediaImageStream(soundImage, _imageFormat);

            var ctx = WebOperationContext.Current.OutgoingResponse;
            ctx.ContentType = ImageMIMETypeController.Instance.GetKeyByValue(_imageFormat);
            ctx.ContentLength = memStream.Length;
            return memStream;
        }

        private Image LoadMediaImage(string fileId, int width, int height, int channelNum)
        {
            var mediaInfo = DataCacheController.GetCachedData(fileId, width, height, channelNum);

            Image soundImage = null;

            if (mediaInfo != null && mediaInfo.SoundImage != null)
            {
                soundImage = mediaInfo.SoundImage;
            }
            else
            {
                var mediaBaseInfo = GetAudioInfo(fileId, channelNum);
                var fileName = fileId;
                if (mediaBaseInfo != null)
                {
                    fileName = mediaBaseInfo.AudioFilePath;
                }
                mediaInfo = _audioDataSource.GetMediaImage(fileId, fileName, width, height, channelNum);
                mediaInfo.ChannelsCount = mediaBaseInfo.ChannelsCount;
                mediaInfo.WaveFormat = mediaBaseInfo.WaveFormat;
                if (mediaInfo != null)
                {
                    soundImage = mediaInfo.SoundImage;
                    DataCacheController.UpdateCacheItem(mediaInfo);
                }
            }
            return soundImage;
        }

        public List<MediaFileItem> GetInfo(string fileId)
        {
            var info = DataCacheController.GetCachedData(fileId);
            if (info == null || info.Count == 0)
            {
                info = _audioDataSource.GetInfo(fileId);
                info.ForEach(infoItem => DataCacheController.UpdateCacheItem(infoItem));
            }
            var mediaFileItems = new List<MediaFileItem>();
            info.ForEach(item => mediaFileItems.Add(UtilsController.AudioFileInfoToMediaFileItem(item)));
            return mediaFileItems;
        }

        private IAudioDataInfo GetAudioInfo(string fileId, int channelNumber)
        {
            var info = DataCacheController.GetCachedDataByChannel(fileId, channelNumber);
            if (info == null)
            {
                info = loadAudioDataInfo(fileId, channelNumber);
                if (info == null)
                    throw new WebFaultException<string>("На сервере нет элемента с указанным индексом.", HttpStatusCode.BadRequest);
                DataCacheController.UpdateCacheItem(info);
            }
            return info;
        }
    }
}
