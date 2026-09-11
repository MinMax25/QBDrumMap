#nullable enable

using System.ComponentModel;
using libQB.Localization;

namespace QBDrumMap.Class;

// 言語選択コンボボックスのグループヘッダー(組み込み言語/言語パックの見出し)を表す表示専用マーカー。
// LanguageInfoではなくこのインスタンスをItemsSourceに混在させ、
// XAML側はDataTriggerの参照比較(Value={x:Static ...})でヘッダーを検出する。
public sealed class LanguageGroupHeaderItem : INotifyPropertyChanged
{
    public static readonly LanguageGroupHeaderItem BuiltIn = new("QBDrumMap.Resources.BuiltInLanguages");

    public static readonly LanguageGroupHeaderItem LanguagePacks = new("QBDrumMap.Resources.LanguagePacks");

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly string _key;

    // 言語切替(SetCulture)のたびにDisplayNameの再評価をUIへ通知する
    private LanguageGroupHeaderItem(string key)
    {
        _key = key;
        LocalizationManager.Instance.PropertyChanged += (_, _) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
    }

    // LocalizationManagerのインデクサは呼び出しのたびに現在のカルチャで再評価するため、
    // 組み込み言語(en-US/ja-JP)への切替でもfallbackが固定化されない。
    // ComboBox.SelectedValuePath="Code" 評価時にプロパティ欠落による Binding Error が発生するのを防止する
    public string Code => string.Empty;

    public string DisplayName => LocalizationManager.Instance[_key] ?? _key;
}
