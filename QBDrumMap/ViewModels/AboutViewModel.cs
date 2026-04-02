using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels
{
    [DIWindow<About>]
    public partial class AboutViewModel : ViewModelBase
    {
        #region Fields

        // ライセンス情報のテキスト
        [ObservableProperty]
        private string _license;

        // アプリケーション名
        [ObservableProperty]
        private string _applicationName;

        // バージョン情報文字列
        [ObservableProperty]
        private string _version;

        // コピーライト表記
        [ObservableProperty]
        private string _copyright = "© 2025 Min Max";

        #endregion

        #region ctor

        public AboutViewModel()
        {
            ApplicationName = typeof(App).Assembly.GetName().Name;

            string fullName = typeof(App).Assembly.Location;
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(fullName);

            // フィールドに直接代入せず、プロパティまたは通知を伴う形式で設定
            Version = $"Version {info.FileVersion}";

            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string directory = Path.GetDirectoryName(assemblyLocation);

            if (directory != null)
            {
                string path = Path.Combine(directory, @"Resources\license.txt");

                if (File.Exists(path))
                {
                    License = File.ReadAllText(path);
                }
            }
        }

        #endregion
    }
}