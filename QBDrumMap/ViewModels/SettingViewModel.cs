using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Class.Services;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIPage<SettingPage>]
    public partial class SettingViewModel : ViewModelBase
    {
        #region Fields

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
                Properties.Resources.Setting_CubaseInstallPath,
                Setting.CubaseInstallPath);

            if (result is string path)
            {
                Setting.CubaseInstallPath = path;
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
            }
        }

        #endregion

        #endregion
    }
}