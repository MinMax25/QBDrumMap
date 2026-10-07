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

namespace QBDrumMap.Class.Services;

[DISingleton<ISettingService>]
public partial class SettingService : ObservableObject, ISettingService
{
    private readonly ISettingStore<AppSettings> SettingStore;

    private readonly IThemeSelectorService ThemeSelector;

    private IMidiService MIDI;

    [ObservableProperty]
    private BaseTheme baseTheme = BaseTheme.Dark;

    [ObservableProperty]
    private string themeColor = "Steel";

    [ObservableProperty]
    private string language = "en-US";

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

    private bool _IsExtendedProgramChange = false;

    [ObservableProperty]
    private ObservableCollection<PartNameAlias> partNameDictionary = new();

    [JsonIgnore]
    public string SearchArticulationFilter { get; set; } = string.Empty;

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

    public SettingService(
        ISettingStore<AppSettings> settingStore,
        IThemeSelectorService themeSelectorService,
        IMidiService midiService)
    {
        SettingStore = settingStore;
        ThemeSelector = themeSelectorService;
        MIDI = midiService;
    }

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

        QBDrumMap.Resources.Culture = culture;
        QBDrumMap.Name.Culture = culture;

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        libQB.Localization.LocalizationManager.Instance.SetCulture(culture.Name);
    }

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

    public void Dispose()
    {
    }
}
