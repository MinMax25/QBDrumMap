using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using libQB.DialogServices;
using libQB.WindowServices;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using QBDrumMap.Class;
using QBDrumMap.Class.Enums;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Contracts.Views;
using QBDrumMap;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels;

[DIWindow<IShellWindow, ShellWindow>]
public partial class ShellViewModel : ViewModelBase, IWindowClosingAware
{
    [ObservableProperty]
    private HamburgerMenuItem _selectedMenuItem;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isCommandEnabled = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    private string _busyMessage = string.Empty;

    private const string QBD_FILTER = "QB DrumMap (*.qbd)|*.qbd";

    public IRelayCommand GoBackCommand => new RelayCommand(OnGoBack, CanGoBack);

    public ICommand MenuItemInvokedCommand => new RelayCommand(OnMenuItemInvoked);

    public ICommand LoadedCommand => new RelayCommand(OnLoaded);

    public ICommand UnloadedCommand => new RelayCommand(OnUnloaded);

    public string AboutCaption
    {
        get
        {
            return string.Format(App.Text("MenuHelpAboutCaption", "About {0}"), AppName);
        }
    }

    private string AppName
    {
        get
        {
            return typeof(App).Assembly.GetName().Name;
        }
    }

    private ISettingService Setting
    {
        get
        {
            return SettingService;
        }
    }

    public ObservableCollection<HamburgerMenuItemBase> MenuItems { get; } =
    [
        new HamburgerMenuSeparatorItem(),

        new HamburgerMenuIconItem()
        {
            Label = App.Text("Title_PluginPage", "Plugin"),
            Icon = ClonePackIcon(PageIcon.PluginPage),
            TargetPageType = typeof(PluginViewModel)
        },

        new HamburgerMenuSeparatorItem(),

        new HamburgerMenuIconItem()
        {
            Label = App.Text("Title_PartsPage", "Parts"),
            Icon = ClonePackIcon(PageIcon.PartsPage),
            TargetPageType = typeof(PartsViewModel)
        },

        new HamburgerMenuIconItem()
        {
            Label = App.Text("Title_ArticulationsPage", "Articulations"),
            Icon = ClonePackIcon(PageIcon.ArticulationsPage),
            TargetPageType = typeof(ArticulationsViewModel)
        },

        new HamburgerMenuSeparatorItem(),

        new HamburgerMenuIconItem()
        {
            Label = App.Text("Title_ExportPage", "Export"),
            Icon = ClonePackIcon(PageIcon.ExportPage),
            TargetPageType = typeof(ExportViewModel)
        },

        new HamburgerMenuSeparatorItem(),

        new HamburgerMenuIconItem()
        {
            Label = App.Text("Title_ConvertMidiPage", "Convert MIDI"),
            Icon = ClonePackIcon(PageIcon.ConvertPage),
            TargetPageType = typeof(ConvertMidiViewModel)
        },

        new HamburgerMenuSeparatorItem()
    ];

    public ObservableCollection<HamburgerMenuItemBase> OptionMenuItems { get; } =
    [
        new HamburgerMenuIconItem()
        {
            Label = App.Text("Setting_PageTitle", "Settings"),
            Icon = ClonePackIcon(PageIcon.SettingPage),
            TargetPageType = typeof(SettingViewModel)
        }
    ];

    public ShellViewModel(IDIContainer diContainer)
        : base(diContainer)
    {
        SetTitle();

        Setting.PropertyChanged += OnPropertyChanged;

        if (Setting.IsOpenTheLastFileOpened && File.Exists(Setting.LastOpenedFilePath))
        {
            var loadTask = Task.Run(async () =>
            {
                return await MapData.LoadAsync(Setting.LastOpenedFilePath);
            });

            if (loadTask.Result == false)
            {
                Setting.LastOpenedFilePath = string.Empty;
            }
        }
        else
        {
            Setting.LastOpenedFilePath = string.Empty;
        }

        MapData.EditStateChanged += OnPropertyChanged;
    }

    [RelayCommand]
    private async Task OnNewFile()
    {
        if (IsBusy)
        {
            return;
        }

        if (await ConfirmUnsavedChangesAsync() == false)
        {
            return;
        }

        MapData.Initialize();
        UndoManager.Clear();
        Setting.LastOpenedFilePath = string.Empty;
    }

