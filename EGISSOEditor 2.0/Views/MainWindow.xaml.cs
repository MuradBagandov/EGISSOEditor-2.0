using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using EGISSOEditor_2._0.Models;
using EGISSOEditor_2._0.Services;
using EGISSOEditor_2._0.Services.Interfaces;
using EGISSOEditor_2._0.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EGISSOEditor_2._0
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IFileRepository<EGISSOFile> repository => App.Host.Services.GetRequiredService<IFileRepository<EGISSOFile>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var items = repository.Items.Where(i => i.IsFileChanged == true).ToList();

            if (items.Count > 0)
            {
                if (MessageBox.Show($"У вас есть несохраненные файлы! \n Вы действительно хотите выйти?", Title, MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                    
            }

            repository.RemoveAll();      
        }
    }
}
