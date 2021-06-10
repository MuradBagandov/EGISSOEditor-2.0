using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Infrastuctures.Commands
{
    internal class OpenFileCommand : Base.Command
    {
        public override bool CanExecute(object parameter) => true;
        
        public override void Execute(object parameter)
        {
            if (!(parameter is string))
                throw new ArgumentException();

            string str = (string)parameter;

            if (File.Exists(str))
                Process.Start((string)parameter);
        }
    }
}
