using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.Services.Enums;

namespace EGISSOEditor_2._0
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IFileRepository<EGISSOFile> Repository => App.Host.Services.GetRequiredService<IFileRepository<EGISSOFile>>();
        private IUserDialog UserDialog => App.Host.Services.GetRequiredService<IUserDialog>();
        private IRepositoryProcedureDialog<EGISSOFile> RepositoryDialog => App.Host.Services.GetRequiredService<IRepositoryProcedureDialog<EGISSOFile>>();

        public MainWindow()
        {
            InitializeComponent();
            Width = Properties.Settings.Default.MainWindowSize.Width;
            Height = Properties.Settings.Default.MainWindowSize.Height;
            Top = Properties.Settings.Default.MainWindowStartupLocation.Height;
            Left = Properties.Settings.Default.MainWindowStartupLocation.Width;

            if (Properties.Settings.Default.MainWindowMaximized)
                WindowState = WindowState.Maximized;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var items = Repository.Items.Where(i => i.IsFileChanged).ToList();

            if (items.Count > 0)
            {
                if (UserDialog.ShowMessage($"У вас есть несохраненные файлы! \n Вы действительно хотите выйти?", Title, ShowMessageIcon.Infomation, ShowMessageButtons.YesNo)
                    == Services.Enums.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            Properties.Settings.Default.MainWindowSize = new System.Drawing.Size((int)Width, (int)Height);
            Properties.Settings.Default.MainWindowStartupLocation = new System.Drawing.Size((int)Left, (int)Top);
            Properties.Settings.Default.MainWindowMaximized = WindowState == WindowState.Maximized;
            Properties.Settings.Default.Save();

            Repository.RemoveAll();
        }

        private async void ListBoxCustom_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = ((string[])e.Data.GetData(DataFormats.FileDrop))
                    .Where(i => i.EndsWith(".xlsx")).ToArray();
                await RepositoryDialog.AddWithShowProgressAsync(files);
            }
        }
    }
}
