using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EGISSOEditor_2._0.Services.Enums;
using EGISSOEditor_2._0.Services.Structs;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    internal interface IUserDialog
    {
        (IProgress<ProcedureElementsProgessReporter> progress, CancellationToken cancel, Action close) ShowProgress();

        DialogResult ShowMessage(string text, string title = "Message", ShowMessageIcon icon = ShowMessageIcon.None, ShowMessageButtons buttons = ShowMessageButtons.Ok, Window owner = null);

    }
}
