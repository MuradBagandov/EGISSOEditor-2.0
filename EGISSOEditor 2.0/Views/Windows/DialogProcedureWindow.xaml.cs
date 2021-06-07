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
using System.Windows.Shapes;

namespace EGISSOEditor_2._0.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogProcedureWindow.xaml
    /// </summary>
    public partial class DialogProcedureWindow : Window
    {
        public static readonly DependencyProperty CurrentProcessedElementProperty =
            DependencyProperty.Register(
                nameof(CurrentProcessedElement),
                typeof(string),
                typeof(DialogProcedureWindow));


        public string CurrentProcessedElement 
        { 
            get => (string)GetValue(CurrentProcessedElementProperty);
            set
            {
                SetValue(CurrentProcessedElementProperty, value);
                tbCurrentProcessedElement.Text = value;
            }
        }

        public DialogProcedureWindow()
        {
            InitializeComponent();
            
        }
    }
}
