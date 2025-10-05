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
    public partial class SettingService
        : ObservableObject
        , ISettingService
    {
        #region Properties

        private static string SettingFilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Setting.json");

        [ObservableProperty]
        private BaseTheme baseTheme = BaseTheme.Dark;

        [ObservableProperty]
        private string themeColor = "Steel";

        [ObservableProperty]
        private Languages language = Languages.English;

        [ObservableProperty]
        private Pages startUpPage = Pages.PluginPage;

        [ObservableProperty]
        private bool useArticulationPitchName = true;

        [ObservableProperty]
        private bool useArticulation = true;

        [ObservableProperty]
        private bool isOpenTheLastFileOpened = true;

        [ObservableProperty]
        private bool isPartScoreViewExtended = false;

        [ObservableProperty]
        private string lastOpenedFilePath = string.Empty;

        [ObservableProperty]
        private string lastImportFilePath = string.Empty;

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

        [ObservableProperty]
        private MIDIFormat convertMIDIFormat = MIDIFormat.Format1;

        [ObservableProperty]
        private string cubaseInstallPath = string.Empty;

        [ObservableProperty]
        private string midiInDevice = string.Empty;

        [ObservableProperty]
        private int midiInFixedPitch = -1;

        [JsonIgnore]
        public bool IsPluginOptionsViewExtended { get => _IsPluginOptionsViewExtended; set => SetProperty(ref _IsPluginOptionsViewExtended, value); }
        private bool _IsPluginOptionsViewExtended = false;

        private IMidiService MIDI;

        #endregion

        #region Fields

        private readonly IThemeSelectorService ThemeSelector;

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

        #region PropertyChanged Callbacks

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
            if (MIDI == null) return;
            MIDI.MidiInDevice = value;
        }

        partial void OnMidiInFixedPitchChanged(int value)
        {
            if (MIDI == null) return;
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
                if (JsonSerializer.Deserialize<SettingService>(jsonString) is SettingService _restore)
                {
                    GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                    .ToList()
                    .ForEach(p => p.SetValue(this, p.GetValue(_restore)));
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

            Properties.Resources.Culture = culture;
            Properties.Name.Culture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            ThemeSelector.SetTheme(BaseTheme, ThemeColor);
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