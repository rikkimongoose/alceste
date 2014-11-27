using Alceste.RestService.AudioDataService.DataSource.Fake;
using Alceste.Plugin;
using Alceste.Plugin.Asterisk;
using Alceste.Plugin.CiscoWin;

namespace Alceste.RestService.AudioDataService
{
    public static class AudioItemsController
    {
        public static IAudioDataSourcePlugin GetFakeAudioSource()
        {
            return FakeAudioDataSource.Instance;
        }

        public static IAudioDataSourcePlugin GetAsteriskFTPSource()
        {
            return AsteriskFtpDataSource.Instance;
        }

        public static IAudioDataSourcePlugin GetCiscoWindowsFTPSource()
        {
            return CiscoWindowsFtpDataSource.Instance;
        }
    }
}