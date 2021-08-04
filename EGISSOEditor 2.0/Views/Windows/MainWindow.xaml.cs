using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.Services.Enums;
using System.Collections.Specialized;

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

        private DataTemplate _wrapDataTemplate, _listDataTemplate;

        public MainWindow()
        {
            InitializeComponent();
            Width = ApplicationSettings.MainWindowSize.Width;
            Height = ApplicationSettings.MainWindowSize.Height;
            Top = ApplicationSettings.MainWindowStartupLocation.Height;
            Left = ApplicationSettings.MainWindowStartupLocation.Width;

            if (ApplicationSettings.IsMainWindowMaximized)
                WindowState = WindowState.Maximized;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<EGISSOFile> items = Repository.Items.Where(i => i.IsFileChanged).ToList();

            if (items.Count > 0)
            {
                if (UserDialog.ShowMessage($"У вас есть несохраненные файлы! \n Вы действительно хотите выйти?", Title, ShowMessageIcon.Infomation, ShowMessageButtons.YesNo)
                    == Services.Enums.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            ApplicationSettings.MainWindowSize = new System.Drawing.Size((int)Width, (int)Height);
            ApplicationSettings.MainWindowStartupLocation = new System.Drawing.Size((int)Left, (int)Top);
            ApplicationSettings.IsMainWindowMaximized = WindowState == WindowState.Maximized;
            Repository.RemoveAll();
        }

        private async void ListBoxCustom_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> addFiles = new List<string>(100);
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] folders = files.Where(i => Directory.Exists(i)).ToArray();
                addFiles.AddRange(files);

                foreach (string folder in folders)
                {
                    files = Directory.GetFiles(folder, "*.xls");
                    addFiles.AddRange(files);
                }
                files = addFiles.Where(i => i.EndsWith(".xlsx") || i.EndsWith(".xls") || i.EndsWith(".xlsm") || i.EndsWith(".xlsb")).ToArray();
                await RepositoryDialog.AddWithShowProgressAsync(files);
            }
        }

        private void lbFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStatusFilesCount();
        }

        private void lbFiles_Loaded(object sender, RoutedEventArgs e)
        {
            ((INotifyCollectionChanged)lbFiles.Items).CollectionChanged += (s, e) =>
            {
                UpdateStatusFilesCount();
            };
            _wrapDataTemplate = (DataTemplate)TryFindResource("WrapDateTemplate");
            _listDataTemplate = (DataTemplate)TryFindResource("ListDateTemplate");
            ChangedListBoxStyle();
        }

        private void UpdateStatusFilesCount()
        {
            StatusFilesCount.Text = lbFiles.SelectedItems.Count == 0 ?
                $"Всего элементов: {lbFiles.Items.Count}" : $"Выбрано элементов: {lbFiles.SelectedItems.Count} из {lbFiles.Items.Count}";
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ChangedListBoxStyle();
        }

        private void ChangedListBoxStyle()
        {
            lbFiles.ItemTemplate = ToggleListStyle.IsChecked != true ? _wrapDataTemplate : _listDataTemplate;
            Type panelType = ToggleListStyle.IsChecked != true ? typeof(WrapPanel) : typeof(VirtualizingStackPanel);
            ItemsPanelTemplate ipt = new ItemsPanelTemplate(new FrameworkElementFactory(panelType));
            lbFiles.ItemsPanel = ipt;
        }
    }
}
