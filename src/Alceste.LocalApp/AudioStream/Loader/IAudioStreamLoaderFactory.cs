using System;
using System.Collections.Generic;
namespace Alceste.LocalApp.AudioStream.Loader
{
    public interface IAudioStreamLoaderFactory
    {
        int MediaTimerUpdateFrequency { get; set; }

        void GetLoaderForId(out IAudioStreamLoader _audioStreamLoader, string id, Action<object, List<LoadedSoundItem>> OnLoadingCompleted);
    }
}
