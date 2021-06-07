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
        private IFileRepository<EGISSOFile> repository => App.Host.Services.GetRequiredService<IFileRepository<EGISSOFile>>();
        private IUserDialog userDialog => App.Host.Services.GetRequiredService<IUserDialog>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var items = repository.Items.Where(i => i.IsFileChanged == true).ToList();

            if (items.Count > 0)
            {
                if (userDialog.ShowMessage($"У вас есть несохраненные файлы! \n Вы действительно хотите выйти?", Title, ShowMessageIcon.Infomation, ShowMessageButtons.YesNo) 
                    == Services.Enums.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                    
            }

            repository.RemoveAll();      
        }
    }
}
