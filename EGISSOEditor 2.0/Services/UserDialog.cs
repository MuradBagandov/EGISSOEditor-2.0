﻿using EGISSOEditor_2._0.Services.Enums;
using EGISSOEditor_2._0.Services.Structs;
using EGISSOEditor_2._0.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EGISSOEditor_2._0.Services
{
    internal class UserDialog : Interfaces.IUserDialog
    {
        public (IProgress<ProcedureFilesProgess> progress, CancellationToken cancel, Action close) ShowProgress()
        {
            var parent_window = App.ActiveWindow ?? App.MainWindow;
            var _progressWindow = new ProgressWindow()
            {
                Owner = parent_window,
            };

            parent_window.IsEnabled = false;
            _progressWindow.Show();

            return (_progressWindow.Progress, _progressWindow.Cancellation, () => { _progressWindow.Close(); parent_window.IsEnabled = true; });
        }


        public DialogResult ShowMessage(string text, string title, ShowMessageIcon icon, ShowMessageButtons buttons, Window owner = null)
        {
            var dialog = new MessageDialog()
            {
                Owner = owner ?? App.ActiveWindow ?? App.MainWindow,
                Title = title,
                MessageIcon = icon,
                MessageButtons = buttons,
                MessageText = text
            };
            return dialog.ShowDialog() == true ? dialog.ShowDialogResult : DialogResult.Cancel;
        }
    }
}
