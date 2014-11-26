using System.Drawing;

namespace Alceste.Plugin.AudioController
{
    public interface IAudioImageInfo
    {
        Image SoundImage { get; set; }

        int ImageWidth { get; }
        int ImageHeight { get; }
    }
}
