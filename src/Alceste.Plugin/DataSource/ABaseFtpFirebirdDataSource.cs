using FirebirdSql.Data.FirebirdClient;

namespace Alceste.Plugin.DataSource
{
    public abstract class ABaseFtpFirebirdDataSource :
        ABaseFtpTemplateDataSource<FbConnection, FbCommand, FbDataReader, FbParameter, FbException>
    {
        public ABaseFtpFirebirdDataSource()
        {
            ConnectionString =
                string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};Port={4};Dialect=3;Charset=UTF8;",
                              PluginConfig.Database.Server, PluginConfig.Database.Title,
                              PluginConfig.Database.Login, PluginConfig.Database.Password, PluginConfig.Database.Port);
        }
    }
}
