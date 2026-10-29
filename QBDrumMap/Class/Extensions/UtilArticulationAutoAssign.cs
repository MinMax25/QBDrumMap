using QBDrumMap.Class.MapModels;
using System.Text;

namespace QBDrumMap.Class.Extensions;

public static class UtilArticulationAutoAssign
{
    // パーツ名の語のうち、入力名に含まれている割合の下限
    private const double MinimumPartCoverage = 0.5;

    // 語尾違い（Choked / Choke など）を同一視する前置一致の最小長
    private const int MinimumPrefixLength = 4;

    // 非ドラム系の打楽器や制御用のノート。Articulationを割り当てない
    private static readonly string[] ExcludedWords = { "spiral", "chopper", "cowbell", "clap", "controller" };

    // パーツやArticulationの判別に寄与しない語。入力名から除いて照合する
    private static readonly string[] NoiseWords = { "drum", "cym", "cymbal", "center" };

    public static (int AssignedCount, int SkippedCount) AutoAssignArticulations(this MapData data, Kit kit, IEnumerable<PartNameAlias> dictionary)
    {
        if (kit == null)
        {
            return (0, 0);
        }

        var owningPlugin = data.Plugins.FirstOrDefault(p => p.Kits.Contains(kit));
        var replacements = BuildReplacements(dictionary);

        int assignedCount = 0;
        int skippedCount = 0;
        var newlyAssigned = new HashSet<KitPitch>();

        foreach (var pitch in kit.Pitches)
        {
            if (data.IsValidArticulationID(pitch.ArticulationID) || string.IsNullOrWhiteSpace(pitch.Name))
            {
                continue;
            }

            if (owningPlugin != null && IsGMFamily(owningPlugin.PluginType) && GMDrumMap.IsPercussionNote(pitch.Pitch))
            {
                pitch.Separator = Separator.Separate;
                continue;
            }

            int? articulationID = FindByOtherKitReference(data, kit, owningPlugin, pitch.Name, replacements)
                ?? FindByPartDictionary(data, pitch.Name, replacements);

            if (articulationID.HasValue)
            {
                pitch.ArticulationID = articulationID.Value;
                newlyAssigned.Add(pitch);
                assignedCount++;
            }
            else
            {
                pitch.Separator = Separator.Separate;
                skippedCount++;
            }
        }

        ResolveDuplicates(kit, newlyAssigned);

        return (assignedCount, skippedCount);
    }

    private static bool IsGMFamily(PluginType pluginType)
    {
        return pluginType == PluginType.GM
            || pluginType == PluginType.GM2
            || pluginType == PluginType.GS
            || pluginType == PluginType.XG;
    }

    private static int? FindByOtherKitReference(MapData data, Kit targetKit, Plugin owningPlugin, string pitchName, IReadOnlyList<(string Alias, string CanonicalName)> replacements)
    {
        string normalized = Normalize(pitchName, replacements);

        var candidates = data.Plugins
            .SelectMany(p => p.Kits.Select(k => new { Plugin = p, Kit = k }))
            .Where(x => x.Kit != targetKit)
            .SelectMany(x => x.Kit.Pitches.Select(p => new { x.Plugin, Pitch = p }))
            .Where(x => data.IsValidArticulationID(x.Pitch.ArticulationID) && !string.IsNullOrWhiteSpace(x.Pitch.Name))
            .Where(x => Normalize(x.Pitch.Name, replacements) == normalized)
            .ToArray();

        if (owningPlugin != null)
        {
            var samePlugin = candidates.FirstOrDefault(x => x.Plugin == owningPlugin);
            if (samePlugin != null)
            {
                return samePlugin.Pitch.ArticulationID;
            }
        }

        return candidates.FirstOrDefault()?.Pitch.ArticulationID;
    }

    private static int? FindByPartDictionary(MapData data, string pitchName, IReadOnlyList<(string Alias, string CanonicalName)> replacements)
    {
        var allTokens = Tokenize(Normalize(pitchName, replacements));
        if (allTokens.Any(t => ExcludedWords.Contains(t)))
        {
            return null;
        }

        var pitchTokens = allTokens.Where(t => !NoiseWords.Contains(t)).ToArray();
        if (pitchTokens.Length == 0)
        {
            return null;
        }

        var part = FindBestPart(data, pitchTokens, replacements);
        return part == null ? null : FindBestArticulation(part, pitchTokens, replacements)?.ID;
    }

    private static Part FindBestPart(MapData data, string[] pitchTokens, IReadOnlyList<(string Alias, string CanonicalName)> replacements)
    {
        Part best = null;
        MatchResult bestMatch = default;

        foreach (var part in data.Parts)
        {
            if (part.Articulations.Count == 0)
            {
                continue;
            }

            var partTokens = Tokenize(Normalize(part.Name, replacements));
            var match = CountMatches(pitchTokens, partTokens);

            // 入力名が長くても特定できるよう、パーツ名側の語がどれだけ入力名に含まれるかで判定する
            if (!match.HasWord || (double)match.CandidateMatched / partTokens.Length < MinimumPartCoverage)
            {
                continue;
            }

            // 一致数が同じなら、後ろの語に一致するパーツを優先する（Hi Hat Closed Pedal は Pedal 側）
            if (best == null
                || match.Matched > bestMatch.Matched
                || (match.Matched == bestMatch.Matched && match.IndexSum > bestMatch.IndexSum)
                || (match.Matched == bestMatch.Matched && match.IndexSum == bestMatch.IndexSum && match.Extras < bestMatch.Extras))
            {
                best = part;
                bestMatch = match;
            }
        }

        return best;
    }

