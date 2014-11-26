using System.IO;
using NAudio.Wave;

namespace Alceste.Plugin.AudioController.InputFileFormat
{
    public sealed class AiffInputFileReader : IInputFileFormatReader
    {
        public string Name
        {
            get { return "AIFF File"; }
        }

        public string Extension
        {
            get { return AudioConverterController.MediaExtAiff; }
        }

        public WaveStream CreateWaveStream(string fileName)
        {
            return new AiffFileReader(fileName);
        }

        public WaveStream CreateWaveStream(Stream fileStream)
        {
            return new AiffFileReader(fileStream);
        }
    }
}
