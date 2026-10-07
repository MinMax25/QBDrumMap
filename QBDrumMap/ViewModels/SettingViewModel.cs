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

namespace QBDrumMap.ViewModels;

[DIPage<SettingPage>]
public partial class SettingViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<PartNameAlias> _selectedPartNameDictionaryEntries = new();

    [ObservableProperty]
    private PartNameAlias _selectedPartNameDictionaryEntry;

    public ISettingService Setting
    {
        get
        {
            return SettingService;
        }
    }

    public IMidiService MIDI
    {
        get
        {
            return App.GetService<IMidiService>();
        }
    }

    public Dictionary<string, Brush> AccentColors { get; set; }

    private static readonly Dictionary<string, string> DisplayNameOverrides = new()
    {
        ["zh-TW"] = "繁體中文",
        ["zh-CN"] = "简体中文",
        ["ko-KR"] = "한국어",
        ["es-ES"] = "Español",
        ["pt-BR"] = "Português (Brasil)",
        ["de-DE"] = "Deutsch",
        ["fr-FR"] = "Français",
    };

    private static readonly string[] BuiltInOrder = { "ja-JP", "en-US" };

    public IReadOnlyList<object> AvailableLanguages
    {
        get
        {
            var languages = LocalizationManager.Instance.AvailableLanguages
                .Select(l =>
                {
                    return DisplayNameOverrides.TryGetValue(l.Code, out var name)
                        ? new LanguageInfo { Code = l.Code, DisplayName = name, IsBuiltIn = l.IsBuiltIn }
                        : l;
                })
                .ToList();

            var builtIn = BuiltInOrder
                .Select(code =>
                {
                    return languages.FirstOrDefault(l => l.Code == code);
                })
                .Where(l => l != null)
                .ToList();

            var external = languages
                .Where(l => l.IsBuiltIn == false)
                .OrderBy(l => l.DisplayName, StringComparer.Ordinal)
                .ToList();

            var result = new List<object>();

            if (builtIn.Count > 0)
            {
                result.Add(LanguageGroupHeaderItem.BuiltIn);
                result.AddRange(builtIn!);
            }

            if (external.Count > 0)
            {
                result.Add(LanguageGroupHeaderItem.LanguagePacks);
                result.AddRange(external);
            }

            return result;
        }
    }

    public ObservableCollection<PartNameAlias> PartNameDictionary
    {
        get
        {
            return Setting.PartNameDictionary;
        }
    }

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

    [RelayCommand]
    private async Task OnSelectCubaseInstallPath()
    {
        var result = await Dialog.ShowSelectFolderDialog(
            App.Text("Setting_CubaseInstallPath", Resources.Setting_CubaseInstallPath),
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
            App.NameText("PartNameAlias", Name.PartNameAlias),
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            SelectedPartNameDictionaryEntries.Clear();
        }
    }
        }
