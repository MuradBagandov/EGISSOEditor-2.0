using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace EGISSOEditor_2._0
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            _ = new Mutex(false, "9ko1cG2CwruC9Ea8", out bool result);
            if (result)
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] Args) => Host.CreateDefaultBuilder(Args).
            UseContentRoot(App.CurrentDirectory).
            ConfigureAppConfiguration((host, cfg) =>
            {
                cfg.SetBasePath(App.CurrentDirectory).
                AddJsonFile("appsettings.json", true, true);
            }).
            ConfigureServices(App.ConfigureServices);
    }
}
