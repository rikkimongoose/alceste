using Npgsql;

namespace Alceste.Plugin.DataSource
{
    public abstract class APostgreDataSource
        : ADbTemplateDataSource<NpgsqlConnection, NpgsqlCommand, NpgsqlDataReader, NpgsqlParameter, NpgsqlException>
    {
        private const string PostgreDefaultPort = "5432";

        public APostgreDataSource()
        {
            ConnectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
                                                 PluginConfig.Database.Server, PluginConfig.Database.Title,
                                                 PluginConfig.Database.Login, PluginConfig.Database.Password,
                                                 ((!string.IsNullOrEmpty(PluginConfig.Database.Port)) ?
                                                    PluginConfig.Database.Port :
                                                    PostgreDefaultPort
                                                 ));
        }
    }
}
