using System.Configuration;
namespace Alceste.Plugin.Config.Element
{
    public class DbElement : ConfigurationElement
    {
        private const string DbTypeKey = "dbtype";
        [ConfigurationProperty(DbTypeKey, DefaultValue = "", IsKey = true, IsRequired = true)]
        public string DbType
        {
            get { return (string)this[DbTypeKey]; }
            set { this[DbTypeKey] = value; }
        }

        private const string ServerKey = "server";
        [ConfigurationProperty(ServerKey, DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Server
        {
            get { return (string)this[ServerKey]; }
            set { this[ServerKey] = value; }
        }

        private const string LoginKey = "login";
        [ConfigurationProperty(LoginKey, DefaultValue = "", IsRequired = true)]
        public string Login
        {
            get { return (string)this[LoginKey]; }
            set { this[LoginKey] = value; }
        }

        private const string PasswordKey = "password";
        [ConfigurationProperty(PasswordKey, DefaultValue = "", IsRequired = true)]
        public string Password
        {
            get { return (string)this[PasswordKey]; }
            set { this[PasswordKey] = value; }
        }

        private const string TitleKey = "title";
        [ConfigurationProperty(TitleKey, DefaultValue = "", IsRequired = false)]
        public string Title
        {
            get { return (string)this[TitleKey]; }
            set { this[TitleKey] = value; }
        }

        private const string PortKey = "port";
        [ConfigurationProperty(PortKey, DefaultValue = "", IsRequired = false)]
        public string Port
        {
            get { return (string)this[PortKey]; }
            set { this[PortKey] = value; }
        }

        private const string TableKey = "table";
        [ConfigurationProperty(TableKey, DefaultValue = "", IsRequired = false)]
        public string Table
        {
            get { return (string)this[TableKey]; }
            set { this[TableKey] = value; }
        }

        private const string KeyColumnKey = "keycolumn";
        [ConfigurationProperty(KeyColumnKey, DefaultValue = "", IsRequired = false)]
        public string KeyColumn
        {
            get { return (string)this[KeyColumnKey]; }
            set { this[KeyColumnKey] = value; }
        }

        private const string PathColumnKey = "pathcolumn";
        [ConfigurationProperty(PathColumnKey, DefaultValue = "", IsRequired = false)]
        public string PathColumn
        {
            get { return (string)this[PathColumnKey]; }
            set { this[PathColumnKey] = value; }
        }
    }
}
