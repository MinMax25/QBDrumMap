using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using libQB.Attributes;
using QBDrumMap.Class;
using QBDrumMap.Views;

namespace QBDrumMap.ViewModels;

[DIWindow<About>]
public partial class AboutViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _license;

    [ObservableProperty]
    private string _applicationName;

    [ObservableProperty]
    private string _version;

    [ObservableProperty]
    private string _copyright = "© 2025 Min Max";

    public AboutViewModel()
    {
        ApplicationName = typeof(App).Assembly.GetName().Name;

        string fullName = typeof(App).Assembly.Location;
        FileVersionInfo info = FileVersionInfo.GetVersionInfo(fullName);

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
        }