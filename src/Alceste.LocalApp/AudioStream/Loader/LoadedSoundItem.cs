using System;
using System.Windows.Media.Imaging;

namespace Alceste.LocalApp.AudioStream.Loader
{
    public sealed class LoadedSoundItem
    {
        public BitmapImage Image { get; set; }
        public TimeSpan Duration { get; set; }

        public string ChannelId { get; set; }
    }
}
