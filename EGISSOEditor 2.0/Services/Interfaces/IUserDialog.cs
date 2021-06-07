using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGISSOEditor_2._0.Services.Enums;

namespace EGISSOEditor_2._0.Services.Interfaces
{
    internal interface IUserDialog
    {
        DialogResult ShowMessage(string text, string title, ShowMessageIcon icon, ShowMessageButtons buttons);

    }
}
