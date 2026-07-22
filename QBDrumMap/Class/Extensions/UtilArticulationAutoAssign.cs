using QBDrumMap.Class.MapModels;

namespace QBDrumMap.Class.Extensions
{
    // マップデータのアーティキュレーションからKitPitch.ArticulationIDを自動設定するロジック
    public static class UtilArticulationAutoAssign
    {
        // 類似度マッチング（優先度3）を採用する最低スコア
        private const double SimilarityThreshold = 0.6;

        #region General

        // 指定Kitの未設定ピッチに対してアーティキュレーションを自動割り当てする
        // 戻り値: (自動設定できた件数, 候補が見つからずスキップした件数)
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
                if (pitch.ArticulationID != 0 || string.IsNullOrWhiteSpace(pitch.Name))
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
                    // ピッチ名はあるがアーティキュレーションが割り当たらなかった場合はSeparateとして区別する
                    pitch.Separator = Separator.Separate;
                    skippedCount++;
                }
            }

            ResolveDuplicates(kit, newlyAssigned);

            return (assignedCount, skippedCount);
        }

        #endregion

        #region Private

        private static bool IsGMFamily(PluginType pluginType)
        {
            return pluginType == PluginType.GM
                || pluginType == PluginType.GM2
                || pluginType == PluginType.GS
                || pluginType == PluginType.XG;
        }

        // 優先度1a/1b: 同一Plugin内の他Kitを優先し、無ければ全Kitから、同名（正規化後）かつArticulationID設定済みのピッチを探す
        private static int? FindByOtherKitReference(MapData data, Kit targetKit, Plugin owningPlugin, string pitchName, IReadOnlyList<PartNameAlias> dictionary)
        {
            string normalized = Normalize(pitchName, dictionary);

            var candidates = data.Plugins
                .SelectMany(p => p.Kits.Select(k => new { Plugin = p, Kit = k }))
                .Where(x => x.Kit != targetKit)
                .SelectMany(x => x.Kit.Pitches.Select(p => new { x.Plugin, Pitch = p }))
                .Where(x => x.Pitch.ArticulationID != 0 && !string.IsNullOrWhiteSpace(x.Pitch.Name))
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

        // 優先度2/3: 辞書で対象Partを特定し、完全一致→類似度マッチングの順で検索
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

        // パーツ名辞書を使い、ピッチ名から対象Partを特定する（最長一致の表記を優先）
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

        // 辞書の別名を正式名称に置き換えたうえで、比較用に小文字化・トリムする
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

        // トークン単位のJaccard係数で類似度を算出
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

        // 同一ArticulationIDが複数ピッチに割り当てられた場合の代表ピッチ／重複ピッチの後処理
        // 今回新規に割り当てたピッチのSeparatorのみを更新し、既存の手動設定は変更しない
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

        #endregion
    }
}
