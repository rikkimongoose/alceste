using System.IO;
using NAudio.Wave;

namespace Alceste.Plugin.AudioController.InputFileFormat
{
    public sealed class Mp3InputFileReader : IInputFileFormatReader
    {
        public string Name
        {
            get { return "MP3 File"; }
        }

        public string Extension
        {
            get { return AudioConverterController.MediaExtMp3; }
        }

        public WaveStream CreateWaveStream(string fileName)
        {
            return new Mp3FileReader(fileName);
        }

        public WaveStream CreateWaveStream(Stream fileStream)
        {
            return new Mp3FileReader(fileStream);
        }
    }
}
