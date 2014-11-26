using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Alceste.Plugin.AudioController.InputFileFormat;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
namespace Alceste.Plugin.AudioController
{
    public sealed class AudioConverterController
    {
        public const string MediaExtMp3 = ".mp3";
        public const string MediaExtWav = ".wav";
        public const string MediaExtAiff = ".aiff";

        private AudioConverterController()
        {
            _inputFileFormatReaders.Add(new WaveInputFileReader());
            _inputFileFormatReaders.Add(new Mp3InputFileReader());
            _inputFileFormatReaders.Add(new AiffInputFileReader());

            SoundGraphBackgroundColor = Color.Black;
            SoundGraphColor = Color.Green;
        }

        private readonly List<IInputFileFormatReader> _inputFileFormatReaders = new List<IInputFileFormatReader>();

        private IInputFileFormatReader GetReaderForFile(string fileName)
        {
            return _inputFileFormatReaders.FirstOrDefault(f => fileName.EndsWith(f.Extension, StringComparison.OrdinalIgnoreCase));
        }

        public Color SoundGraphBackgroundColor { get; set; }
        public Color SoundGraphColor { get; set; }

        public AudioFileInfo GetAudioInfoWithImage(int width, int height, string filePath)
        {
            var rawAudioFileInfo = GetImageRawData(width, filePath);
            return RawAudioFileInfoToAudioFileInfo(rawAudioFileInfo, width, height);
        }

        public AudioFileInfo GetAudioInfoWithImage(int width, int height, Stream fileStream, string filePath)
        {
            var rawAudioFileInfo = GetImageRawData(width, fileStream, filePath);
            return RawAudioFileInfoToAudioFileInfo(rawAudioFileInfo, width, height);
        }

        private AudioFileInfo RawAudioFileInfoToAudioFileInfo(RawAudioFileInfo rawAudioFileInfo, int width, int height)
        {
            var audioFileInfo = GetAudioFileInfo(rawAudioFileInfo);
            audioFileInfo.SoundImage = DrawImageForSound(width, height, rawAudioFileInfo);
            return audioFileInfo;
        }

        public AudioFileInfo GetAudioInfo(string filepath)
        {
            var rawAudioFileInfo = GetImageRawData(filepath);
            var audioFileInfo = GetAudioFileInfo(rawAudioFileInfo);
            return audioFileInfo;
        }

        private RawAudioFileInfo GetImageRawData(int length, string filePath)
        {
            RawAudioFileInfo rawAudioFileInfo;
            using (var waveStream = OpenWaveStream(filePath))
            {
                rawAudioFileInfo = ReadImageRawData(length, waveStream);
            }
            return rawAudioFileInfo;
        }

        private RawAudioFileInfo GetImageRawData(int length, Stream fileStream, string fileName)
        {
            RawAudioFileInfo rawAudioFileInfo;
            using (var waveStream = OpenWaveStream(fileStream, fileName))
            {
                rawAudioFileInfo = ReadImageRawData(length, waveStream);
            }
            return rawAudioFileInfo;
        }

        private RawAudioFileInfo GetImageRawData(string filePath)
        {
            var rawAudioFileInfo = new RawAudioFileInfo();
            using (var waveStream = OpenWaveStream(filePath))
            {
                GetAudioInfoFromWaveStream(rawAudioFileInfo, waveStream);
            }
            return rawAudioFileInfo;
        }

        private WaveStream OpenWaveStream(string filePath)
        {
            var reader = GetReaderForWaveStream(filePath);
            var waveStream = reader.CreateWaveStream(filePath);
            if (waveStream == null)
                throw new Exception(string.Format("Unable to stream file \"{0}\"", filePath));
            return waveStream;
        }

        private WaveStream OpenWaveStream(Stream fileStream, string fileExt)
        {
            var reader = GetReaderForWaveStream(fileExt);
            var waveStream = reader.CreateWaveStream(fileStream);
            if (waveStream == null)
                throw new Exception(string.Format("Unable to create stream file."));
            return waveStream;
        }

        private IInputFileFormatReader GetReaderForWaveStream(string fileExt)
        {
            var reader = GetReaderForFile(fileExt);
            if (reader == null)
                throw new Exception(string.Format("Unable to load a reader for file \"{0}\"", fileExt));
            return reader;
        }

        private RawAudioFileInfo ReadImageRawData(int length, WaveStream waveStream)
        {
            var rawAudioFileInfo = new RawAudioFileInfo(length);

            var waveChannel = new SampleChannel(waveStream, true);

            int bytesPerSample = (waveStream.WaveFormat.BitsPerSample / 8) * waveChannel.WaveFormat.Channels;
            int samplesPerPixel = (int)waveStream.Length / (length * bytesPerSample);
            int blockSize = samplesPerPixel * bytesPerSample;

            waveStream.Position = 0;
            var waveData = new byte[blockSize];

            for (int x = 0; x < length; x++)
            {
                short low = 0;
                short high = 0;
                int bytesRead = waveStream.Read(waveData, 0, blockSize);
                if (bytesRead == 0)
                    break;
                for (int n = 0; n < bytesRead; n += 2)
                {
                    short sample = BitConverter.ToInt16(waveData, n);
                    if (sample < low) low = sample;
                    if (sample > high) high = sample;
                }
                float lowPercent = ((((float)low) - short.MinValue) / ushort.MaxValue);
                float highPercent = ((((float)high) - short.MinValue) / ushort.MaxValue);
                rawAudioFileInfo.LowValues[x] = lowPercent;
                rawAudioFileInfo.HighValues[x] = highPercent;
            }

            GetAudioInfoFromWaveStream(rawAudioFileInfo, waveStream);

            return rawAudioFileInfo;
        }

        private void GetAudioInfoFromWaveStream(IAudioDataInfo audioFileInfo, WaveStream stream)
        {
            audioFileInfo.Length = stream.TotalTime;
            audioFileInfo.WaveFormat = stream.WaveFormat.ToString();
        }

        private Image DrawImageForSound(int width, int height, IRawAudioFileInfo fileInfo)
        {
            var soundImage = new Bitmap(width, height);

            using (var g = Graphics.FromImage(soundImage))
            {
                g.Clear(SoundGraphBackgroundColor);
                var pen = new Pen(SoundGraphColor);

                int i = 0;
                while (i < Math.Max(fileInfo.HighValues.Length, width))
                {
                    g.DrawLine(pen, i, height * fileInfo.LowValues[i], i, height * fileInfo.HighValues[i]);
                    i++;
                }
            }
            return soundImage;
        }

        private AudioFileInfo GetAudioFileInfo(RawAudioFileInfo rawAudioFileInfo)
        {
            return new AudioFileInfo { Length = rawAudioFileInfo.Length, WaveFormat = rawAudioFileInfo.WaveFormat };
        }


        private static AudioConverterController _instance;
        private static readonly object _syncRoot = new object();

        public static AudioConverterController Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            var temp = new AudioConverterController();
                            System.Threading.Thread.MemoryBarrier();
                            _instance = temp;
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
