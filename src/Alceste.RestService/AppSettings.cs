using System.Configuration;

namespace Alceste.RestService
{
    public static class AppSettings
    {
        public const string CallCenterDriverKey = "CallCenterDriver";

        private static string getByKey(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string CallCenterDriver
        {
            get { return getByKey(CallCenterDriverKey); }
        }
    }
}
