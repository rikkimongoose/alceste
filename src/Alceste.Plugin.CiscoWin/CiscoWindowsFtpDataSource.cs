using System;
using System.Collections.Generic;
using System.Data.Common;
using Alceste.Model;
using Alceste.Plugin.AudioController.InputFileFormat;
using Alceste.Plugin.DataSource;
using Alceste.Plugin.Utils;

namespace Alceste.Plugin.CiscoWin
{
    public sealed class CiscoWindowsFtpDataSource : ABaseFtpFirebirdDataSource
    {
        public override string DataSourceId { get { return "CiscoWinFTP"; } }
        public override string DataSourceTitle { get { return "Cisco by FTP (Windows)"; } }

        public const string DurationStartColumn = "STARTDATE";
        public const string DurationEndColumn = "ENDDATE";

        #region Singletone
        private static CiscoWindowsFtpDataSource _instance;

        public static IAudioDataSourcePlugin Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CiscoWindowsFtpDataSource();
                return _instance;
            }
        }

        private CiscoWindowsFtpDataSource()
        {
            KeyColumn = PluginConfig.Database.KeyColumn;
            SqlCmdGetList = string.Format("SELECT {0}, {1} FROM {2};", KeyColumn, DurationEndColumn, PluginConfig.Database.Table);
            SqlCmdGetInfo = string.Format(
                "SELECT {0}, {1}, {2}, {3} FROM {4} WHERE {0} = @FileId;",
                PluginConfig.Database.KeyColumn, PluginConfig.Database.PathColumn, DurationStartColumn, DurationEndColumn, PluginConfig.Database.Table);
        }
        #endregion

        public override MediaFileServerRecord ParseMediaFileServerRecord(DbDataReader dbDataReader)
        {
            return new MediaFileServerRecord { Title = dbDataReader.GetString(dbDataReader.GetOrdinal(KeyColumn)) };
        }

        public override List<IAudioDataInfo> ParseAudioFileInfo(DbDataReader dbDataReader, string fileId)
        {
            var audioFileList = new List<IAudioDataInfo>();
            var audioFileInfo = new CiscoWindowsFtpAudioFileInfo { AudioFileId = fileId };
            var audioFilePath = dbDataReader.GetString(dbDataReader.GetOrdinal(PluginConfig.Database.PathColumn));
            audioFilePath = string.Format("{0}_*.mp3", audioFilePath);
            var endDate = dbDataReader.GetDateTime(dbDataReader.GetOrdinal(DurationEndColumn));
            if (dbDataReader[DurationStartColumn] != DBNull.Value && dbDataReader[DurationEndColumn] != DBNull.Value)
            {
                audioFileInfo.Length = UtilsController.StartEndToTimeSpan(dbDataReader.GetDateTime(dbDataReader.GetOrdinal(DurationStartColumn)), endDate);
            }
            var folderName = endDate.ToString("dd_MM_yyyy");
            audioFileInfo.AudioFilePath = string.Format("{0}/{1}", folderName, audioFilePath);
            audioFileList.Add(audioFileInfo);
            return audioFileList;
        }
    }
}
