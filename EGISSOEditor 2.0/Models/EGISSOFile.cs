using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGISSOEditor_2._0.Models
{
    internal class EGISSOFile
    {
        public int Id { get;  }

        public string Directory { get; set; }

        public string TemplateDirectory { get; }

        public string Name { get; }

        public bool IsFileChanged { get; set; }

        public EGISSOFile(int id, string directory, string templateDirectory, string name)
        {
            Id = id;
            Directory = directory;
            TemplateDirectory = templateDirectory;
            Name = name;
        }


    }
}
