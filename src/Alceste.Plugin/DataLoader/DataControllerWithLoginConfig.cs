using System;

namespace Alceste.Plugin.DataLoader
{
    public abstract class DataControllerWithLoginConfig : DataControllerConfig
    {
        public string Server { get; set; }

        public bool IsCredentials { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
