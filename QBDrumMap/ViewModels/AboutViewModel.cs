using System.IO;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIWindow<About>]
    public partial class AboutViewModel
        : ViewModelBase
    {
        #region Properties

        [ObservableProperty]
        private string license;

        [ObservableProperty]
        private string applicationName;

        [ObservableProperty]
        private string version;

        [ObservableProperty]
        private string copyright = "© 2025 Min Max";

        #endregion

        #region ctor

        public AboutViewModel()
        {
            ApplicationName = typeof(App).Assembly.GetName().Name;

            var fullname = typeof(App).Assembly.Location;
            var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(fullname);
            version = $"Version {info.FileVersion}";

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\license.txt");
            if (File.Exists(path))
            {
                License = File.ReadAllText(path);
            }
        }

        #endregion
    }
}
