using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using libQB.Attributes;
using libQB.DialogServices;
using libQB.UndoRedo;
using libQB.WindowServices;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QBDrumMap.Class.Services;
using QBDrumMap.Contracts.Services;
using QBDrumMap.Models;
using QBDrumMap.Services;

namespace QBDrumMap
{
    public partial class App
        : Application
    {
        private static Mutex _mutex;

        public static T GetService<T>()
        {
            if (_host is not null && _host.Services.GetService<T>() is T service)
            {
                return service;
            }
            return default;
        }

        public static Dictionary<Type, Type> _pages { get; } = new();

        public static string Text(string key, string fallback = "")
        {
            return libQB.Localization.LocalizationManager.Instance[$"QBDrumMap.Resources.{key}"] ?? fallback;
        }

        public static string NameText(string key, string fallback = "")
        {
            return libQB.Localization.LocalizationManager.Instance[$"QBDrumMap.Name.{key}"] ?? fallback;
        }

        private static IHost _host;

        public App()
        {
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            libQB.Localization.LocalizationManager.Instance.RegisterResourceManager("QBDrumMap.Resources", QBDrumMap.Properties.Resources.ResourceManager);
            libQB.Localization.LocalizationManager.Instance.RegisterResourceManager("QBDrumMap.Name", QBDrumMap.Properties.Name.ResourceManager);
            libQB.Localization.LocalizationManager.Instance.Initialize(Path.Combine(appLocation, "Languages"));
            RegisterDialogTexts();

            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureAppConfiguration(c =>
                    {
                        c.SetBasePath(appLocation);
                    })
                    .ConfigureServices(ConfigureServices)
                    .Build();

            bool isNewInstance;
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            _mutex = new Mutex(true, appName, out isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show(Text("Alert_FileNotFound", "File not found."), appName, MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
                return;
            }

            await _host.StartAsync();
        }

        private static void RegisterDialogTexts()
        {
            DialogTextProvider.OkButtonText = () => Text("Button_OK", "OK");
            DialogTextProvider.CancelButtonText = () => Text("Button_Cancel", "Cancel");
            DialogTextProvider.YesButtonText = () => Text("Button_Yes", "Yes");
            DialogTextProvider.NoButtonText = () => Text("Button_No", "No");
            DialogTextProvider.InformationTitle = () => Text("Title_Dialog_Information", "Information");
            DialogTextProvider.WarningTitle = () => Text("Title_Dialog_Warning", "Warning");
            DialogTextProvider.ErrorTitle = () => Text("Title_Dialog_Error", "Error");
            DialogTextProvider.ConfirmTitle = () => Text("Title_Dialog_Confirm", "Confirm");
            DialogTextProvider.ClearButtonTooltip = () => Text("Title_Command_Clear", "Clear");
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // App Host
            services.AddHostedService<ApplicationHostService>();

            // Activation Handlers

            // Core Services

            // Services
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogCoordinator, DialogCoordinator>();

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<IUndoManager, UndoManager>();

            //
            _pages.Clear();

            foreach (var t in GetDITargets().ToArray())
            {
                var attr = t.GetCustomAttribute<DIAttribute>();
                var attrtype = attr.GetType();

                if (attrtype.Name == typeof(DIWindowAttribute<,>).Name)
                {
                    services.AddTransient(t);
                    services.AddTransient(attr.Type1, attr.Type2);
                }
                else if (attrtype.Name == typeof(DIWindowAttribute<>).Name)
                {
                    services.AddTransient(t);
                    services.AddTransient(attr.Type1);
                }
                else if (attrtype.Name == typeof(DIPageAttribute<>).Name)
                {
                    services.AddTransient(t);
                    services.AddTransient(attr.Type1);
                    _pages.Add(t, attr.Type1);
                }
                else if (attrtype.Name == typeof(DIUserControlAttribute<>).Name)
                {
                    services.AddTransient(t);
                    services.AddTransient(attr.Type1);
                }
                else if (attrtype.Name == typeof(DISingletonAttribute<>).Name)
                {
                    services.AddSingleton(attr.Type1, t);
                }
                else if (attrtype == typeof(DISingletonAttribute))
                {
                    services.AddSingleton(t);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            // Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
        }

        private static IEnumerable<Type> GetDITargets()
        {
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.GetCustomAttribute<DIAttribute>() is DIAttribute)
                {
                    yield return t;
                }
            }
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            GetService<IDIContainer>()?.Flush();

            if (_host == null)
                return;

            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
        }
    }
}
