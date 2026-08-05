using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using libQB.Attributes;
using libQB.Localization;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIPage<SettingPage>]
    public partial class SettingViewModel : ViewModelBase
    {
        #region Fields

        // グリッドで選択されているパーツ名辞書エントリ
        [ObservableProperty]
        private ObservableCollection<PartNameAlias> _selectedPartNameDictionaryEntries = new();

        // 単一選択されているパーツ名辞書エントリ
        [ObservableProperty]
        private PartNameAlias _selectedPartNameDictionaryEntry;

        #endregion

        #region Properties

        // 設定サービスへの参照
        public ISettingService Setting
        {
            get
            {
                return SettingService;
            }
        }

        // MIDIサービスへの参照
        public IMidiService MIDI
        {
            get
            {
                return App.GetService<IMidiService>();
            }
        }

        // 利用可能なアクセントカラーのリスト
        public Dictionary<string, Brush> AccentColors { get; set; }

        private static readonly string[] BuiltInLanguageOrder = { "ja-JP", "en-US" };

        private static readonly Dictionary<string, string> LanguageDisplayNameOverrides = new()
        {
            ["zh-TW"] = "繁體中文",
            ["zh-CN"] = "简体中文",
            ["ko-KR"] = "한국어",
            ["es-ES"] = "Español",
            ["pt-BR"] = "Português (Brasil)",
            ["de-DE"] = "Deutsch",
            ["fr-FR"] = "Français",
        };

        public IReadOnlyList<object> AvailableLanguages
        {
            get
            {
                var languages = LocalizationManager.Instance.AvailableLanguages
                    .Select(language => LanguageDisplayNameOverrides.TryGetValue(language.Code, out var displayName)
                        ? new LanguageInfo
                        {
                            Code = language.Code,
                            DisplayName = displayName,
                            IsBuiltIn = language.IsBuiltIn
                        }
                        : language)
                    .ToList();
                var result = new List<object>();

                var builtIn = BuiltInLanguageOrder
                    .Select(code => languages.FirstOrDefault(x => x.Code == code))
                    .Where(x => x != null)
                    .ToList();

                if (builtIn.Count > 0)
                {
                    result.Add(LanguageGroupHeaderItem.BuiltIn);
                    result.AddRange(builtIn!);
                }

                var languagePacks = languages
                    .Where(x => x.IsBuiltIn == false)
                    .OrderBy(x => x.DisplayName, StringComparer.CurrentCulture)
                    .ToList();

                if (languagePacks.Count > 0)
                {
                    result.Add(LanguageGroupHeaderItem.LanguagePacks);
                    result.AddRange(languagePacks);
                }

                return result;
            }
        }

        // パーツ名辞書（表記ゆれ）の編集対象コレクション
        public ObservableCollection<PartNameAlias> PartNameDictionary
        {
            get
            {
                return Setting.PartNameDictionary;
            }
        }

        #endregion

        #region ctor

        public SettingViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
            AccentColors = ThemeManager.Current.Themes
                .GroupBy(x =>
                {
                    return x.ColorScheme;
                })
                .OrderBy(a =>
                {
                    return a.Key;
                })
                .Select(a =>
                {
                    return new
                    {
                        Name = a.Key,
                        ColorBrush = a.First().ShowcaseBrush
                    };
                })
                .ToDictionary(x =>
                {
                    return x.Name;
                }, x =>
                {
                    return x.ColorBrush;
                });
        }

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private async Task OnSelectCubaseInstallPath()
        {
            var result = await Dialog.ShowSelectFolderDialog(
                App.Text("Setting_CubaseInstallPath", Properties.Resources.Setting_CubaseInstallPath),
                Setting.CubaseInstallPath);

            if (result is string path)
            {
                Setting.CubaseInstallPath = path;
            }
        }

        [RelayCommand]
        private void OnAddPartNameDictionaryEntry()
        {
            var entry = new PartNameAlias
            {
                CanonicalName = string.Empty,
                Aliases = string.Empty
            };

            PartNameDictionary.Add(entry);
            SelectedPartNameDictionaryEntry = entry;
        }

        [RelayCommand]
        private async Task OnDeleteSelectedPartNameDictionaryEntries()
        {
            if (SelectedPartNameDictionaryEntries == null || SelectedPartNameDictionaryEntries.Count == 0)
            {
                return;
            }

            string names = string.Join(", ", SelectedPartNameDictionaryEntries.Select(x => x.CanonicalName));
            string message = string.Format(
                App.Text("Message_Command_Delete"),
                App.NameText("PartNameAlias", Properties.Name.PartNameAlias),
                names);

            if (await Dialog.ShowConfirmAsync(message, App.Text("Title_Command_Delete")) == false)
            {
                return;
            }

            foreach (var entry in SelectedPartNameDictionaryEntries.ToArray())
            {
                PartNameDictionary.Remove(entry);
            }

            SelectedPartNameDictionaryEntries.Clear();
            SelectedPartNameDictionaryEntry = null;
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                SelectedPartNameDictionaryEntries.Clear();
            }
        }

        #endregion

        #endregion
    }
}
