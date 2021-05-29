using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            string path = @"D:\Users";

            Console.WriteLine( Extensions.IsDirectory(path));
            path = Path.GetDirectoryName(path);
            Console.WriteLine(path);
            Console.WriteLine(Directory.Exists(path));
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
