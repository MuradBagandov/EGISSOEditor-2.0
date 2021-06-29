using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EGISSOEditor_2._0.Infrastuctures.Commands
{
    internal class SelectAllCommand : Base.Command
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is ListBox listbox))
                throw new ArgumentException();

            return listbox.Items.Count > 0 && listbox.SelectedItems.Count != listbox.Items.Count;
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is ListBox listbox))
                throw new ArgumentException();

            listbox.SelectAll();
        }
    }
}
