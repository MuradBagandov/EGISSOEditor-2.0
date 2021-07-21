using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Spire.Xls;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Workbook workbook = new Workbook();
            workbook.LoadFromFile("d:\\input.xlsb");
            //Console.WriteLine(Path.ChangeExtension("d:\\input.xls", ".xlsx"));
            workbook.SaveToFile("d:\\input.xlsx", ExcelVersion.Version2013);
            Console.ReadLine();
        }

        

    }

    public static class Extensions
    {
        public static bool IsDirectory(string fileName)
        {
            if ((fileName == null) || (fileName.IndexOfAny(Path.GetInvalidPathChars()) != -1))
                return false;
            try
            {
                var tempFileInfo = new FileInfo(fileName);
                return true;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }

}
