using System.IO;
using System.Text.RegularExpressions;

namespace QBDrumMap.Class.Extentions
{
    public static class StringExtensions
    {
        public static bool Like(this string target, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return true;
            if (target == null) target = string.Empty;

            string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";

            return Regex.IsMatch(target, regexPattern, RegexOptions.IgnoreCase);
        }

        public static string ToSafeFilePath(this string fullPath, char replacement = '_')
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;

            int lastSlashIndex = fullPath.LastIndexOf('\\');

            string directory = lastSlashIndex >= 0 ? fullPath.Substring(0, lastSlashIndex + 1) : string.Empty;
            string fileName = lastSlashIndex >= 0 ? fullPath.Substring(lastSlashIndex + 1) : fullPath;

            if (string.IsNullOrEmpty(fileName))
                return fullPath;

            var invalidChars = Path.GetInvalidFileNameChars();

            invalidChars = invalidChars.Concat(new[] { '/' }).ToArray();

            string safeFileName = string.Concat(
                fileName.Select(c => invalidChars.Contains(c) ? replacement : c)
            );

            return directory + safeFileName;
        }

        public static string FilePathFix(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}