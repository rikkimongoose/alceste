using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alceste.Plugin.AudioController.InputFileFormat
{
    public interface IAudioDataInfo
    {
        string AudioFileId { get; set; }

        string AudioFilePath { get; set; }

        int ChannelsCount { get; set; }
        int ChannelNumber { get; set; }

        TimeSpan Length { get; set; }
        string WaveFormat { get; set; }
    }
}
