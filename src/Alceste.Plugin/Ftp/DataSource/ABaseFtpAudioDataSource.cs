using System.IO;
using Alceste.Plugin.AudioController;
using Alceste.Plugin.Config;
using Alceste.Plugin.Ftp;

namespace Alceste.Plugin.DataSource
{
    public abstract class ABaseFtpAudioDataSource : ABaseAudioDataSource<FtpPluginConfig>
    {
        protected readonly FtpLoader LocalFtpLoader;

        public ABaseFtpAudioDataSource()
        {
            LocalFtpLoader = new FtpLoader(PluginConfig.FTP.Server, PluginConfig.FTP.Login, PluginConfig.FTP.Password, PluginConfig.FTP.IsFtps);
        }

        public override Stream GetMedia(string filepath, int channelNum)
        {
            return LocalFtpLoader.GetFileByFtpMask(filepath);
        }

        public override IAudioFileInfo GetMediaImage(string fileId, string filepath, int width, int height, int channelNum)
        {
            var streamItem = LocalFtpLoader.GetFileByFtpMask(filepath);
            var audioItem = AudioConverterController.Instance.GetAudioInfoWithImage(width, height, streamItem, filepath);
            audioItem.AudioFileId = fileId;
            audioItem.AudioFilePath = filepath;
            audioItem.ChannelNumber = channelNum;
            return audioItem;
        }
    }
}
