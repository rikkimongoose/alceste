using Oracle.ManagedDataAccess.Client;

namespace Alceste.Plugin.DataSource
{
    public abstract class AOracleDataSource :
        ADbTemplateDataSource<OracleConnection, OracleCommand, OracleDataReader, OracleParameter, OracleException>
    {
        public AOracleDataSource()
        {
            ConnectionString =
                    string.Format("Server={0};Data Source={1};User Id={2};Password={3};",
                                  PluginConfig.Database.Server, PluginConfig.Database.Title,
                                  PluginConfig.Database.Login, PluginConfig.Database.Password);
        }
    }
}
