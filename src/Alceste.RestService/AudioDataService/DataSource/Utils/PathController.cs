using System;
using System.IO;
using System.Web.Hosting;

namespace Alceste.RestService.AudioDataService.DataSource.Utils
{
    public static class PathController
    {
        public const string WrongPathExceptionText = "Путь \"{0}\" не существует.";

        public static string GetPath(string mediaPath)
        {
            var path = HostingEnvironment.MapPath(mediaPath);
            if (path == null)
                throw new Exception(string.Format(WrongPathExceptionText, mediaPath));
            return path;
        }

        public static string GetPathCombined(string mediaPath, string filePath)
        {
            var path = GetPath(mediaPath);
            return Path.Combine(path, filePath);
        }
    }
}
