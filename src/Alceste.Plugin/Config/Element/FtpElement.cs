﻿using System.Configuration;
namespace Alceste.Plugin.Config.Element
{
    public class FtpElement : ConfigurationElement
    {
        private const string DbTypeKey = "server";
        [ConfigurationProperty(DbTypeKey, DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Server
        {
            get { return (string)this[DbTypeKey]; }
            set { this[DbTypeKey] = value; }
        }

        private const string LoginKey = "login";
        [ConfigurationProperty(LoginKey, DefaultValue = "", IsRequired = true)]
        public string Login
        {
            get { return (string)this[LoginKey]; }
            set { this[LoginKey] = value; }
        }

        private const string PasswordKey = "password";
        [ConfigurationProperty(PasswordKey, DefaultValue = "", IsRequired = false)]
        public string Password
        {
            get { return (string)this[PasswordKey]; }
            set { this[PasswordKey] = value; }
        }

        private const string IsFtpsKey = "ftps";
        [ConfigurationProperty(IsFtpsKey, DefaultValue = false, IsRequired = false)]
        public bool IsFtps
        {
            get { return (bool)this[IsFtpsKey]; }
            set { this[IsFtpsKey] = value; }
        }
    }
}
