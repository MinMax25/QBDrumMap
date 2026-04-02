using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;
using libQB.Enums;
using QBDrumMap.Class.Cubase;
using QBDrumMap.Class.Enums;
using QBDrumMap.Services;

namespace QBDrumMap.Class.Services
{
    [DISingleton<ISettingService>]
    public partial class SettingService : ObservableObject, ISettingService
    {
        #region Fields

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
        private Languages language = Languages.English;

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

        #endregion

        #region Properties

        // 設定ファイルの保存先パス
        private static string SettingFilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Setting.json");
            }
        }

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

        public SettingService()
        {
        }

        public SettingService(IThemeSelectorService themeSelectorService, IMidiService midiService)
        {
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
            if (File.Exists(SettingFilePath))
            {
                var jsonString = File.ReadAllText(SettingFilePath);
                var restore = JsonSerializer.Deserialize<SettingService>(jsonString);

                if (restore != null)
                {
                    GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                        .ToList()
                        .ForEach(p =>
                        {
                            p.SetValue(this, p.GetValue(restore));
                        });
                }
            }

            var culture = Language switch
            {
                Languages.Japanese => new CultureInfo("ja-JP"),
                Languages.English => new CultureInfo("en-US"),
                _ => throw new NotSupportedException()
            };

            libQB.Properties.Dialog.Culture = culture;
            libQB.Properties.Hamburger.Culture = culture;
            libQB.Properties.Menu.Culture = culture;
            libQB.Properties.Resources.Culture = culture;

            QBDrumMap.Properties.Resources.Culture = culture;
            QBDrumMap.Properties.Name.Culture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            ThemeSelector?.SetTheme(BaseTheme, ThemeColor);
        }

        public void Save()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SettingFilePath, jsonString);
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