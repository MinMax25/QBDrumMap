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

namespace QBDrumMap.ViewModels
{
    [DIPage<PluginPage>]
    public partial class PluginViewModel : ViewModelBaseHasContents
    {
        #region Fields

        // プラグイン一覧のビュー
        [ObservableProperty]
        private ICollectionView _pluginsView;

        // 選択されている複数のプラグイン
        [ObservableProperty]
        private ObservableCollection<Plugin> _selectedPlugins = new();

        // 編集対象として選択されている単一のプラグイン
        [ObservableProperty]
        private Plugin _selectedPlugin;

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

        #endregion

        #region ctor

        public PluginViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
            Task.Run(InitializeDataAsync);
            ContentViewModel = App.GetService<KitPanelFactory>()?.Create(null);

            MapData.Loaded += OnMapDataLoaded;
            MapData.Saved += OnMapDataSaved;
            MapData.EditStateChanged += OnPropertyChanged;
        }

        #endregion

        #region Methods

        #region Commands

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
                libQB.Properties.Resources.Message_Command_Delete,
                Properties.Name.Plugin,
                names);

            if (await Dialog.ShowConfirmAsync(message, libQB.Properties.Dialog.Title_Command_Delete) == false)
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
                ID = MapData.Plugins.GetNewID()
            };
            plugin.Name = $"Plugin{plugin.ID}";

            // Expression Lambda (式ツリー) を使用
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

        #endregion

        #region EventHandler

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

        #endregion

        #region PropertyChanged Callbacks

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

        #endregion

        #region General

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
                MapData.Plugins.Clear();
                foreach (var plugin in sortedPlugins)
                {
                    MapData.Plugins.Add(plugin);
                }

                PluginsView = CollectionViewSource.GetDefaultView(MapData.Plugins);
                SelectedPlugin = null;

                MapData.SetEditState(this, false);
            });
        }

        #endregion

        #region Dispose

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

        #endregion

        #endregion
    }
}