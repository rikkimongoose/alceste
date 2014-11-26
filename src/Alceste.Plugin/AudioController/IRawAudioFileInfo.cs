namespace Alceste.Plugin.AudioController
{
    public interface IRawAudioFileInfo
    {
        float[] HighValues { get; set; }
        float[] LowValues { get; set; }
    }
}
