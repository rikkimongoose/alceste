using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alceste.Plugin.Config;

namespace Alceste.Plugin.DataSource
{
    public abstract class AFileDataSource : ABaseAudioDataSource
    {
        protected FilePluginConfig filePluginConfig { get { return PluginConfig as FilePluginConfig; } }

        public override List<Model.MediaFileServerRecord> GetFilesList()
        {
            throw new NotImplementedException();
        }

        public override List<AudioController.InputFileFormat.IAudioDataInfo> GetInfo(string fileId)
        {
            throw new NotImplementedException();
        }
    }
}
