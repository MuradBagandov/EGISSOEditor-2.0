using EGISSOEditor_2._0.Services.Enums;
using EGISSOEditor_2._0.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EGISSOEditor_2._0.Services
{
    internal class UserDialog : Interfaces.IUserDialog
    {
        public DialogResult ShowMessage(string text, string title, ShowMessageIcon icon, ShowMessageButtons buttons) =>
            ShowMessage(text, title, icon, buttons, App.MainWindow);



        public DialogResult ShowMessage(string text, string title, ShowMessageIcon icon, ShowMessageButtons buttons, Window owner)
        {
            var dialog = new DialogProcedureWindow()
            {
                Owner = owner,
                Title = title,
                MessageIcon = icon,
                MessageButtons = buttons,
                MessageText = text
            };
            return dialog.ShowDialog() == true ? dialog.ShowDialogResult : DialogResult.Cancel;
        }
    }
}
