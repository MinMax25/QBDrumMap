#nullable enable

using System.ComponentModel;
using libQB.Localization;

namespace QBDrumMap.Class;

// 言語選択リスト内で、組み込み言語と言語パックを区切る表示専用項目。
public sealed class LanguageGroupHeaderItem : INotifyPropertyChanged
{
    public static readonly LanguageGroupHeaderItem BuiltIn = new("QBDrumMap.Resources.BuiltInLanguages");

    public static readonly LanguageGroupHeaderItem LanguagePacks = new("QBDrumMap.Resources.LanguagePacks");

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly string _key;

    private LanguageGroupHeaderItem(string key)
    {
        _key = key;
        LocalizationManager.Instance.PropertyChanged += (_, _) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
    }

    public string DisplayName => LocalizationManager.Instance[_key] ?? _key;
}
