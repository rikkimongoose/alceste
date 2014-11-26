using System;
using System.Collections.Generic;

namespace Alceste.LocalApp.AudioStream.Loader
{

    public class AudioStreamLoaderFactory : IAudioStreamLoaderFactory
    {
        public readonly string SoundPathTemplate;
        public readonly string InfoPathTemplate;
        public readonly string SoundPicPathTemplate;

        public const int DefaultMediaTimerUpdateFrequency = 100;

        public AudioStreamLoaderFactory(string soundPathTemplate, string infoPath, string soundPicPath)
        {
            SoundPathTemplate = soundPathTemplate;
            InfoPathTemplate = infoPath;
            SoundPicPathTemplate = soundPicPath;

            MediaTimerUpdateFrequency = DefaultMediaTimerUpdateFrequency;
        }

        public int MediaTimerUpdateFrequency { get; set; }

        public void GetLoaderForId(out IAudioStreamLoader _audioStreamLoader, string id, Action<object, List<LoadedSoundItem>> OnLoadingCompleted)
        {
            _audioStreamLoader = new AudioStreamLoader(SoundPathTemplate, InfoPathTemplate, SoundPicPathTemplate, id, MediaTimerUpdateFrequency);

            if (OnLoadingCompleted != null)
            {
                _audioStreamLoader.OnSoundItemsLoadedCompleted += (sender, list) => OnLoadingCompleted(sender, list);
            }

            _audioStreamLoader.LoadInfoItems();
        }
    }
}