    private static Articulation FindBestArticulation(Part part, string[] pitchTokens, IReadOnlyList<(string Alias, string CanonicalName)> replacements)
    {
        Articulation best = null;
        MatchResult bestMatch = default;

        foreach (var articulation in part.Articulations)
        {
            var match = CountMatches(pitchTokens, Tokenize(Normalize(articulation.Name, replacements)));

            // 同点の場合は余分な語がないものを優先し、それも同点なら一覧の先頭を採用する。主要なArticulationが先頭に並ぶ前提
            if (best == null
                || match.Matched > bestMatch.Matched
                || (match.Matched == bestMatch.Matched && match.IndexSum > bestMatch.IndexSum)
                || (match.Matched == bestMatch.Matched && match.IndexSum == bestMatch.IndexSum && match.Extras == 0 && bestMatch.Extras != 0))
            {
                best = articulation;
                bestMatch = match;
            }
        }

        return best;
    }

    private static MatchResult CountMatches(string[] pitchTokens, string[] candidateTokens)
    {
        int matched = 0;
        int indexSum = 0;
        bool hasWord = false;

        for (int i = 0; i < pitchTokens.Length; i++)
        {
            if (!candidateTokens.Any(c => IsSameToken(pitchTokens[i], c)))
            {
                continue;
            }

            matched++;
            indexSum += i;

            // 数字や1文字だけの一致（"1"、"L" など）では、対象パーツと判断しない
            hasWord |= pitchTokens[i].Length >= 2 && pitchTokens[i].Any(char.IsLetter);
        }

        int candidateMatched = candidateTokens.Count(c => pitchTokens.Any(p => IsSameToken(p, c)));

        return new MatchResult(matched, indexSum, candidateTokens.Length - candidateMatched, candidateMatched, hasWord);
    }

    private static bool IsSameToken(string a, string b)
    {
        if (a == b)
        {
            return true;
        }

        return Math.Min(a.Length, b.Length) >= MinimumPrefixLength
            && (a.StartsWith(b, StringComparison.Ordinal) || b.StartsWith(a, StringComparison.Ordinal));
    }

    private static (string Alias, string CanonicalName)[] BuildReplacements(IEnumerable<PartNameAlias> dictionary)
    {
        return (dictionary ?? Array.Empty<PartNameAlias>())
            .SelectMany(entry => entry.GetAllNames().Select(alias => (Alias: alias, entry.CanonicalName)))
            .OrderByDescending(x => x.Alias.Length)
            .ToArray();
    }

    // 別名を正規名へ置き換える。語の途中（"Hat" が "Hi-Hat" の一部など）では置き換えない
    private static string Normalize(string name, IReadOnlyList<(string Alias, string CanonicalName)> replacements)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var result = new StringBuilder();
        int index = 0;

        while (index < name.Length)
        {
            var match = IsWordStart(name, index)
                ? replacements.FirstOrDefault(x => IsAliasAt(name, index, x.Alias))
                : default;

            if (match.Alias != null)
            {
                result.Append(match.CanonicalName);
                index += match.Alias.Length;
            }
            else
            {
                result.Append(name[index]);
                index++;
            }
        }

        return result.ToString().Trim().ToLowerInvariant();
    }

    private static bool IsWordStart(string text, int index)
    {
        return index == 0 || !char.IsLetterOrDigit(text[index - 1]);
    }

    private static bool IsAliasAt(string text, int index, string alias)
    {
        if (string.Compare(text, index, alias, 0, alias.Length, StringComparison.OrdinalIgnoreCase) != 0)
        {
            return false;
        }

        int end = index + alias.Length;
        return end == text.Length || !char.IsLetterOrDigit(text[end]);
    }

    // 語の位置（後ろの語ほど具体的）を比較に使うため、出現順を保って重複を除く
    private static string[] Tokenize(string text)
    {
        return text
            .Split(new[] { ' ', '-', '_', '[', ']', '<', '>', '(', ')', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToLowerInvariant())
            .Distinct()
            .ToArray();
    }

    private readonly record struct MatchResult(int Matched, int IndexSum, int Extras, int CandidateMatched, bool HasWord);

    private static void ResolveDuplicates(Kit kit, HashSet<KitPitch> newlyAssigned)
    {
        var groups = kit.Pitches
            .Where(p => p.ArticulationID != 0)
            .GroupBy(p => p.ArticulationID);

        foreach (var group in groups)
        {
            var members = group.ToArray();
            if (members.Length <= 1)
            {
                continue;
            }

            var representative = members.FirstOrDefault(p => p.Separator == Separator.None && !newlyAssigned.Contains(p));

            if (representative == null)
            {
                var newMembers = members.Where(p => newlyAssigned.Contains(p)).ToArray();
                representative = newMembers.FirstOrDefault(p => GMDrumMap.IsStandardDrumNote(p.Pitch))
                    ?? newMembers.FirstOrDefault();
            }

            foreach (var member in members)
            {
                if (!newlyAssigned.Contains(member))
                {
                    continue;
                }

                member.Separator = (member == representative) ? Separator.None : Separator.Duplicate;
            }
        }
    }
}
