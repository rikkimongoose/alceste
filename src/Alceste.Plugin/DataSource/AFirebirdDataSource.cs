using FirebirdSql.Data.FirebirdClient;

namespace Alceste.Plugin.DataSource
{
    public abstract class AFirebirdDataSource :
        ADbTemplateDataSource<FbConnection, FbCommand, FbDataReader, FbParameter, FbException>
    {
        public AFirebirdDataSource()
        {
            ConnectionString =
                string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};Port={4};Dialect=3;Charset=UTF8;",
                              PluginConfig.Database.Server, PluginConfig.Database.Title,
                              PluginConfig.Database.Login, PluginConfig.Database.Password, PluginConfig.Database.Port);
        }
    }
}
