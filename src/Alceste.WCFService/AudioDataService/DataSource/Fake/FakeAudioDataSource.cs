using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Alceste.WCFService.AudioDataService.DataSource.Utils;
using Alceste.Model;
using Alceste.Plugin;
using Alceste.Plugin.AudioController;
using Alceste.Plugin.AudioController.InputFileFormat;
using Alceste.Plugin.Config;

namespace Alceste.WCFService.AudioDataService.DataSource.Fake
{
    public sealed class FakeAudioDataSource : ABaseAudioDataSource<LocalFilePluginConfig>
    {
        public override string DataSourceId { get { return "Fake"; } }
        public override string DataSourceTitle { get { return "Fake Data Source (example one)"; } }

        private readonly string MediaPath;

        #region Singleton

        private static FakeAudioDataSource _instance;

        public static IAudioDataSourcePlugin Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FakeAudioDataSource();
                return _instance;
            }
        }

        private FakeAudioDataSource()
        {
            MediaPath = PluginConfig.LocalDirectory.Path;
            ConvertedImageFormat = ImageFormat.Png;
        }

        #endregion

        public ImageFormat ConvertedImageFormat { get; set; }


        public override List<MediaFileServerRecord> GetFilesList()
        {
            return
                Directory.GetFiles(PathController.GetPath(MediaPath), "*.*", SearchOption.AllDirectories)
                         .Select(item => new MediaFileServerRecord { Title = Path.GetFileName(item) })
                         .ToList();
        }

        public override Stream GetMedia(string filePath, int channelNumber)
        {
            var path = PathController.GetPathCombined(MediaPath, filePath);
            return File.OpenRead(path);
        }

        public override IAudioFileInfo GetMediaImage(string fileId, string fileName, int width, int height, int channelNumber)
        {
            var path = PathController.GetPathCombined(MediaPath, fileId);
            var audioItem = AudioConverterController.Instance.GetAudioInfoWithImage(width, height, path);
            audioItem.AudioFileId = fileId;
            audioItem.AudioFilePath = fileName;
            audioItem.ChannelNumber = channelNumber;
            return audioItem;
        }

        public override List<IAudioDataInfo> GetInfo(string fileId)
        {
            var resultList = new List<IAudioDataInfo>();
            var path = PathController.GetPathCombined(MediaPath, fileId);
            var audioInfo = AudioConverterController.Instance.GetAudioInfo(path);
            if (audioInfo == null)
                return null;
            audioInfo.AudioFileId = fileId;
            audioInfo.AudioFilePath = fileId;
            audioInfo.ChannelNumber = 1;
            audioInfo.ChannelsCount = 1;
            resultList.Add(audioInfo);
            return resultList;
        }
    }
}
