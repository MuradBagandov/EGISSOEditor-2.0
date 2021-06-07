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
using EGISSOEditor_2._0.Services.Enums;

namespace EGISSOEditor_2._0.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogProcedureWindow.xaml
    /// </summary>
    public partial class DialogProcedureWindow : Window
    {
        #region Properties
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register(
                nameof(MessageText),
                typeof(string),
                typeof(DialogProcedureWindow));


        public string MessageText
        {
            get => (string)GetValue(MessageTextProperty);
            set
            {
                SetValue(MessageTextProperty, value);
            }
        }

        public static readonly DependencyProperty MessageIconProperty =
           DependencyProperty.Register(
               nameof(MessageIcon),
               typeof(ShowMessageIcon),
               typeof(DialogProcedureWindow),
               new PropertyMetadata(ShowMessageIcon.None));


        public ShowMessageIcon MessageIcon
        {
            get => (ShowMessageIcon)GetValue(MessageIconProperty);
            set => SetValue(MessageIconProperty, value);
        }

        public static readonly DependencyProperty MessageButtonsProperty =
           DependencyProperty.Register(
               nameof(MessageButtonsProperty),
               typeof(ShowMessageButtons),
               typeof(DialogProcedureWindow),
               new PropertyMetadata(ShowMessageButtons.YesNoCancel));

        public ShowMessageButtons MessageButtons
        {
            get => (ShowMessageButtons)GetValue(MessageButtonsProperty);
            set
            {
                SetValue(MessageButtonsProperty, value);
                SetButtonsStyle();
            }
        }

        public DialogResult ShowDialogResult { get; private set; }
        #endregion



        public DialogProcedureWindow()
        {
            InitializeComponent();
            this.Loaded += ShowMessageWindow_Loaded;
        }

        private void ShowMessageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonsStyle();
            switch (MessageIcon)
            {
                case ShowMessageIcon.None: IconMessage.Visibility = Visibility.Collapsed; break;
                case ShowMessageIcon.Warning: IconMessage.Icon = FontAwesome.WPF.FontAwesomeIcon.Warning; break;
                case ShowMessageIcon.Infomation: IconMessage.Icon = FontAwesome.WPF.FontAwesomeIcon.InfoCircle; break;
                case ShowMessageIcon.Error: IconMessage.Icon = FontAwesome.WPF.FontAwesomeIcon.Warning; break;
            }
        }

        private void SetButtonsStyle()
        {
            btnYes.Visibility = Visibility.Visible;
            btnNo.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            btnOk.Visibility = Visibility.Visible;

            switch (MessageButtons)
            {
                case ShowMessageButtons.YesNo:CollapseButton(btnCancel, btnOk);break;
                case ShowMessageButtons.YesNoCancel: CollapseButton(btnOk); break;
                case ShowMessageButtons.Ok:CollapseButton(btnCancel, btnYes, btnNo);break;
                case ShowMessageButtons.OkCancel: CollapseButton(btnYes, btnNo); break;
            }

            void CollapseButton(params Button[] buttons)
            {
                foreach (Button btn in buttons)
                    btn.Visibility = Visibility.Collapsed;
            }
        }


        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogResult = Services.Enums.DialogResult.Yes;
            this.DialogResult = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogResult = Services.Enums.DialogResult.No;
            this.DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogResult = Services.Enums.DialogResult.Cancel;
            this.DialogResult = false;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogResult = Services.Enums.DialogResult.Ok;
            this.DialogResult = false;
            Close();
        }
    }
}
