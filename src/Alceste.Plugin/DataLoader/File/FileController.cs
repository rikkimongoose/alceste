using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Alceste.Plugin.Utils;

namespace Alceste.Plugin.DataLoader.File
{
    public sealed class FileController : ADataLoaderController<FileControllerConfig>
    {
        public FileController(FileControllerConfig config)
            : base(config)
        {
        }

        public override MemoryStream GetFileStream(string filepath)
        {
            if (!FileExists(filepath))
                return null;

            var ms = new MemoryStream();
            using (var file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                file.CopyTo(ms);
                file.Close();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public override MemoryStream GetFileByMask(string filepath)
        {
            if (!UtilsController.HasWildcards(filepath))
                return GetFileStream(filepath);
            var directory = Path.GetDirectoryName(filepath);
            var filename = Path.GetFileName(filepath);
            var files = Directory.GetFiles(directory, filename, SearchOption.AllDirectories);
            if(files.Length > 0)
                return null;
            return GetFileStream(files[0]);
        }

        public override bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }
    }
}
