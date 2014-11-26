using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Alceste.Model;
using Alceste.Plugin.AudioController;
using Alceste.Plugin.AudioController.InputFileFormat;
using Alceste.Plugin.Config;

namespace Alceste.Plugin
{
    public abstract class ABaseAudioDataSource<T> : IAudioDataSourcePlugin where T : PluginConfig
    {
        public virtual string DataSourceId { get { return string.Empty; } }
        public virtual string DataSourceTitle { get { return string.Empty; } }

        public T PluginConfig { get; protected set; }

        public ABaseAudioDataSource()
        {
            PluginConfig = ConfigurationManager.GetSection(DataSourceId) as T;
        }

        public abstract List<MediaFileServerRecord> GetFilesList();

        public abstract Stream GetMedia(string fileId, int channel);
        public abstract IAudioFileInfo GetMediaImage(string fileId, string fileName, int width, int height, int channel);

        public abstract List<IAudioDataInfo> GetInfo(string fileId);

        public Stream GetMediaImageStream(Image soundImage, ImageFormat imageFormat)
        {
            var mem = new MemoryStream();
            soundImage.Save(mem, imageFormat);
            mem.Position = 0;
            return mem;
        }
    }
}
