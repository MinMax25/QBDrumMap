using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class.Extensions;

public static class UtilArticulationAutoAssign
{
    private const double SimilarityThreshold = 0.6;

    public static (int AssignedCount, int SkippedCount) AutoAssignArticulations(this MapData data, Kit kit, IEnumerable<PartNameAlias> dictionary)
    {
        if (kit == null)
        {
            return (0, 0);
        }

        var owningPlugin = data.Plugins.FirstOrDefault(p => p.Kits.Contains(kit));
        var dictionaryList = dictionary?.ToArray() ?? Array.Empty<PartNameAlias>();

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

            int? articulationID = FindByOtherKitReference(data, kit, owningPlugin, pitch.Name, dictionaryList)
                ?? FindByPartDictionary(data, pitch.Name, dictionaryList);

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

    private static int? FindByOtherKitReference(MapData data, Kit targetKit, Plugin owningPlugin, string pitchName, IReadOnlyList<PartNameAlias> dictionary)
    {
        string normalized = Normalize(pitchName, dictionary);

        var candidates = data.Plugins
            .SelectMany(p => p.Kits.Select(k => new { Plugin = p, Kit = k }))
            .Where(x => x.Kit != targetKit)
            .SelectMany(x => x.Kit.Pitches.Select(p => new { x.Plugin, Pitch = p }))
            .Where(x => data.IsValidArticulationID(x.Pitch.ArticulationID) && !string.IsNullOrWhiteSpace(x.Pitch.Name))
            .Where(x => Normalize(x.Pitch.Name, dictionary) == normalized)
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

    private static int? FindByPartDictionary(MapData data, string pitchName, IReadOnlyList<PartNameAlias> dictionary)
    {
        var part = IdentifyPart(data, pitchName, dictionary);
        if (part == null || part.Articulations.Count == 0)
        {
            return null;
        }

        string normalized = Normalize(pitchName, dictionary);

        var exact = part.Articulations.FirstOrDefault(a => Normalize(a.Name, dictionary) == normalized);
        if (exact != null)
        {
            return exact.ID;
        }

        Articulation best = null;
        double bestScore = 0;

        foreach (var articulation in part.Articulations)
        {
            double score = CalculateSimilarity(normalized, Normalize(articulation.Name, dictionary));
            if (score > bestScore)
            {
                bestScore = score;
                best = articulation;
            }
        }

        return (best != null && bestScore >= SimilarityThreshold) ? best.ID : null;
    }

    private static Part IdentifyPart(MapData data, string pitchName, IReadOnlyList<PartNameAlias> dictionary)
    {
        string bestCanonical = null;
        int bestLength = 0;

        foreach (var entry in dictionary)
        {
            foreach (var candidate in entry.GetAllNames())
            {
                if (candidate.Length > bestLength && ContainsWord(pitchName, candidate))
                {
                    bestCanonical = entry.CanonicalName;
                    bestLength = candidate.Length;
                }
            }
        }

        if (bestCanonical == null)
        {
            return null;
        }

        return data.Parts.FirstOrDefault(p => string.Equals(p.Name, bestCanonical, StringComparison.OrdinalIgnoreCase));
    }

    private static bool ContainsWord(string text, string word)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(word))
        {
            return false;
        }

        return text.Contains(word, StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string name, IReadOnlyList<PartNameAlias> dictionary)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        string result = name;

        var replacements = dictionary
            .SelectMany(entry => entry.GetAllNames().Select(alias => (Alias: alias, entry.CanonicalName)))
            .OrderByDescending(x => x.Alias.Length);

        foreach (var (alias, canonicalName) in replacements)
        {
            int index = result.IndexOf(alias, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                result = result.Remove(index, alias.Length).Insert(index, canonicalName);
            }
        }

        return result.Trim().ToLowerInvariant();
    }

    private static double CalculateSimilarity(string a, string b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
        {
            return 0;
        }

        var tokensA = Tokenize(a);
        var tokensB = Tokenize(b);

        if (tokensA.Count == 0 || tokensB.Count == 0)
        {
            return 0;
        }

        int common = tokensA.Intersect(tokensB).Count();
        int union = tokensA.Union(tokensB).Count();

        return union == 0 ? 0 : (double)common / union;
    }

    private static HashSet<string> Tokenize(string text)
    {
        return text
            .Split(new[] { ' ', '-', '_', '[', ']', '<', '>', '(', ')', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet();
    }

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
