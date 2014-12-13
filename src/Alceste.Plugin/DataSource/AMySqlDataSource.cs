using MySql.Data.MySqlClient;

namespace Alceste.Plugin.DataSource
{
    public abstract class AMySqlDataSource :
        ADbTemplateDataSource<MySqlConnection, MySqlCommand, MySqlDataReader, MySqlParameter, MySqlException>
    {
        public AMySqlDataSource()
        {
            ConnectionString = string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3}",
                                                 PluginConfig.Database.Server, PluginConfig.Database.Title,
                                                 PluginConfig.Database.Login, PluginConfig.Database.Password);
        }
    }
}
