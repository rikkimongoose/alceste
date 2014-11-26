using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Alceste.Model;
using Alceste.Plugin.AudioController;
using Alceste.Plugin.AudioController.InputFileFormat;

namespace Alceste.Plugin
{
    public class IAudioDataSourcePlugin
    {
        string DataSourceId { get; }
        string DataSourceTitle { get; }

        List<MediaFileServerRecord> GetFilesList();
        Stream GetMedia(string fileId, int channel);
        IAudioFileInfo GetMediaImage(string fileId, string fileName, int width, int height, int channel);
        Stream GetMediaImageStream(Image soundImage, ImageFormat imageFormat);

        List<IAudioDataInfo> GetInfo(string fileId);
    }
}
