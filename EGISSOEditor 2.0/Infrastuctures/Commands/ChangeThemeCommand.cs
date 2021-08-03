using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Infrastuctures.Commands
{
    internal class ChangeThemeCommand : Base.Command
    {
        public override bool CanExecute(object parameter) => true;
       

        public override void Execute(object parameter)
        {
            ThemeContoller.CurrentTheme = ThemeContoller.CurrentTheme == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark;
        }
    }
}
