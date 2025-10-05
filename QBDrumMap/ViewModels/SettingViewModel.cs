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
    public partial class SettingViewModel
        : ViewModelBase
    {
        #region Properties

        public ISettingService Setting => SettingService;

        public IMidiService MIDI => App.GetService<IMidiService>();

        public Dictionary<string, Brush> AccentColors { get; set; }

        #endregion

        #region Fields

        #endregion

        #region ctor

        public SettingViewModel(IDIContainer diContainer)
            : base(diContainer)
        {
            AccentColors =
                ThemeManager.Current.Themes
                .GroupBy(x => x.ColorScheme)
                .OrderBy(a => a.Key)
                .Select(a => new { Name = a.Key, ColorBrush = a.First().ShowcaseBrush })
                .ToDictionary(x => x.Name, x => x.ColorBrush);
        }

        #endregion

        #region Methods

        #region Commands

        [RelayCommand]
        private async Task OnSelectCubaseInstallPath()
        {
            if (await Dialog.ShowSelectFolderDialog(Properties.Resources.Setting_CubaseInstallPath, Setting.CubaseInstallPath) is string path)
            {
                Setting.CubaseInstallPath = path;
            }
        }

        #endregion

        #endregion
    }
}
