using System.IO;
using NAudio.Wave;

namespace Alceste.Plugin.AudioController.InputFileFormat
{
    public sealed class WaveInputFileReader : IInputFileFormatReader
    {
        public string Name
        {
            get { return "WAV file"; }
        }

        public string Extension
        {
            get { return AudioConverterController.MediaExtWav; }
        }

        public WaveStream CreateWaveStream(string fileName)
        {
            return GetWaveStream(new WaveFileReader(fileName));
        }

        public WaveStream CreateWaveStream(Stream fileStream)
        {
            return GetWaveStream(new WaveFileReader(fileStream));
        }

        private WaveStream GetWaveStream(WaveStream readerStream)
        {
            if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
            }
            return readerStream;
        }
    }
}
