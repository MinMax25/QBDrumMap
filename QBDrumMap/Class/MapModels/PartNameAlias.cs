using CommunityToolkit.Mvvm.ComponentModel;

namespace QBDrumMap.Class.MapModels
{
    // パーツ名の表記ゆれ辞書の1エントリ（正式名称と別名のセット）
    public partial class PartNameAlias : ObservableObject
    {
        #region Fields

        // 正式名称（Part.Nameと対応させる文字列）
        [ObservableProperty]
        private string canonicalName = string.Empty;

        // 別名（カンマ区切り、例: "Bass Drum,BD"）
        [ObservableProperty]
        private string aliases = string.Empty;

        #endregion

        #region Methods

        // 正式名称と別名をまとめた表記ゆれ候補一覧を取得
        public IEnumerable<string> GetAllNames()
        {
            var names = new List<string>();

            if (!string.IsNullOrWhiteSpace(CanonicalName))
            {
                names.Add(CanonicalName.Trim());
            }

            if (!string.IsNullOrWhiteSpace(Aliases))
            {
                names.AddRange(Aliases.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }

            return names.Where(x => !string.IsNullOrWhiteSpace(x));
        }

        #endregion
    }
}
