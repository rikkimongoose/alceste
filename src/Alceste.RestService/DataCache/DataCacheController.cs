using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Alceste.DAL;
using Alceste.DAL.DataTypes;
using Alceste.Plugin.AudioController;
using Alceste.Plugin.AudioController.InputFileFormat;
using Alceste.Plugin.Utils;
namespace Alceste.RestService.DataCache
{
    public static class DataCacheController
    {
        private const string RemoveAllFromTable = "TRUNCATE TABLE {0}";
        private static readonly string[] TablesToClear = { "CachedItemImages", "CachedItems" };

        public static List<IAudioDataInfo> GetCachedData(string mediaItemId)
        {
            List<CachedItem> mediaItems;
            using (var ctx = new Context())
            {
                mediaItems = ctx.CachedItems.Where(item => item.FileItemId == mediaItemId).ToList();
            }
            var result = new List<IAudioDataInfo>();
            mediaItems.ForEach(mediaItem => CachedItemToAudioFileInfo(mediaItemId, mediaItem));
            return result; ;
        }

        public static IAudioDataInfo GetCachedDataByChannel(string mediaItemId, int channelNumber = 1)
        {
            CachedItem mediaItem;
            using (var ctx = new Context())
            {
                mediaItem = ctx.CachedItems.FirstOrDefault(item => item.FileItemId == mediaItemId && item.ChannelNumber == channelNumber);
            }
            return CachedItemToAudioFileInfo(mediaItemId, mediaItem);
        }

        private static AudioFileInfo CachedItemToAudioFileInfo(string mediaItemId, CachedItem mediaItem)
        {
            if (mediaItem == null)
                return null;
            return new AudioFileInfo
            {
                AudioFileId = mediaItemId,
                AudioFilePath = mediaItem.FilePath,
                ChannelNumber = mediaItem.ChannelNumber,
                ChannelsCount = mediaItem.ChannelsCount,
                WaveFormat = mediaItem.WaveFormat,
                Length =
                    mediaItem.Length.HasValue
                        ? new TimeSpan(0, 0, (int)Math.Floor(mediaItem.Length.Value))
                        : TimeSpan.Zero
            };
        }

        public static IAudioFileInfo GetCachedData(string mediaItemId, int width, int height, int channelNum = 1)
        {
            CachedItem mediaItem;
            CachedItemImage image;
            using (var ctx = new Context())
            {
                mediaItem = ctx.CachedItems.FirstOrDefault(item => item.FileItemId == mediaItemId && item.ChannelNumber == channelNum);

                if (mediaItem == null || mediaItem.Images == null)
                    return null;

                image = mediaItem.Images.FirstOrDefault(img => img.MediaImageWidth == width &&
                                                               img.MediaImageHeight == height);
            }
            if (image == null)
                return null;
            var audioFileInfo = CachedItemToAudioFileInfo(mediaItemId, mediaItem);
            audioFileInfo.SoundImage = image.MediaImage != null
                                           ? UtilsController.ByteArrayToObject(image.MediaImage) as Image
                                           : null;
            return audioFileInfo;
        }

        public static void CreateCacheItem(IAudioFileInfo audioFileInfo)
        {
            var cachedItem = ConvertToCachedItem(audioFileInfo);

            var cachedItemImage = ConvertToNewCachedItemImage(audioFileInfo);

            if (cachedItemImage != null)
            {
                cachedItem.Images.Add(cachedItemImage);
            }

            using (var ctx = new Context())
            {
                ctx.CachedItems.Add(cachedItem);
                ctx.SaveChanges();
            }
        }

        public static void CreateCacheItem(IAudioDataInfo audioFileInfo)
        {
            var cachedItem = ConvertToCachedItem(audioFileInfo);

            using (var ctx = new Context())
            {
                ctx.CachedItems.Add(cachedItem);
                ctx.SaveChanges();
            }
        }

