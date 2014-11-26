using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.ServiceModel.Web;
using Alceste.Model;
using Alceste.Plugin.AudioController.InputFileFormat;


namespace Alceste.Plugin.DataSource
{
    public abstract class ABaseFtpTemplateDataSource<TConnection, TCommand, TDbDataReader, TDbParamener, TDbException> : ABaseFtpAudioDataSource
        where TConnection : DbConnection, new()
        where TCommand : DbCommand, new()
        where TDbDataReader : DbDataReader
        where TDbParamener : DbParameter, new()
        where TDbException : DbException
    {
        public string ConnectionString { get; protected set; }
        public string KeyColumn { get; protected set; }

        public string SqlCmdGetList { get; protected set; }
        public string SqlCmdGetInfo { get; protected set; }

        public string FileIdCode { get; protected set; }

        public TConnection CreateConnection()
        {
            FileIdCode = "@FileId";
            return new TConnection { ConnectionString = ConnectionString };
        }

        public override List<MediaFileServerRecord> GetFilesList()
        {
            var loadedItems = new List<MediaFileServerRecord>();
            var cmd = new TCommand();
            var connection = CreateConnection();
            cmd.CommandText = SqlCmdGetList;
            cmd.Connection = connection;
            TDbDataReader reader = null;
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                reader = cmd.ExecuteReader() as TDbDataReader;
                while (reader.Read() && reader[KeyColumn] != DBNull.Value)
                {
                    loadedItems.Add(ParseMediaFileServerRecord(reader));
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                connection.Close();
            }
            return loadedItems;
        }

        public override List<IAudioDataInfo> GetInfo(string fileId)
        {
            var audioFileInfo = new List<IAudioDataInfo>();
            var connection = CreateConnection();
            var cmd = new TCommand();
            cmd.CommandText = SqlCmdGetInfo;
            cmd.Parameters.Add(GetSqlParameter(FileIdCode, fileId));
            cmd.Connection = connection;
            TDbDataReader reader = null;
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                reader = cmd.ExecuteReader() as TDbDataReader;

                if (reader.Read())
                {
                    audioFileInfo = ParseAudioFileInfo(reader, fileId);
                }
            }
            catch (TDbException ex)
            {
                throw new WebFaultException<string>("SQL-сервер недоступен.", HttpStatusCode.BadRequest);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                connection.Close();
            }
            return audioFileInfo;
        }

        public abstract MediaFileServerRecord ParseMediaFileServerRecord(DbDataReader dbDataReader);

        public abstract List<IAudioDataInfo> ParseAudioFileInfo(DbDataReader dbDataReader, string fileId);

        public static TDbParamener GetSqlParameter(string command, string value)
        {
            return new TDbParamener
            {
                ParameterName = command,
                Value = value
            };
        }
    }
}
