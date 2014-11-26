using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Alceste.LocalApp.AudioStream.Loader
{
    public interface IAudioStreamLoader
    {
        string SoundPath { get; }
        string InfoPath { get; }
        string SoundPicPathTemplate { get; }

        void LoadInfoItems();

        TimeSpan LoadDuration(string channelNum);

        BitmapImage LoadImage(int width, int height, string channelNum);

        string LoadSoundUrl(string channelNum);

        event EventHandler<List<LoadedSoundItem>> OnSoundItemsLoadedCompleted;
    }
}
