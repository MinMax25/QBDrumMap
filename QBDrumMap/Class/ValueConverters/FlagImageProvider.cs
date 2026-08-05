#nullable enable

using System.Globalization;
using System.IO;

namespace QBDrumMap.Class.ValueConverters;

// 組み込み言語はResources/Flags、外部言語パックはLanguages/{culture}から国旗を解決する。
public static class FlagImageProvider
{
    private const string BuiltInFlagsDirectory = "Resources/Flags";
    private const string LanguagePackDirectory = "Languages";

    public static string? Resolve(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return null;
        }

        try
        {
            var countryCode = new RegionInfo(cultureName).TwoLetterISORegionName.ToLowerInvariant();
            var builtInPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                BuiltInFlagsDirectory,
                $"{countryCode}.png");
            if (File.Exists(builtInPath))
            {
                return builtInPath;
            }

            var languagePackDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                LanguagePackDirectory,
                cultureName);
            var languagePackPath = Path.Combine(languagePackDirectory, $"{countryCode}.png");
            if (File.Exists(languagePackPath))
            {
                return languagePackPath;
            }

            return Directory.Exists(languagePackDirectory)
                ? Directory.EnumerateFiles(languagePackDirectory, "*.png", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault()
                : null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
