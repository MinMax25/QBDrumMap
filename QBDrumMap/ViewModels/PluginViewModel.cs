using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.ViewModels.Controls;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels;

[DIPage<PluginPage>]
public partial class PluginViewModel : ViewModelBaseHasContents
{
    [ObservableProperty]
    private ICollectionView _pluginsView;

    [ObservableProperty]
    private ObservableCollection<Plugin> _selectedPlugins = new();

    [ObservableProperty]
    private Plugin _selectedPlugin;

    public ISettingService Setting
    {
        get
        {
            return SettingService;
        }
    }

    public PluginViewModel(IDIContainer diContainer)
        : base(diContainer)
    {
        Task.Run(InitializeDataAsync);
        ContentViewModel = App.GetService<KitPanelFactory>()?.Create(null);

        MapData.Loaded += OnMapDataLoaded;
        MapData.Saved += OnMapDataSaved;
        MapData.EditStateChanged += OnPropertyChanged;
    }

    [RelayCommand]
    private async Task OnDeleteSelectedPlugins()
    {
        if (SelectedPlugins == null || SelectedPlugins.Count == 0)
        {
            return;
        }

        string names = string.Join("', '", SelectedPlugins.Select(x =>
        {
            return x.Name;
        }));

        string message = string.Format(
            App.Text("Message_Command_Delete"),
            App.NameText("Plugin", Name.Plugin),
            names);

        if (await Dialog.ShowConfirmAsync(message, App.Text("Title_Command_Delete")) == false)
        {
            return;
        }

        foreach (var item in SelectedPlugins.ToArray())
        {
            item.Kits.Clear();
            MapData.Plugins.Remove(item);
        }

        SelectedPlugins.Clear();
        SelectedPlugin = null;

        PluginsView.Refresh();
    }

    [RelayCommand]
    private void OnAdd()
    {
        Plugin plugin = new()
        {
            ID = MapData.NewPluginID()
        };
        plugin.Name = $"Plugin{plugin.ID}";

        MapData.Plugins.AddItem(plugin, x => x.DisplayOrder);

        SelectedPlugin = plugin;
    }

    [RelayCommand]
    private void OnMoveTop() => MapData.Plugins.MoveTop(SelectedPlugins, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveUp() => MapData.Plugins.MoveUp(SelectedPlugins, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveDown() => MapData.Plugins.MoveDown(SelectedPlugins, x => x.DisplayOrder);

    [RelayCommand]
    private void OnMoveBottom() => MapData.Plugins.MoveBottom(SelectedPlugins, x => x.DisplayOrder);

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(MapData.EditState):
                IsCommandEnabled = !MapData.EditState;
                break;
            default:
                break;
        }
    }

    private async void OnMapDataLoaded(object sender, RoutedEventArgs e)
    {
        await InitializeDataAsync();
    }

    private async void OnMapDataSaved(object sender, DrumMapIOEventArgs e)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            PluginsView.Refresh();
        });
    }

    partial void OnSelectedPluginChanged(Plugin oldValue, Plugin newValue)
    {
        oldValue?.UnregisterUndoManager();
        newValue?.RegisterUndoManager(UndoManager);

        IsCommandEnabled = !MapData.EditState;
        IsContentEnabled = newValue != null;

        if (newValue != (ContentViewModel as KitPanelViewModel)?.Plugin)
        {
            ContentViewModel?.Dispose();
        }

        ContentViewModel = App.GetService<KitPanelFactory>()?.Create(newValue);
    }

    private async Task InitializeDataAsync()
    {
        SelectedPlugins.Clear();

        var sortedPlugins = await Task.Run(() =>
        {
            return MapData.Plugins.OrderBy(x =>
            {
                return x.DisplayOrder;
            }).ToList();
        });

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // 表示順に並べ直すだけで、編集ではない
            MapData.RunWithoutDirtyChange(() =>
            {
                MapData.Plugins.Clear();
                foreach (var plugin in sortedPlugins)
                {
                    MapData.Plugins.Add(plugin);
                }
            });

            PluginsView = CollectionViewSource.GetDefaultView(MapData.Plugins);
            SelectedPlugin = null;

            MapData.SetEditState(this, false);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            ContentViewModel?.Dispose();
            MapData.Loaded -= OnMapDataLoaded;
            MapData.Saved -= OnMapDataSaved;
            MapData.EditStateChanged -= OnPropertyChanged;
            PropertyChanged -= OnPropertyChanged;
        }
    }
}
