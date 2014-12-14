using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private List<string> getFilesListByMask(string filepath)
        {
            var directory = Path.GetDirectoryName(filepath);
            var filename = Path.GetFileName(filepath);
            return new List<string>(Directory.GetFiles(directory, filename, SearchOption.AllDirectories));
        }

        public override MemoryStream GetFileByMask(string filepath)
        {
            if (!UtilsController.HasWildcards(filepath))
                return GetFileStream(filepath);
            var files = getFilesListByMask(filepath);
            if (files.Count <= 0)
                return null;
            return GetFileStream(files[0]);
        }

        public override IList<DataFileItem> GetFilesList(string filePath)
        {
            var files = Directory.GetFiles(filePath).ToList();
            return files.Select(file => 
                {
                    var fi = new System.IO.FileInfo(file);
                    return new DataFileItem { FileName = fi.Name, Date = fi.CreationTime, Size = fi.Length };
             }).ToList();
        }

        public override IList<MemoryStream> GetFilesByMask(string filepath)
        {
            if (!UtilsController.HasWildcards(filepath))
                return new List<MemoryStream>() { GetFileStream(filepath) };
            var files = getFilesListByMask(filepath);
            if (files.Count <= 0)
                return null;
            return files.Select(file => GetFileStream(file)).ToList();
        }

        public override bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }
    }
}
