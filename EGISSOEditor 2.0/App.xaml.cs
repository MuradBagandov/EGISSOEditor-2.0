using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.Services;
using EGISSOEditor_2._0.ViewModels;
using EGISSOEditor_2._0.Models;
using System.Windows.Controls;

namespace EGISSOEditor_2._0
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Window FocusedWindow => Current.Windows.Cast<Window>().Where(i => i.IsFocused).FirstOrDefault();
        public static Window ActiveWindow => Current.Windows.Cast<Window>().Where(i => i.IsActive).FirstOrDefault();
        public static Window MainWindow => Current.Windows.Cast<MainWindow>().FirstOrDefault();
        public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();
        public static bool IsDesignMode { get; set; } = true;
        public static string CurrentDirectory => IsDesignMode == true ?
            Path.GetDirectoryName(GetSourceCodePath()) :
            Environment.CurrentDirectory;

        private static IHost _host;

        protected override async void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
            IsDesignMode = false;
            var host = Host;
            await host.StartAsync().ConfigureAwait(false);
            host.Dispose();
            _host = null;
        }

        protected override void OnExit(ExitEventArgs e)
        {

            base.OnExit(e);
            using (var host = Host)
                host.StopAsync().ConfigureAwait(false);
        }

        internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<IFileRepository<EGISSOFile>, EGISSOFileRepository>();
            services.AddSingleton<IRepositoryProcedureDialog<EGISSOFile>, EGISSORepositoryProcedureDialog>();
            services.AddSingleton<IUserDialog, UserDialog>();
            services.AddSingleton<IEGISSOFileEditor<EGISSOFile>, EGISSOFileEditor>();
        }

        public static string GetSourceCodePath([CallerFilePath] string path = null) => path;

    }
}
