using System.Data.SqlClient;

namespace Alceste.Plugin.DataSource
{
    public abstract class AMsSqlDataSource :
        ADbTemplateDataSource<SqlConnection, SqlCommand, SqlDataReader, SqlParameter, SqlException>
    {
        public AMsSqlDataSource()
        {
            ConnectionString =
                (string.IsNullOrEmpty(PluginConfig.Database.Port)) ?
                    string.Format("Server={0};Database={1};User Id={2};Password={3};",
                                  PluginConfig.Database.Server, PluginConfig.Database.Title,
                                  PluginConfig.Database.Login, PluginConfig.Database.Password) :
                    string.Format("Server={0},{4};Database={1};User Id={2};Password={3};",
                                  PluginConfig.Database.Server, PluginConfig.Database.Title,
                                  PluginConfig.Database.Login, PluginConfig.Database.Password, PluginConfig.Database.Port);
        }
    }
}
