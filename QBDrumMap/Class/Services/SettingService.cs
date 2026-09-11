using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;
using libQB.Enums;
using libQB.Settings;
using QBDrumMap.Class.Cubase;
using QBDrumMap.Class.Enums;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Services;

namespace QBDrumMap.Class.Services
{
    [DISingleton<ISettingService>]
    public partial class SettingService : ObservableObject, ISettingService
    {
        #region Fields

        private readonly ISettingStore<AppSettings> SettingStore;

        // テーマの切り替えを管理するサービス
        private readonly IThemeSelectorService ThemeSelector;

        // MIDI入出力を管理するサービス
        private IMidiService MIDI;

        // ベーステーマ (Dark/Light)
        [ObservableProperty]
        private BaseTheme baseTheme = BaseTheme.Dark;

        // テーマのアクセントカラー名
        [ObservableProperty]
        private string themeColor = "Steel";

        // アプリケーションの表示言語
        [ObservableProperty]
        private string language = "en-US";

        // 起動時に表示する初期ページ
        [ObservableProperty]
        private Pages startUpPage = Pages.PluginPage;

        // アーティキュレーション名にピッチ名を含めるかどうか
        [ObservableProperty]
        private bool useArticulationPitchName = true;

        // アーティキュレーション機能自体の有効/無効
        [ObservableProperty]
        private bool useArticulation = true;

        // 最後に開いたファイルを起動時に自動で読み込むかどうか
        [ObservableProperty]
        private bool isOpenTheLastFileOpened = true;

        // パートスコアビューの拡張表示設定
        [ObservableProperty]
        private bool isPartScoreViewExtended = false;

        // 最後に開いたファイルのフルパス
        [ObservableProperty]
        private string lastOpenedFilePath = string.Empty;

        // 最後にインポートしたファイルのフルパス
        [ObservableProperty]
        private string lastImportFilePath = string.Empty;

        // 各エクスポート形式のデフォルト出力パス
        [ObservableProperty]
        private string exportStudioOneDefaultPath = string.Empty;

        [ObservableProperty]
        private string exportCubaseDefaultPath = string.Empty;

        [ObservableProperty]
        private string exportTextDefaultPath = string.Empty;

        [ObservableProperty]
        private string exportCubaseBaseOnDefaultPath = string.Empty;

        [ObservableProperty]
        private string exportNoteMapperDefaultPath = string.Empty;

        [ObservableProperty]
        private string exportQBDrummerDefaultPath = string.Empty;

        // MIDI出力時のフォーマット形式
        [ObservableProperty]
        private MIDIFormat convertMIDIFormat = MIDIFormat.Format1;

        // Cubaseの実行ファイル/インストールディレクトリのパス
        [ObservableProperty]
        private string cubaseInstallPath = string.Empty;

        // 既定のMIDI入力デバイス名
        [ObservableProperty]
        private string midiInDevice = string.Empty;

        // MIDI入力時の固定ピッチ（-1は無効）
        [ObservableProperty]
        private int midiInFixedPitch = -1;

        // プログラムチェンジの拡張設定フラグ
        private bool _IsExtendedProgramChange = false;

        // パーツ名の表記ゆれ辞書
        [ObservableProperty]
        private ObservableCollection<PartNameAlias> partNameDictionary = new();

        #endregion

        #region Properties

        // アーティキュレーション検索用のフィルター（非永続化）
        [JsonIgnore]
        public string SearchArticulationFilter { get; set; } = string.Empty;

        // プログラムチェンジの拡張設定
        [JsonIgnore]
        public bool IsExtendedProgramChange
        {
            get
            {
                return _IsExtendedProgramChange;
            }
            set
            {
                SetProperty(ref _IsExtendedProgramChange, value);
            }
        }

        #endregion

        #region ctor

        public SettingService(
            ISettingStore<AppSettings> settingStore,
            IThemeSelectorService themeSelectorService,
            IMidiService midiService)
        {
            SettingStore = settingStore;
            ThemeSelector = themeSelectorService;
            MIDI = midiService;
        }

        #endregion

        #region Methods

        #region Property Change Handler

        partial void OnBaseThemeChanged(BaseTheme value)
        {
            ThemeSelector?.SetTheme(value, ThemeColor);
        }

        partial void OnThemeColorChanged(string value)
        {
            ThemeSelector?.SetTheme(BaseTheme, value);
        }

        partial void OnLanguageChanged(string value)
        {
            ApplyLanguage(value);
            CubaseScore.Initialize();
        }

        partial void OnMidiInDeviceChanged(string value)
        {
            if (MIDI == null)
            {
                return;
            }
            MIDI.MidiInDevice = value;
        }

        partial void OnMidiInFixedPitchChanged(int value)
        {
            if (MIDI == null)
            {
                return;
            }
            MIDI.MidiInFixedPitch = value;
        }

        partial void OnCubaseInstallPathChanged(string value)
        {
            CubaseScore.Initialize();
        }

        #endregion

        #region General

        public void Load()
        {
            ApplySettings(SettingStore.Load());

            ApplyLanguage(Language);

            ThemeSelector?.SetTheme(BaseTheme, ThemeColor);

            SeedDefaultPartNameDictionaryIfEmpty();
        }

