using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using Alceste.Model;
using Alceste.Plugin.AudioController.InputFileFormat;
using Alceste.Plugin.DataSource;
using Alceste.Plugin.Utils;

namespace Alceste.Plugin.Asterisk
{

    public sealed class AsteriskFtpDataSource : ABaseFtpMySqlDataSource
    {
        public override string DataSourceId { get { return "AsteriskFTP"; } }
        public override string DataSourceTitle { get { return "Asterisk by FTP"; } }

        public const string ColumnDuration = "duration";
        public const string ColumnCallDate = "calldate";

        public const string AsteriskFtpWaveFormat = "128 WAV PCM";

        #region Singletone
        private static AsteriskFtpDataSource _instance;

        public static IAudioDataSourcePlugin Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AsteriskFtpDataSource();
                return _instance;
            }
        }

        private AsteriskFtpDataSource()
        {
            KeyColumn = PluginConfig.Database.KeyColumn;
            SqlCmdGetList = string.Format("SELECT {0}, {1} FROM {2} WHERE lastapp=\"Queue\" AND recordingfile != \"\";", KeyColumn, ColumnDuration, PluginConfig.Database.Table);
            SqlCmdGetInfo = string.Format(
                "SELECT {0}, {1}, {3}, {4} FROM {2} WHERE lastapp=\"Queue\" AND recordingfile != \"\" AND {0} = @FileId;",
                PluginConfig.Database.KeyColumn, PluginConfig.Database.PathColumn, PluginConfig.Database.Table, ColumnDuration, ColumnCallDate);
        }
        #endregion
        public override MediaFileServerRecord ParseMediaFileServerRecord(DbDataReader dbDataReader)
        {
            return new MediaFileServerRecord
            {
                Title = dbDataReader.GetString(dbDataReader.GetOrdinal(KeyColumn)),
                Length = (int)UtilsController.DurationToPCM(dbDataReader.GetString(dbDataReader.GetOrdinal(ColumnDuration))).TotalSeconds
            };
        }

        public override List<IAudioDataInfo> ParseAudioFileInfo(DbDataReader dbDataReader, string fileId)
        {
            List<IAudioDataInfo> audioDataInfoList;
            audioDataInfoList = new List<IAudioDataInfo>();
            var filePath = dbDataReader.GetString(dbDataReader.GetOrdinal(PluginConfig.Database.PathColumn));
            var fileDate = dbDataReader.GetDateTime(dbDataReader.GetOrdinal(ColumnCallDate));
            var datePrefix = fileDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            var filePathWithDate = string.Format("{0}/{1}", datePrefix, filePath);
            var fileExt = filePath.Substring(filePath.LastIndexOf(".", StringComparison.InvariantCulture));
            var filePathIn = filePath.Replace(fileExt, string.Format("-in{0}", fileExt));
            var filePathOut = filePath.Replace(fileExt, string.Format("-out{0}", fileExt));
            var filePathInWithDate = string.Format("{0}/{1}", datePrefix, filePathIn);
            var filePathOutWithDate = string.Format("{0}/{1}", datePrefix, filePathOut);

            var soundLength = UtilsController.DurationToPCM(
                dbDataReader.GetString(dbDataReader.GetOrdinal(ColumnDuration)));

            TryAddAudioDataToList(audioDataInfoList, fileId, filePath, soundLength);
            TryAddAudioDataToList(audioDataInfoList, fileId, filePathWithDate, soundLength);
            TryAddAudioDataToList(audioDataInfoList, fileId, filePathIn, soundLength);
            TryAddAudioDataToList(audioDataInfoList, fileId, filePathOut, soundLength, 2);
            TryAddAudioDataToList(audioDataInfoList, fileId, filePathInWithDate, soundLength);
            TryAddAudioDataToList(audioDataInfoList, fileId, filePathOutWithDate, soundLength, 2);

            audioDataInfoList.ForEach(item => item.ChannelsCount = audioDataInfoList.Count);

            return audioDataInfoList;
        }

        private void TryAddAudioDataToList(List<IAudioDataInfo> audioDataInfoList, string fileId, string filePath, TimeSpan length, int channelNum = 1)
        {

            if (LocalFtpLoader.IsExistFileFTP(filePath))
            {
                audioDataInfoList.Add(new AsteriskAudioFileInfo
                {
                    AudioFileId = fileId,
                    AudioFilePath = filePath,
                    Length = length,
                    WaveFormat = AsteriskFtpWaveFormat,
                    ChannelNumber = channelNum
                });
            }
        }
    }
}
