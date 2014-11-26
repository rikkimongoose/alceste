using System;
using Alceste.Plugin.AudioController.InputFileFormat;

namespace Alceste.Plugin.AudioController
{
    public sealed class RawAudioFileInfo : IRawAudioFileInfo, IAudioDataInfo
    {
        public RawAudioFileInfo()
        {
            HighValues = null;
            LowValues = null;
        }

        public RawAudioFileInfo(int valuesCount)
        {
            HighValues = new float[valuesCount];
            LowValues = new float[valuesCount];
        }

        public string AudioFileId { get; set; }
        public string AudioFilePath { get; set; }

        public TimeSpan Length { get; set; }
        public string WaveFormat { get; set; }

        public float[] HighValues { get; set; }
        public float[] LowValues { get; set; }
        public int ChannelsCount { get; set; }
        public int ChannelNumber { get; set; }
    }
}
