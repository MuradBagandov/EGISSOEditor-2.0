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
    public partial class MessageDialog : Window
    {
        #region Properties
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register(
                nameof(MessageText),
                typeof(string),
                typeof(MessageDialog));


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
               typeof(MessageDialog),
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
               typeof(MessageDialog),
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



        public MessageDialog()
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
            chbForAll.Visibility = Visibility.Visible;

            switch (MessageButtons)
            {
                case ShowMessageButtons.YesNo:CollapseButton(btnCancel, btnOk, chbForAll);break;
                case ShowMessageButtons.YesNoForAll: CollapseButton(btnCancel, btnOk); break;
                case ShowMessageButtons.YesNoCancel: CollapseButton(btnOk, chbForAll); break;
                case ShowMessageButtons.YesNoCancelForAll: CollapseButton(btnOk); break;
                case ShowMessageButtons.Ok:CollapseButton(btnCancel, btnYes, btnNo, chbForAll);break;
                case ShowMessageButtons.OkCancel: CollapseButton(btnYes, btnNo, chbForAll); break;
            }

            void CollapseButton(params Control[] controls)
            {
                foreach (Control control in controls)
                    control.Visibility = Visibility.Collapsed;
            }
        }


        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogResult = chbForAll.IsChecked == true ? Services.Enums.DialogResult.YesForAll: Services.Enums.DialogResult.Yes;
            this.DialogResult = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogResult = chbForAll.IsChecked == true ? Services.Enums.DialogResult.NoForAll : Services.Enums.DialogResult.No;
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