        private static CachedItem ConvertToCachedItem(IAudioDataInfo audioFileInfo)
        {
            return new CachedItem
            {
                FileItemId = audioFileInfo.AudioFileId,
                Length = audioFileInfo.Length != TimeSpan.Zero ? Math.Floor(audioFileInfo.Length.TotalSeconds) : 0,
                FilePath = audioFileInfo.AudioFilePath,
                WaveFormat = audioFileInfo.WaveFormat,
                ChannelNumber = audioFileInfo.ChannelNumber,
                ChannelsCount = audioFileInfo.ChannelsCount,
                Images = new Collection<CachedItemImage>()
            };
        }

        private static void UpdateCachedItem(CachedItem oldItem, CachedItem newItem, bool forceUpdate = false)
        {
            if (oldItem != null)
            {
                if (!forceUpdate && newItem.Length != null)
                    oldItem.Length = newItem.Length;

                if (!forceUpdate && newItem.FilePath != null)
                    oldItem.FilePath = newItem.FilePath;

                if (!forceUpdate && newItem.WaveFormat != null)
                    oldItem.WaveFormat = newItem.WaveFormat;

                if (!forceUpdate && newItem.ChannelsCount != 0)
                    oldItem.ChannelNumber = newItem.ChannelsCount;

                if (!forceUpdate && newItem.ChannelNumber != 0)
                    oldItem.ChannelNumber = newItem.ChannelNumber;
            }
        }

        private static CachedItemImage ConvertToNewCachedItemImage(IAudioImageInfo audioFileInfo)
        {
            if (audioFileInfo.SoundImage == null)
                return null;

            return new CachedItemImage
            {
                MediaImage = UtilsController.ObjectToByteArray(audioFileInfo.SoundImage),
                MediaImageHeight = audioFileInfo.ImageHeight,
                MediaImageWidth = audioFileInfo.ImageWidth
            };
        }

        public static void UpdateCacheItem(IAudioDataInfo audioFileInfo, bool forceUpdate = false)
        {
            CachedItem mediaItemOld;
            using (var ctx = new Context())
            {
                mediaItemOld = ctx.CachedItems.FirstOrDefault(item => item.FileItemId == audioFileInfo.AudioFileId && item.ChannelNumber == audioFileInfo.ChannelNumber);

                if (mediaItemOld != null)
                {
                    var cachedItem = ConvertToCachedItem(audioFileInfo);

                    if (cachedItem != null)
                    {
                        UpdateCachedItem(mediaItemOld, cachedItem);

                        var audioFileWithImageInfo = audioFileInfo as IAudioFileInfo;
                        if (audioFileWithImageInfo != null)
                        {
                            var mediaItemImage = ConvertToNewCachedItemImage(audioFileWithImageInfo);

                            if (mediaItemOld.Images == null)
                            {
                                mediaItemOld.Images = new Collection<CachedItemImage>();
                            }

                            var cachedItemImage =
                                mediaItemOld.Images.FirstOrDefault(
                                    img => img.MediaImageWidth == audioFileWithImageInfo.ImageWidth &&
                                           img.MediaImageHeight == audioFileWithImageInfo.ImageHeight);
                            if (cachedItemImage == null)
                            {
                                mediaItemOld.Images.Add(mediaItemImage);
                            }
                            else if (forceUpdate)
                            {
                                cachedItemImage.MediaImage =
                                    UtilsController.ObjectToByteArray(audioFileWithImageInfo.SoundImage);
                            }
                        }
                    }
                }
                ctx.SaveChanges();
            }
            if (mediaItemOld == null)
            {
                CreateCacheItem(audioFileInfo);
            }
        }

        public static void Clear()
        {
            using (var ctx = new Context())
            {
                foreach (var tableToClear in TablesToClear)
                    ctx.Database.ExecuteSqlCommand(string.Format(RemoveAllFromTable, tableToClear));
                ctx.SaveChanges();
            }
        }
    }
}
