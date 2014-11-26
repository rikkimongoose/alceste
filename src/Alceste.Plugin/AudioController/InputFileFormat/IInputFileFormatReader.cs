using System.IO;
using NAudio.Wave;

namespace Alceste.Plugin.AudioController.InputFileFormat
{
    public interface IInputFileFormatReader
    {
        string Name { get; }
        string Extension { get; }
        WaveStream CreateWaveStream(string fileName);
        WaveStream CreateWaveStream(Stream fileStream);
    }
}
