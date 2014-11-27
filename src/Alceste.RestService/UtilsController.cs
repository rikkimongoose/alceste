using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Alceste.Model;
using Alceste.Plugin.AudioController.InputFileFormat;
using Alceste.Plugin.Ftp;

namespace Alceste.RestService
{
    public static class UtilsController
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            byte[] result = null;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                result = ms.ToArray();
            }
            return result;
        }

        public static object ByteArrayToObject(byte[] buff)
        {
            if (buff == null)
                return null;
            object result;
            using (var ms = new MemoryStream(buff))
            {
                var bf = new BinaryFormatter();
                result = bf.Deserialize(ms);
            }
            return result;
        }

        public static MediaFileItem AudioFileInfoToMediaFileItem(IAudioDataInfo audioDataInfo)
        {
            if (audioDataInfo == null)
                return null;
            return new MediaFileItem
                {
                    FileId = audioDataInfo.AudioFileId,
                    Length = (int)Math.Floor(audioDataInfo.Length.TotalSeconds),
                    WaveFormat = audioDataInfo.WaveFormat,
                    ChannelId = string.Format("{0}", audioDataInfo.ChannelNumber),
                    ChannelsCount = audioDataInfo.ChannelsCount
                };
        }

        public static List<string> GetListFromStream(MemoryStream memoryStream)
        {
            var fileList = new List<string>();

            memoryStream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(memoryStream))
            {
                while (!reader.EndOfStream)
                {
                    fileList.Add(reader.ReadLine());
                }
            }

            return fileList;
        }

        public static string GetDirUrlFromPath(string filepath)
        {
            return filepath.Substring(0, filepath.LastIndexOf("/", StringComparison.InvariantCulture));
        }

        public static string GetFileNameFromPath(string filepath)
        {
            return filepath.Substring(filepath.LastIndexOf("/", StringComparison.InvariantCulture) + 1);
        }

        public static string PrepareFTPFileName(string fileName)
        {
            return fileName.Replace(" ", string.Empty);
        }

        public static IList<FtpFileRecordItem> ParseFtpFileItems(List<string> filesStr)
        {
            var ftpFileRecordItems = new List<FtpFileRecordItem>();
            filesStr.ForEach(item =>
                {
                    string[] itemFields = item.Split(new []{" "}, StringSplitOptions.RemoveEmptyEntries);

                    DateTime date;
                    long size;
                    string fileName;

                    DateTime.TryParse(string.Format("{0} {1}", itemFields[0], itemFields[1]), out date);
                    long.TryParse(itemFields[2], out size);
                    fileName = itemFields[3];

                    ftpFileRecordItems.Add(new FtpFileRecordItem
                        {
                            Date = date,
                            Size = size,
                            FileName = fileName
                        });
                });
            return ftpFileRecordItems;
        }

        public static bool СompareWithWildcards(string strToCompare, string mask)
        {
            int i = 0, j = 0;
            bool isAsterisk = false;
            if (mask.Length > strToCompare.Length)
                return false; 
            for (i = 0; i < strToCompare.Length; i++)
            {
                if (mask[j] == '*')
                {
                    isAsterisk = true;
                }
                if (mask[j] == strToCompare[i])
                {
                    if (strToCompare[i] != '*')
                    {
                        isAsterisk = false;
                    }
                    j++;
                }
                else
                {
                    if (mask[j] == '?')
                    {
                        j++;
                    }
                    else if (!isAsterisk)
                    {
                        return false;
                    }
                }
            }
            return j <= i;
        }

        public static TimeSpan DurationToPCM(object durationStr)
        {
            if (durationStr != DBNull.Value)
            {
                int secs;
                if (int.TryParse(durationStr.ToString(), out secs))
                    return new TimeSpan(0, 0, 0, secs);
            }
            return TimeSpan.Zero;
        }

        public static TimeSpan StartEndToTimeSpan(DateTime startDateTime, DateTime endDateTime)
        {
            return endDateTime - startDateTime;
        }
    }
}
