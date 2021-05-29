using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using EGISSOEditor_2._0.ViewModels.Base;

namespace EGISSOEditor_2._0.Models
{
    internal class EGISSOFile: ViewModel
    {
        public int Id { get;  }

        private string _directory;
        public string Directory {
            get => _directory;
            set 
            { 
                Set(ref _directory, value);
                OnPropertyChanged(nameof(Name));
            }
        }

        public string TemplateDirectory { get; }

        public string Name => Path.GetFileName(Directory);

        public bool IsFileChanged { get; set; }

        public EGISSOFile(int id, string directory, string templateDirectory)
        {
            Id = id;
            Directory = directory;
            TemplateDirectory = templateDirectory;
        }

    }
}