    [RelayCommand]
    private async Task OnOpenFile()
    {
        if (IsBusy)
        {
            return;
        }

        if (await ConfirmUnsavedChangesAsync() == false)
        {
            return;
        }

        var path = await Dialog.ShowOpenFileDialog(
            App.Text("Common_OpenFile", "Open File"),
            QBD_FILTER,
            GetValidFilePath(Setting.LastOpenedFilePath));

        if (path == null)
        {
            return;
        }

        bool loaded;

        BusyMessage = App.Text("Busy_Loading", "Loading…");
        IsBusy = true;
        try
        {
            loaded = await MapData.LoadAsync(path);
        }
        finally
        {
            IsBusy = false;
        }

        if (loaded)
        {
            Setting.LastOpenedFilePath = path;
            UndoManager.Clear();
        }
        else
        {
            await Dialog.ShowErrorAsync(
                $"{App.Text("Error_FileIO", "File IO Error")}{Environment.NewLine}{MapData.LastError?.Message}",
                App.Text("Common_OpenFile", "Open File"));
        }
    }

    [RelayCommand]
    private async Task OnSaveFile()
    {
        await SaveCoreAsync();
    }

    private async Task<bool> SaveCoreAsync()
    {
        if (IsBusy)
        {
            return false;
        }

        if (MapData.HasError())
        {
            await Dialog.ShowAlertAsync(App.Text("Confirm_SaveHasError"));
            return false;
        }

        var path = await Dialog.ShowSaveFileDialog(
            App.Text("Common_SaveFile", "Save File"),
            QBD_FILTER,
            GetValidFilePath(Setting.LastOpenedFilePath));

        if (path == null)
        {
            return false;
        }

        bool saved;

        BusyMessage = App.Text("Busy_Saving", "Saving…");
        IsBusy = true;
        try
        {
            saved = await MapData.SaveAsync(path);
        }
        finally
        {
            IsBusy = false;
        }

        if (saved)
        {
            Setting.LastOpenedFilePath = path;
        }
        else
        {
            await Dialog.ShowErrorAsync(
                $"{App.Text("Error_FileIO", "File IO Error")}{Environment.NewLine}{MapData.LastError?.Message}",
                App.Text("Common_SaveFile", "Save File"));
        }

        return saved;
    }

    private Task<bool> ConfirmUnsavedChangesAsync()
    {
        return UnsavedChangesGuard.ConfirmAsync(MapData.IsModified, AskUnsavedChoiceAsync, SaveCoreAsync);
    }

    private async Task<UnsavedChoice> AskUnsavedChoiceAsync()
    {
        var settings = new MetroDialogSettings
        {
            AffirmativeButtonText = App.Text("Button_Save", "Save"),
            NegativeButtonText = App.Text("Button_Cancel", "Cancel"),
            FirstAuxiliaryButtonText = App.Text("Button_DontSave", "Don't Save"),
            DefaultButtonFocus = MessageDialogResult.Affirmative,
        };

        var result = await App.GetService<IDialogCoordinator>().ShowMessageAsync(
            this,
            DialogTextProvider.ConfirmTitle(),
            App.Text("Confirm_UnsavedChanges", "There are unsaved changes. Save them before continuing?"),
            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
            settings);

        return result switch
        {
            MessageDialogResult.Affirmative => UnsavedChoice.Save,
            MessageDialogResult.FirstAuxiliary => UnsavedChoice.DontSave,
            _ => UnsavedChoice.Cancel,
        };
    }

    [RelayCommand]
    private void OnShowAbout()
    {
        WindowService.ShowWindowWithCallback<About, AboutViewModel, object>(
            owner: Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
        );
    }

    [RelayCommand]
    private void OnShowDocument()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string language = Setting.Language;
        string path = language switch
        {
            "ja-JP" => Path.Combine(baseDir, "Resources", "Document", "Manual", "ja", "index.html"),
            "en-US" => Path.Combine(baseDir, "Resources", "Document", "Manual", "en", "index.html"),
            _ => Path.Combine(baseDir, "Languages", language, "Manual", "index.html")
        };

        if (File.Exists(path) == false)
        {
            path = Path.Combine(baseDir, "Resources", "Document", "Manual", "en", "index.html");
        }

        if (File.Exists(path) == false)
        {
            return;
        }