        private void ApplySettings(AppSettings settings)
        {
            BaseTheme = settings.BaseTheme;
            ThemeColor = settings.ThemeColor;
            Language = settings.Language;
            StartUpPage = settings.StartUpPage;
            UseArticulationPitchName = settings.UseArticulationPitchName;
            UseArticulation = settings.UseArticulation;
            IsOpenTheLastFileOpened = settings.IsOpenTheLastFileOpened;
            IsPartScoreViewExtended = settings.IsPartScoreViewExtended;
            LastOpenedFilePath = settings.LastOpenedFilePath;
            LastImportFilePath = settings.LastImportFilePath;
            ExportStudioOneDefaultPath = settings.ExportStudioOneDefaultPath;
            ExportCubaseDefaultPath = settings.ExportCubaseDefaultPath;
            ExportTextDefaultPath = settings.ExportTextDefaultPath;
            ExportCubaseBaseOnDefaultPath = settings.ExportCubaseBaseOnDefaultPath;
            ExportNoteMapperDefaultPath = settings.ExportNoteMapperDefaultPath;
            ExportQBDrummerDefaultPath = settings.ExportQBDrummerDefaultPath;
            ConvertMIDIFormat = settings.ConvertMIDIFormat;
            CubaseInstallPath = settings.CubaseInstallPath;
            MidiInDevice = settings.MidiInDevice;
            MidiInFixedPitch = settings.MidiInFixedPitch;
            PartNameDictionary = new ObservableCollection<PartNameAlias>(
                settings.PartNameDictionary.Select(item => new PartNameAlias
                {
                    CanonicalName = item.CanonicalName,
                    Aliases = item.Aliases
                }));
        }

        private static void ApplyLanguage(string cultureCode)
        {
            CultureInfo culture;
            try
            {
                culture = new CultureInfo(cultureCode);
            }
            catch (CultureNotFoundException)
            {
                culture = new CultureInfo("en-US");
            }

            QBDrumMap.Properties.Resources.Culture = culture;
            QBDrumMap.Properties.Name.Culture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            libQB.Localization.LocalizationManager.Instance.SetCulture(culture.Name);
        }

        // 初回起動時など辞書が未登録の場合に、代表的なドラムパーツの表記ゆれをデフォルト登録する
        private void SeedDefaultPartNameDictionaryIfEmpty()
        {
            if (PartNameDictionary.Any())
            {
                return;
            }

            var defaults = new[]
            {
                new PartNameAlias { CanonicalName = "Kick", Aliases = "Bass Drum,BD" },
                new PartNameAlias { CanonicalName = "Snare", Aliases = "SD" },
                new PartNameAlias { CanonicalName = "Hi-Hat", Aliases = "HiHat,HH,Hat" },
                new PartNameAlias { CanonicalName = "Tom", Aliases = "Floor Tom" },
                new PartNameAlias { CanonicalName = "Ride", Aliases = "Ride Cymbal" },
                new PartNameAlias { CanonicalName = "Crash", Aliases = "Crash Cymbal" },
                new PartNameAlias { CanonicalName = "Cymbal", Aliases = string.Empty },
            };

            foreach (var item in defaults)
            {
                PartNameDictionary.Add(item);
            }
        }

        public void Save()
        {
            SettingStore.Save(CreateSettings(this));
        }

        private static AppSettings CreateSettings(ISettingService source)
        {
            return new AppSettings
            {
                BaseTheme = source.BaseTheme,
                ThemeColor = source.ThemeColor,
                Language = source.Language,
                StartUpPage = source.StartUpPage,
                UseArticulationPitchName = source.UseArticulationPitchName,
                UseArticulation = source.UseArticulation,
                IsOpenTheLastFileOpened = source.IsOpenTheLastFileOpened,
                IsPartScoreViewExtended = source.IsPartScoreViewExtended,
                LastOpenedFilePath = source.LastOpenedFilePath,
                LastImportFilePath = source.LastImportFilePath,
                ExportStudioOneDefaultPath = source.ExportStudioOneDefaultPath,
                ExportCubaseDefaultPath = source.ExportCubaseDefaultPath,
                ExportTextDefaultPath = source.ExportTextDefaultPath,
                ExportCubaseBaseOnDefaultPath = source.ExportCubaseBaseOnDefaultPath,
                ExportNoteMapperDefaultPath = source.ExportNoteMapperDefaultPath,
                ExportQBDrummerDefaultPath = source.ExportQBDrummerDefaultPath,
                ConvertMIDIFormat = source.ConvertMIDIFormat,
                CubaseInstallPath = source.CubaseInstallPath,
                MidiInDevice = source.MidiInDevice,
                MidiInFixedPitch = source.MidiInFixedPitch,
                PartNameDictionary = source.PartNameDictionary
                    .Select(item => new PartNameAliasSetting
                    {
                        CanonicalName = item.CanonicalName,
                        Aliases = item.Aliases
                    })
                    .ToList()
            };
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
        }

        #endregion

        #endregion
    }
}
