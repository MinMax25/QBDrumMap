using System.IO;
using System.Text.RegularExpressions;

namespace QBDrumMap.Class.Extensions
{
    public static class StringExtensions
    {
        #region Methods

        #region General

        // ワイルドカードを用いたパターンマッチング
        public static bool Like(this string target, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return true;
            }

            if (target == null)
            {
                target = string.Empty;
            }

            string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";

            return Regex.IsMatch(target, regexPattern, RegexOptions.IgnoreCase);
        }

        // ファイル名に使用できない文字を置換して安全なパスを返す
        public static string ToSafeFilePath(this string fullPath, char replacement = '_')
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return string.Empty;
            }

            int lastSlashIndex = fullPath.LastIndexOf('\\');

            string directory = lastSlashIndex >= 0 ? fullPath.Substring(0, lastSlashIndex + 1) : string.Empty;
            string fileName = lastSlashIndex >= 0 ? fullPath.Substring(lastSlashIndex + 1) : fullPath;

            if (string.IsNullOrEmpty(fileName))
            {
                return fullPath;
            }

            var invalidChars = Path.GetInvalidFileNameChars();

            // スラッシュ等を含めた無効な文字リストの作成
            var combinedInvalidChars = invalidChars.Concat(new[] { '/' }).ToArray();

            string safeFileName = string.Concat(
                fileName.Select(c =>
                {
                    return combinedInvalidChars.Contains(c) ? replacement : c;
                })
            );

            return directory + safeFileName;
        }

        // パス区切り文字を現在のOS標準に統一する
        public static string FilePathFix(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return path.Replace('/', Path.DirectorySeparatorChar)
                       .Replace('\\', Path.DirectorySeparatorChar);
        }

        #endregion

        #endregion
    }
}