        var p = new Process();
        p.StartInfo = new ProcessStartInfo(path)
        {
            UseShellExecute = true
        };
        p.Start();
    }

    private void ApplyLocalizedLabels()
    {
        ((HamburgerMenuIconItem)MenuItems[1]).Label = App.Text("Title_PluginPage", "Plugin");
        ((HamburgerMenuIconItem)MenuItems[3]).Label = App.Text("Title_PartsPage", "Parts");
        ((HamburgerMenuIconItem)MenuItems[4]).Label = App.Text("Title_ArticulationsPage", "Articulations");
        ((HamburgerMenuIconItem)MenuItems[6]).Label = App.Text("Title_ExportPage", "Export");
        ((HamburgerMenuIconItem)MenuItems[8]).Label = App.Text("Title_ConvertMidiPage", "Convert MIDI");
        ((HamburgerMenuIconItem)OptionMenuItems[0]).Label = App.Text("Setting_PageTitle", "Settings");
    }

    [RelayCommand]
    private void OnClose()
    {
        WindowService.CloseWindow(Application.Current.MainWindow);
    }

    [RelayCommand]
    private void OnUndo()
    {
        if (IsBusy)
        {
            return;
        }

        UndoManager.Undo();
    }

    [RelayCommand]
    private void OnRedo()
    {
        if (IsBusy)
        {
            return;
        }

        UndoManager.Redo();
    }

    private void OnLoaded()
    {
        Navigation.Navigated += OnNavigated;
    }

    private void OnUnloaded()
    {
        Navigation.Navigated -= OnNavigated;
    }

    private void OnNavigated(object sender, string viewModelName)
    {
        var item = MenuItems
            .OfType<HamburgerMenuItem>()
            .FirstOrDefault(i =>
            {
                return viewModelName == i.TargetPageType?.FullName;
            });

        if (item != null)
        {
            SelectedMenuItem = item;
        }

        GoBackCommand.NotifyCanExecuteChanged();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Setting.LastOpenedFilePath):
                SetTitle();
                break;
            case nameof(Setting.Language):
                ApplyLocalizedLabels();
                OnPropertyChanged(nameof(AboutCaption));
                break;
            case nameof(MapData.EditState):
                UpdateCommandEnabled();
                break;
            default:
                break;
        }
    }

    public async Task<bool> OnWindowClosingAsync()
    {
        if (IsBusy)
        {
            return false;
        }

        if (MapData.IsModified())
        {
            return await ConfirmUnsavedChangesAsync();
        }
        else
        {
            var result = await Dialog.ShowConfirmAsync(App.Text("Confirm_ApplicationClose"));
            return result ?? false;
        }
    }

    partial void OnIsBusyChanged(bool value)
    {
        UpdateCommandEnabled();
    }

    private void UpdateCommandEnabled()
    {
        IsCommandEnabled = !MapData.EditState && !IsBusy;
    }

    private bool CanGoBack()
    {
        return Navigation.CanGoBack;
    }

    private void OnGoBack()
    {
        Navigation.GoBack();
    }

    private void OnMenuItemInvoked()
    {
        if (SelectedMenuItem is HamburgerMenuIconItem item && item.TargetPageType != null)
        {
            NavigateTo(item.TargetPageType);
        }
    }

    private void NavigateTo(Type targetViewModel)
    {
        if (targetViewModel != null)
        {
            Navigation.NavigateTo(targetViewModel.FullName);
            App.GetService<IMidiService>().MidiThruEnabled = false;
        }
    }

    private static object ClonePackIcon(PageIcon pageIcon)
    {
        var original = Application.Current.Resources[$"{pageIcon}Icon"];
        if (original == null)
        {
            return null;
        }

        string xaml = System.Windows.Markup.XamlWriter.Save(original);
        return System.Windows.Markup.XamlReader.Parse(xaml);
    }

    private void SetTitle()
    {
        if (File.Exists(Setting.LastOpenedFilePath))
        {
            Title = $"{AppName} ({Path.GetFileNameWithoutExtension(Setting.LastOpenedFilePath)})";
        }
        else
        {
            Title = $"{AppName}";
        }
    }

    private string GetValidFilePath(string path)
    {
        if (File.Exists(path))
        {
            return path;
        }

        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            MapData.EditStateChanged -= OnPropertyChanged;
            Setting.PropertyChanged -= OnPropertyChanged;
        }
    }
}
