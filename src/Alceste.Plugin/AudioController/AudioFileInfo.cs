using System;
using System.Drawing;

namespace Alceste.Plugin.AudioController
{
    public class AudioFileInfo : IAudioFileInfo
    {
        public string AudioFileId { get; set; }

        public string AudioFilePath { get; set; }

        public int ChannelsCount { get; set; }
        public int ChannelNumber { get; set; }

        public TimeSpan Length { get; set; }
        public string WaveFormat { get; set; }
        public Image SoundImage { get; set; }

        public int ImageWidth { get { return SoundImage != null ? SoundImage.Width : 0; } }
        public int ImageHeight { get { return SoundImage != null ? SoundImage.Height : 0; } }
    }
}
