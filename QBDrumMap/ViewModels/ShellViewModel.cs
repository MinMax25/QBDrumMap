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
using libQB.WindowServices;
using MahApps.Metro.Controls;
using QBDrumMap.Class;
using QBDrumMap.Class.Enums;
using QBDrumMap.Class.MapModels;
using QBDrumMap.Class.Services;
using QBDrumMap.Contracts.Views;
using QBDrumMap.Properties;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIWindow<IShellWindow, ShellWindow>]
    public partial class ShellViewModel : ViewModelBase, IWindowClosingAware
    {
        #region Fields

        // ハンバーガーメニューで現在選択されている項目
        [ObservableProperty]
        private HamburgerMenuItem _selectedMenuItem;

        // ウィンドウのタイトル文字列
        [ObservableProperty]
        private string _title;

        // コマンドの実行可否状態
        [ObservableProperty]
        private bool _isCommandEnabled = true;

        // ファイルダイアログ用のフィルター定数
        private const string QBD_FILTER = "QB DrumMap (*.qbd)|*.qbd";

        #endregion

        #region Properties

        // 前の画面に戻るコマンド
        public IRelayCommand GoBackCommand => new RelayCommand(OnGoBack, CanGoBack);

        // ハンバーガーメニュー項目が選択された際のコマンド
        public ICommand MenuItemInvokedCommand => new RelayCommand(OnMenuItemInvoked);

        // ウィンドウロード時のコマンド
        public ICommand LoadedCommand => new RelayCommand(OnLoaded);

        // ウィンドウアンロード時のコマンド
        public ICommand UnloadedCommand => new RelayCommand(OnUnloaded);

        // アプリケーション情報（バージョン等）のキャプション
        public string AboutCaption
        {
            get
            {
                return string.Format(Properties.Resources.MenuHelpAboutCaption, AppName);
            }
        }

        // アプリケーション名
        private string AppName
        {
            get
            {
                return typeof(App).Assembly.GetName().Name;
            }
        }

        // 設定サービスへの参照
        private ISettingService Setting
        {
            get
            {
                return SettingService;
            }
        }

        // メインメニュー項目
        public ObservableCollection<HamburgerMenuItemBase> MenuItems { get; } =
        [
            new HamburgerMenuSeparatorItem(),

            new HamburgerMenuIconItem()
            {
                Label = Resources.Title_PluginPage,
                Icon = ClonePackIcon(PageIcon.PluginPage),
                TargetPageType = typeof(PluginViewModel)
            },

            new HamburgerMenuSeparatorItem(),

            new HamburgerMenuIconItem()
            {
                Label = Resources.Title_PartsPage,
                Icon = ClonePackIcon(PageIcon.PartsPage),
                TargetPageType = typeof(PartsViewModel)
            },

            new HamburgerMenuIconItem()
            {
                Label = Resources.Title_ArticulationsPage,
                Icon = ClonePackIcon(PageIcon.ArticulationsPage),
                TargetPageType = typeof(ArticulationsViewModel)
            },

            new HamburgerMenuSeparatorItem(),

            new HamburgerMenuIconItem()
            {
                Label = Resources.Title_ExportPage,
                Icon = ClonePackIcon(PageIcon.ExportPage),
                TargetPageType = typeof(ExportViewModel)
            },

            new HamburgerMenuSeparatorItem(),

            new HamburgerMenuIconItem()
            {
                Label = Resources.Title_ConvertMidiPage,
                Icon = ClonePackIcon(PageIcon.ConvertPage),
                TargetPageType = typeof(ConvertMidiViewModel)
            },

            new HamburgerMenuSeparatorItem()
        ];

        // オプションメニュー項目（設定等）
        public ObservableCollection<HamburgerMenuItemBase> OptionMenuItems { get; } =
        [
            new HamburgerMenuIconItem()
            {
                Label = libQB.Properties.Resources.Setting_PageTitle,
                Icon = ClonePackIcon(PageIcon.SettingPage),
                TargetPageType = typeof(SettingViewModel)
            }
        ];

        #endregion

        #region ctor

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

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private async Task OnNewFile()
        {
            if (MapData.IsModified())
            {
                if (await Dialog.ShowConfirmAsync(libQB.Properties.Dialog.Confirm_NewFile) == false)
                {
                    return;
                }
            }

            MapData.Initialize();
            Setting.LastOpenedFilePath = string.Empty;
        }

        [RelayCommand]
        private async Task OnOpenFile()
        {
            if (MapData.IsModified())
            {
                if (await Dialog.ShowConfirmAsync(libQB.Properties.Dialog.Confirm_NewFile) == false)
                {
                    return;
                }
            }

            var path = await Dialog.ShowOpenFileDialog(
                libQB.Properties.Dialog.Common_OpenFile,
                QBD_FILTER,
                GetValidFilePath(Setting.LastOpenedFilePath));

            if (path == null)
            {
                return;
            }

            Setting.LastOpenedFilePath = path;

            if (await MapData.LoadAsync(path) == false)
            {
                await Dialog.ShowErrorAsync("File IO Error", "Load");
            }
        }

        [RelayCommand]
        private async Task OnSaveFile()
        {
            if (MapData.HasError())
            {
                await Dialog.ShowAlertAsync(libQB.Properties.Dialog.Confirm_SaveHasError);
                return;
            }

            var path = await Dialog.ShowSaveFileDialog(
                libQB.Properties.Dialog.Common_SaveFile,
                QBD_FILTER,
                GetValidFilePath(Setting.LastOpenedFilePath));

            if (path == null)
            {
                return;
            }

            if (await MapData.SaveAsync(path))
            {
                Setting.LastOpenedFilePath = path;
            }
            else
            {
                await Dialog.ShowErrorAsync("File IO Error", "Save");
            }
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
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Document\Manual\index.html");

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

        [RelayCommand]
        private void OnClose()
        {
            WindowService.CloseWindow(Application.Current.MainWindow);
        }

        [RelayCommand]
        private void OnUndo()
        {
            UndoManager.Undo();
        }

        [RelayCommand]
        private void OnRedo()
        {
            UndoManager.Redo();
        }

        #endregion

        #region EventHandler

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
                case nameof(MapData.EditState):
                    IsCommandEnabled = !MapData.EditState;
                    break;
                default:
                    break;
            }
        }

        public async Task<bool> OnWindowClosingAsync()
        {
            if (MapData.IsModified())
            {
                var result = await Dialog.ShowWarningAsync(libQB.Properties.Dialog.Confirm_ApplicationCloseWithTheChanges);
                return result ?? false;
            }
            else
            {
                var result = await Dialog.ShowConfirmAsync(libQB.Properties.Dialog.Confirm_ApplicationClose);
                return result ?? false;
            }
        }

        #endregion

        #region General

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

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                MapData.EditStateChanged -= OnPropertyChanged;
                Setting.PropertyChanged -= OnPropertyChanged;
            }
        }

        #endregion

        #endregion
    }
}