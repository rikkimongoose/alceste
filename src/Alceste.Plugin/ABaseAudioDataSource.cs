using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using Alceste.Model;
using Alceste.Plugin.AudioController;
using Alceste.Plugin.AudioController.InputFileFormat;
using Alceste.Plugin.Config;
using Alceste.Plugin.DataLoader;

namespace Alceste.Plugin
{
    public abstract class ABaseAudioDataSource : IAudioDataSourcePlugin
    {
        public virtual string DataSourceId { get { return string.Empty; } }
        public virtual string DataSourceTitle { get { return string.Empty; } }

        public DbPluginConfig PluginConfig { get; protected set; }
        protected ADataLoaderController LocalLoader { get; private set; }

        public ABaseAudioDataSource()
        {
            PluginConfig = ConfigurationManager.GetSection(DataSourceId) as DbPluginConfig;
            LocalLoader = DataLoaderGenerator.GetLoader(PluginConfig);

            if (LocalLoader == null)
                throw new WebFaultException<string>("Неизвестные формат настроек плагина", HttpStatusCode.BadRequest); 
        }

        public abstract List<MediaFileServerRecord> GetFilesList();

        public abstract List<IAudioDataInfo> GetInfo(string fileId);

        public Stream GetMediaImageStream(Image soundImage, ImageFormat imageFormat)
        {
            var mem = new MemoryStream();
            soundImage.Save(mem, imageFormat);
            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }

        public Stream GetMedia(string filepath, int channelNum)
        {
            return LocalLoader.GetFileByMask(filepath);
        }

        public IAudioFileInfo GetMediaImage(string fileId, string filepath, int width, int height, int channelNum)
        {
            var streamItem = LocalLoader.GetFileByMask(filepath);
            var audioItem = AudioConverterController.Instance.GetAudioInfoWithImage(width, height, streamItem, filepath);
            audioItem.AudioFileId = fileId;
            audioItem.AudioFilePath = filepath;
            audioItem.ChannelNumber = channelNum;
            return audioItem;
        }
    }
}
