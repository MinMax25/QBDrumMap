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
            var builtInPath = $"{BuiltInFlagsDirectory}/{countryCode}.png";
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, builtInPath)))
            {
                return $"/{builtInPath}";
            }

            var languagePackPath = $"{LanguagePackDirectory}/{cultureName}/{countryCode}.png";
            return File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, languagePackPath))
                ? $"/{languagePackPath}"
                : null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
