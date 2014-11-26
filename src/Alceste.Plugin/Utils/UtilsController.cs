using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Alceste.Model;
using Alceste.Plugin.AudioController.InputFileFormat;
using Alceste.Plugin.Ftp;

namespace Alceste.Plugin.Utils
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

        public static MediaFileItem AudioFileInfoToMediaFileItem(IAudioDataInfo audioDataInfoNew)
        {
            if (audioDataInfoNew == null)
                return null;
            return new MediaFileItem
            {
                FileId = audioDataInfoNew.AudioFileId,
                Length = (int)Math.Floor(audioDataInfoNew.Length.TotalSeconds),
                WaveFormat = audioDataInfoNew.WaveFormat
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
            var index = filepath.LastIndexOf("/", StringComparison.InvariantCulture);
            if (index == -1)
                return filepath;
            return filepath.Substring(0, index);
        }

        public static string GetFileNameFromPath(string filepath)
        {
            var index = filepath.LastIndexOf("/", StringComparison.InvariantCulture);
            if (index == -1)
                return filepath;
            return filepath.Substring(index + 1);
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
                string[] itemFields = item.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                DateTime date;
                long size;
                string fileName;

                int fieldDate = itemFields.Length - 4;
                int fieldTime = itemFields.Length - 3;
                int fieldSize = itemFields.Length - 2;
                int fieldFileName = itemFields.Length - 1;

                DateTime.TryParse(string.Format("{0} {1}", itemFields[fieldDate], itemFields[fieldTime]), out date);
                long.TryParse(itemFields[fieldSize], out size);
                fileName = itemFields[fieldFileName];

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

        public static bool HasWildcards(string mask)
        {
            return mask.IndexOf("*", StringComparison.InvariantCulture) > -1 || mask.IndexOf("?", StringComparison.InvariantCulture) > -1;
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
