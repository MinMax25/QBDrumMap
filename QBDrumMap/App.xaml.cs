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
using QBDrumMap.Class.Extentions;
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
            if (_host.Services.GetService<T>() is T service)
            {
                return service;
            }
            return default;
        }

        private static IHost _host;

        public App()
        {
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

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
                MessageBox.Show(libQB.Properties.Dialog.Alert_FileNotFound, appName, MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
                return;
            }

            await _host.StartAsync();
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
            PageHolder.Items.Clear();

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
                    PageHolder.Items.Add(t, attr.Type1);
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

            if (_host == null) return;

            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
        }
    }
}
