using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Media.Imaging;
using System.Timers;
using Alceste.Model;

namespace Alceste.LocalApp.AudioStream.Loader
{
    public class AudioStreamLoader : IAudioStreamLoader
    {
        /// <summary>
        /// Compare image size with the following observational error.
        /// </summary>
        private const double EPS = 1;

        private readonly JavaScriptSerializer _jsonSerializer;

        public readonly string Id;

        public List<LoadedSoundItem> Channels;

        internal AudioStreamLoader(string soundPath, string infoPath, string soundPicPath, string id,
                                   int mediaTimerUpdateFrequency)
        {
            _jsonSerializer = new JavaScriptSerializer();
            Channels = new List<LoadedSoundItem>();

            Id = id;

            SoundPath = string.Format(soundPath, id);
            InfoPath = string.Format(infoPath, id);
            SoundPicPathTemplate = soundPicPath;

            SoundUrl = SoundPath;
        }

        public void LoadInfoItems()
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString(InfoPath);
                var mediaFileItemInfoItems = _jsonSerializer.Deserialize<List<MediaFileItem>>(json);
                mediaFileItemInfoItems.ForEach(item => Channels.Add(new LoadedSoundItem
                {
                    ChannelId = item.ChannelId,
                    Duration = (item.Length == 0) ? TimeSpan.Zero : new TimeSpan(0, 0, 0, item.Length),
                }));
            }
            if (OnSoundItemsLoadedCompleted != null)
                OnSoundItemsLoadedCompleted(this, Channels);
        }

        public string SoundPath { get; private set; }
        public string InfoPath { get; private set; }
        public string SoundPicPathTemplate { get; private set; }

        public TimeSpan LoadDuration(string channelNum)
        {
            var channel = Channels.FirstOrDefault(item => item.ChannelId == channelNum);
            if (channel == null)
                return TimeSpan.Zero;
            return channel.Duration;
        }

        public BitmapImage LoadImage(int width, int height, string channelNum)
        {
            return DownloadImage(width, height, channelNum);
        }

        public string SoundUrl { get; private set; }

        public string LoadSoundUrl(string channelNum)
        {
            return ConvertChannelNumber(SoundUrl, channelNum);
        }

        public Timer MediaTimer { get; private set; }
        public event EventHandler<List<LoadedSoundItem>> OnSoundItemsLoadedCompleted;

        private BitmapImage DownloadImage(int width, int height, string channelNum)
        {
            var pathToImage = ConvertChannelNumber(string.Format(SoundPicPathTemplate, Id, width, height), channelNum);
            return new BitmapImage(new Uri(pathToImage));
        }

        private string ConvertChannelNumber(string url, string channelNum)
        {
            return url.Replace("%channelnum%", channelNum);
        }
    }
}
